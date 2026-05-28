#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""FlaUI BDD 測試控制台：Feature 勾選、執行進度、結果檢視。"""

from __future__ import annotations

import json
import os
import re
import subprocess
import sys
import urllib.error
import urllib.request
import threading
import time
import uuid
import xml.etree.ElementTree as ET
from datetime import datetime, timezone
from http import HTTPStatus
from http.server import SimpleHTTPRequestHandler
from pathlib import Path
from socketserver import ThreadingMixIn, TCPServer
from urllib.parse import urlparse

# Chrome/Edge 封鎖 6665-6669（ERR_UNSAFE_PORT），預設使用安全埠 6688
PORT = int(os.environ.get("FLAUIBDD_DASHBOARD_PORT", "6688"))
WEB_ROOT = Path(__file__).resolve().parent
PROJECT_ROOT = WEB_ROOT.parent
TEST_PROJECT = PROJECT_ROOT / "Testcase_shopping_cart_FlaUI_BDD"
FEATURES_DIR = TEST_PROJECT / "Features"
REPORTS_DIR = TEST_PROJECT / "reports"
DATA_DIR = WEB_ROOT / "data"
STATUS_FILE = DATA_DIR / "status.json"

_job_lock = threading.Lock()
_current_job: dict | None = None
_demo_server_proc: subprocess.Popen | None = None


def _demo_url_ready(url: str = "http://127.0.0.1:8888/") -> bool:
    try:
        with urllib.request.urlopen(url, timeout=2) as resp:
            return resp.status == 200
    except (urllib.error.URLError, TimeoutError, OSError):
        return False


def ensure_demo_server() -> None:
    global _demo_server_proc
    if _demo_url_ready():
        return

    serve_script = PROJECT_ROOT.parent / "demo" / "shopping_cart" / "serve.py"
    if not serve_script.exists():
        raise FileNotFoundError(f"找不到 demo 伺服器：{serve_script}")

    demo_dir = serve_script.parent
    _demo_server_proc = subprocess.Popen(
        [sys.executable, str(serve_script)],
        cwd=str(demo_dir),
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL,
    )

    deadline = time.time() + 20
    while time.time() < deadline:
        if _demo_url_ready():
            return
        time.sleep(0.5)

    raise TimeoutError("Demo 購物車網頁 (localhost:8888) 啟動逾時")


def utc_now() -> str:
    return datetime.now(timezone.utc).isoformat()


def read_status() -> dict:
    if STATUS_FILE.exists():
        try:
            return json.loads(STATUS_FILE.read_text(encoding="utf-8"))
        except (json.JSONDecodeError, OSError):
            pass
    return {"status": "idle", "scenarios": [], "progress": {"total": 0, "completed": 0, "passed": 0, "failed": 0, "skipped": 0}}


def write_status(payload: dict) -> None:
    DATA_DIR.mkdir(parents=True, exist_ok=True)
    STATUS_FILE.write_text(json.dumps(payload, ensure_ascii=False, indent=2), encoding="utf-8")


def extract_tc_code(title: str) -> str:
    match = re.match(r"(TC\d+)", title.strip())
    return match.group(1) if match else title.strip()[:32]


def parse_feature_file(path: Path) -> dict:
    lines = path.read_text(encoding="utf-8").splitlines()
    feature_title = path.stem
    feature_description: list[str] = []
    pending_tags: list[str] = []
    scenarios: list[dict] = []
    in_description = False

    for raw in lines:
        line = raw.strip()
        if not line or line.startswith("#"):
            continue

        if line.startswith("@"):
            pending_tags = [part.lstrip("@") for part in line.split() if part.startswith("@")]
            continue

        if line.startswith("Feature:"):
            feature_title = line.split(":", 1)[1].strip()
            in_description = True
            continue

        if line.startswith("Scenario Outline:"):
            title = line.split(":", 1)[1].strip()
            scenarios.append(
                {
                    "id": f"{path.stem}::{title}",
                    "title": title,
                    "filterKey": extract_tc_code(title),
                    "tags": pending_tags.copy(),
                    "type": "outline",
                    "file": path.name,
                    "feature": feature_title,
                }
            )
            pending_tags = []
            in_description = False
            continue

        if line.startswith("Scenario:"):
            title = line.split(":", 1)[1].strip()
            scenarios.append(
                {
                    "id": f"{path.stem}::{title}",
                    "title": title,
                    "filterKey": extract_tc_code(title),
                    "tags": pending_tags.copy(),
                    "type": "scenario",
                    "file": path.name,
                    "feature": feature_title,
                }
            )
            pending_tags = []
            in_description = False
            continue

        if in_description and not line.startswith("|") and not line.startswith("Examples:"):
            if line.startswith(("Given", "When", "Then", "And", "But")):
                in_description = False
            else:
                feature_description.append(line)

    return {
        "file": path.name,
        "path": str(path.relative_to(PROJECT_ROOT)).replace("\\", "/"),
        "title": feature_title,
        "description": "\n".join(feature_description).strip(),
        "scenarios": scenarios,
    }


def load_features() -> list[dict]:
    if not FEATURES_DIR.exists():
        return []
    features = []
    for path in sorted(FEATURES_DIR.glob("*.feature")):
        features.append(parse_feature_file(path))
    return features


def build_filter(selected: list[dict]) -> str:
    keys = []
    for item in selected:
        key = item.get("filterKey") or extract_tc_code(item.get("title", ""))
        if key and key not in keys:
            keys.append(key)
    if not keys:
        return ""
    return "|".join(f"Name~{key}" for key in keys)


def parse_junit_results() -> dict | None:
    junit_path = REPORTS_DIR / "junit-results.xml"
    if not junit_path.exists():
        return None

    tree = ET.parse(junit_path)
    root = tree.getroot()
    if root.tag == "testsuites":
        suites = list(root.findall("testsuite"))
    elif root.tag == "testsuite":
        suites = [root]
    else:
        suites = []

    cases: list[dict] = []
    passed = failed = skipped = 0

    for suite in suites:
        for case in suite.findall("testcase"):
            name = case.get("name", "")
            classname = case.get("classname", "")
            duration = float(case.get("time", "0") or 0)
            failure = case.find("failure")
            skipped_el = case.find("skipped")

            if failure is not None:
                status = "failed"
                failed += 1
                message = failure.get("message") or (failure.text or "").strip()
            elif skipped_el is not None:
                status = "skipped"
                skipped += 1
                message = skipped_el.get("message") or ""
            else:
                status = "passed"
                passed += 1
                message = ""

            cases.append(
                {
                    "name": name,
                    "classname": classname,
                    "status": status,
                    "durationSec": round(duration, 2),
                    "message": message[:500] if message else "",
                }
            )

    total = len(cases)
    return {
        "generatedAt": datetime.fromtimestamp(junit_path.stat().st_mtime, timezone.utc).isoformat(),
        "summary": {
            "total": total,
            "passed": passed,
            "failed": failed,
            "skipped": skipped,
            "passRate": round((passed / total) * 100, 1) if total else 0,
        },
        "cases": cases,
        "reports": {
            "junit": str(junit_path.relative_to(PROJECT_ROOT)).replace("\\", "/"),
            "html": "Testcase_shopping_cart_FlaUI_BDD/reports/TestReport.html",
            "summaryHtml": "Testcase_shopping_cart_FlaUI_BDD/reports/summary_report.html",
        },
    }


def recalc_progress(status: dict) -> None:
    scenarios = status.get("scenarios", [])
    passed = failed = skipped = completed = 0
    for scenario in scenarios:
        st = scenario.get("status", "pending")
        if st == "passed":
            passed += 1
            completed += 1
        elif st == "failed":
            failed += 1
            completed += 1
        elif st == "skipped":
            skipped += 1
            completed += 1
    status["progress"] = {
        "total": len(scenarios),
        "completed": completed,
        "passed": passed,
        "failed": failed,
        "skipped": skipped,
    }


def run_tests_job(job_id: str, selected: list[dict], configuration: str) -> None:
    global _current_job

    scenarios_state = [
        {
            "id": s["id"],
            "title": s["title"],
            "filterKey": s["filterKey"],
            "feature": s.get("feature", ""),
            "tags": s.get("tags", []),
            "status": "pending",
            "message": "",
        }
        for s in selected
    ]

    status = {
        "jobId": job_id,
        "status": "running",
        "startedAt": utc_now(),
        "finishedAt": None,
        "configuration": configuration,
        "selectedCount": len(selected),
        "scenarios": scenarios_state,
        "progress": {
            "total": len(scenarios_state),
            "completed": 0,
            "passed": 0,
            "failed": 0,
            "skipped": 0,
        },
        "exitCode": None,
        "filter": build_filter(selected),
        "logTail": [],
    }
    write_status(status)

    try:
        ensure_demo_server()
        status["logTail"] = (status.get("logTail", []) + ["Demo 網頁已就緒: http://localhost:8888/"])[-80:]
        write_status(status)
    except (FileNotFoundError, TimeoutError) as exc:
        status["status"] = "failed"
        status["finishedAt"] = utc_now()
        status["logTail"] = [f"無法啟動 Demo 網頁: {exc}"]
        write_status(status)
        with _job_lock:
            _current_job = None
        return

    REPORTS_DIR.mkdir(parents=True, exist_ok=True)
    junit_path = REPORTS_DIR / "junit-results.xml"
    if junit_path.exists():
        junit_path.unlink()

    filter_expr = status["filter"]
    cmd = [
        "dotnet",
        "test",
        "-c",
        configuration,
        "--logger",
        f"junit;LogFilePath={junit_path}",
        "--logger",
        "console;verbosity=normal",
    ]
    if filter_expr:
        cmd.extend(["--filter", filter_expr])

    try:
        proc = subprocess.Popen(
            cmd,
            cwd=str(TEST_PROJECT),
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
            encoding="utf-8",
            errors="replace",
        )
    except OSError as exc:
        status["status"] = "failed"
        status["finishedAt"] = utc_now()
        status["exitCode"] = -1
        status["logTail"] = [f"無法啟動 dotnet test: {exc}"]
        write_status(status)
        with _job_lock:
            _current_job = None
        return

    result_pattern = re.compile(r"\b(Passed|Failed|Skipped)\s+(.+?)(?:\s+\[|\s*$)")
    running_pattern = re.compile(r"Running\s+(.+)")

    assert proc.stdout is not None
    for line in proc.stdout:
        line = line.rstrip()
        if not line:
            continue
        status["logTail"] = (status["logTail"] + [line])[-80:]
        write_status(status)

        running_match = running_pattern.search(line)
        if running_match:
            name = running_match.group(1).strip()
            for scenario in status["scenarios"]:
                if scenario["filterKey"] in name or scenario["title"] in name:
                    scenario["status"] = "running"
                    break

        result_match = result_pattern.search(line)
        if result_match:
            outcome, name = result_match.group(1).lower(), result_match.group(2).strip()
            for scenario in status["scenarios"]:
                key = scenario["filterKey"]
                if key in name or scenario["title"] in name:
                    scenario["status"] = outcome
                    recalc_progress(status)
                    break

    proc.wait()
    exit_code = proc.returncode

    junit = parse_junit_results()
    if junit:
        for case in junit["cases"]:
            for scenario in status["scenarios"]:
                key = scenario["filterKey"]
                if key in case["name"]:
                    scenario["status"] = case["status"]
                    scenario["durationSec"] = case["durationSec"]
                    if case["message"]:
                        scenario["message"] = case["message"]
        recalc_progress(status)

    status["status"] = "completed" if exit_code == 0 else "completed_with_failures"
    status["finishedAt"] = utc_now()
    status["exitCode"] = exit_code
    write_status(status)

    with _job_lock:
        _current_job = None


class DashboardHandler(SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=str(WEB_ROOT), **kwargs)

    def log_message(self, format: str, *args) -> None:
        if args and str(args[0]).startswith("GET /api/"):
            return
        super().log_message(format, *args)

    def _send_json(self, payload: dict, code: int = HTTPStatus.OK) -> None:
        body = json.dumps(payload, ensure_ascii=False).encode("utf-8")
        self.send_response(code)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.send_header("Access-Control-Allow-Origin", "*")
        self.end_headers()
        self.wfile.write(body)

    def _read_json_body(self) -> dict:
        length = int(self.headers.get("Content-Length", 0))
        if length <= 0:
            return {}
        raw = self.rfile.read(length)
        return json.loads(raw.decode("utf-8"))

    def do_OPTIONS(self) -> None:
        self.send_response(HTTPStatus.NO_CONTENT)
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
        self.send_header("Access-Control-Allow-Headers", "Content-Type")
        self.end_headers()

    def do_GET(self) -> None:
        parsed = urlparse(self.path)
        path = parsed.path

        if path == "/api/features":
            self._send_json({"features": load_features(), "testProject": str(TEST_PROJECT.name)})
            return

        if path == "/api/status":
            self._send_json(read_status())
            return

        if path == "/api/results":
            junit = parse_junit_results()
            status = read_status()
            self._send_json({"junit": junit, "lastRun": status})
            return

        if path == "/api/health":
            self._send_json(
                {
                    "ok": True,
                    "featuresDir": str(FEATURES_DIR),
                    "testProjectExists": TEST_PROJECT.exists(),
                    "jobRunning": _current_job is not None,
                }
            )
            return

        if path.startswith("/reports/"):
            rel = path[len("/reports/") :]
            target = REPORTS_DIR / rel
            if target.exists() and target.is_file():
                self.send_response(HTTPStatus.OK)
                content = target.read_bytes()
                if target.suffix == ".html":
                    ctype = "text/html; charset=utf-8"
                elif target.suffix == ".xml":
                    ctype = "application/xml; charset=utf-8"
                elif target.suffix == ".png":
                    ctype = "image/png"
                else:
                    ctype = "application/octet-stream"
                self.send_header("Content-Type", ctype)
                self.send_header("Content-Length", str(len(content)))
                self.end_headers()
                self.wfile.write(content)
                return
            self.send_error(HTTPStatus.NOT_FOUND)
            return

        if path in ("/", "/index.html"):
            self.path = "/index.html"
        return super().do_GET()

    def do_POST(self) -> None:
        global _current_job

        parsed = urlparse(self.path)
        if parsed.path != "/api/run":
            self.send_error(HTTPStatus.NOT_FOUND)
            return

        try:
            body = self._read_json_body()
        except json.JSONDecodeError:
            self._send_json({"error": "無效的 JSON"}, HTTPStatus.BAD_REQUEST)
            return

        with _job_lock:
            if _current_job is not None:
                self._send_json({"error": "已有測試正在執行中"}, HTTPStatus.CONFLICT)
                return

        scenario_ids = body.get("scenarioIds", [])
        features = load_features()
        selected: list[dict] = []
        for feat in features:
            for scenario in feat["scenarios"]:
                if scenario["id"] in scenario_ids:
                    selected.append(scenario)

        if not selected:
            self._send_json({"error": "請至少勾選一個 Scenario"}, HTTPStatus.BAD_REQUEST)
            return

        configuration = body.get("configuration", "Release")
        job_id = str(uuid.uuid4())

        thread = threading.Thread(
            target=run_tests_job,
            args=(job_id, selected, configuration),
            daemon=True,
        )
        with _job_lock:
            _current_job = {"jobId": job_id, "startedAt": utc_now()}
        thread.start()

        self._send_json({"jobId": job_id, "message": "測試已開始", "filter": build_filter(selected), "selected": len(selected)})


class ThreadingHTTPServer(ThreadingMixIn, TCPServer):
    allow_reuse_address = True
    daemon_threads = True


def main() -> None:
    DATA_DIR.mkdir(parents=True, exist_ok=True)
    if not TEST_PROJECT.exists():
        print(f"警告: 找不到測試專案 {TEST_PROJECT}", file=sys.stderr)

    with ThreadingHTTPServer(("", PORT), DashboardHandler) as httpd:
        url = f"http://localhost:{PORT}/"
        print("FlaUI BDD 測試控制台")
        print(f"  {url}")
        print("  勾選 Features | 執行進度 | 測試結果")
        print("按 Ctrl+C 停止。")
        httpd.serve_forever()


if __name__ == "__main__":
    main()
