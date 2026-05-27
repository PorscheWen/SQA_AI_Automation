#!/usr/bin/env python3
"""
依 report_prompt.md（TestComplete Summary Report 格式）從 JUnit XML 產生摘要報告。

輸出：
  - summary_report.md
  - summary_report.html
"""

from __future__ import annotations

import argparse
import html
import os
import platform
import socket
import sys
import xml.etree.ElementTree as ET
from dataclasses import dataclass, field
from datetime import datetime, timezone
from pathlib import Path
from typing import Iterable


@dataclass
class TestCaseRow:
    name: str
    project: str
    start_time: str
    duration_sec: float
    status: str  # Passed | Failed | Error | Skipped | Warning
    detail: str = ""


@dataclass
class SummaryStats:
    session_name: str
    project_name: str
    start_time: str
    end_time: str
    duration_sec: float
    computer: str
    user: str
    run_count: int = 0
    passed: int = 0
    failed: int = 0
    warnings: int = 0
    errors: int = 0
    skipped: int = 0
    rerun: int = 0
    unexecuted: int = 0
    cases: list[TestCaseRow] = field(default_factory=list)


def _local_username() -> str:
    try:
        return os.getlogin()
    except OSError:
        return os.environ.get("USERNAME") or os.environ.get("USER") or "unknown"


def _parse_timestamp(value: str | None) -> str:
    if not value:
        return "—"
    try:
        # ISO8601 e.g. 2026-05-28T10:00:00 or with Z
        normalized = value.replace("Z", "+00:00")
        dt = datetime.fromisoformat(normalized)
        if dt.tzinfo is None:
            dt = dt.replace(tzinfo=timezone.utc)
        return dt.astimezone().strftime("%Y-%m-%d %H:%M:%S")
    except ValueError:
        return value


def _case_status(testcase: ET.Element) -> tuple[str, str]:
    skipped = testcase.find("skipped")
    if skipped is not None:
        msg = (skipped.get("message") or skipped.text or "").strip()
        return "Skipped", msg

    failure = testcase.find("failure")
    if failure is not None:
        msg = (failure.get("message") or failure.text or "").strip()
        return "Failed", msg

    error = testcase.find("error")
    if error is not None:
        msg = (error.get("message") or error.text or "").strip()
        return "Error", msg

    # 部分工具以 outcome 屬性標記
    outcome = (testcase.get("status") or "").lower()
    if outcome in ("failed", "fail"):
        return "Failed", testcase.get("message") or ""
    if outcome in ("error",):
        return "Error", testcase.get("message") or ""
    if outcome in ("skipped", "skip"):
        return "Skipped", ""
    if outcome in ("warning", "warn"):
        return "Warning", testcase.get("message") or ""

    return "Passed", ""


def _iter_testsuites(root: ET.Element) -> Iterable[ET.Element]:
    if root.tag == "testsuites":
        suites = root.findall("testsuite")
        if suites:
            yield from suites
        else:
            yield from root.findall(".//testsuite")
    elif root.tag == "testsuite":
        yield root
    else:
        yield from root.findall(".//testsuite")


def _collect_cases(junit_path: Path, default_project: str) -> SummaryStats:
    tree = ET.parse(junit_path)
    root = tree.getroot()

    session_name = root.get("name") or junit_path.stem
    project_name = default_project
    suite_timestamp = _parse_timestamp(root.get("timestamp"))
    start_time = suite_timestamp
    end_time = suite_timestamp
    total_time = 0.0

    cases: list[TestCaseRow] = []
    passed = failed = warnings = errors = skipped = 0

    suites = list(_iter_testsuites(root))
    if not suites and root.tag == "testsuite":
        suites = [root]

    for suite in suites:
        if suite.get("name"):
            if project_name == default_project:
                project_name = suite.get("name") or default_project
            session_name = suite.get("name") or session_name

        ts = suite.get("timestamp")
        if ts:
            start_time = _parse_timestamp(ts)

        try:
            total_time += float(suite.get("time") or 0)
        except ValueError:
            pass

        for tc in suite.findall("testcase"):
            name = tc.get("name") or "Unnamed"
            classname = tc.get("classname") or project_name
            try:
                duration = float(tc.get("time") or 0)
            except ValueError:
                duration = 0.0

            status, detail = _case_status(tc)
            cases.append(
                TestCaseRow(
                    name=name,
                    project=classname,
                    start_time=start_time,
                    duration_sec=duration,
                    status=status,
                    detail=detail,
                )
            )

            if status == "Passed":
                passed += 1
            elif status == "Failed":
                failed += 1
            elif status == "Error":
                errors += 1
            elif status == "Skipped":
                skipped += 1
            elif status == "Warning":
                warnings += 1

    run_count = len(cases)
    now = datetime.now().astimezone()
    if start_time == "—":
        start_time = now.strftime("%Y-%m-%d %H:%M:%S")
    end_time = now.strftime("%Y-%m-%d %H:%M:%S")

    if total_time <= 0 and cases:
        total_time = sum(c.duration_sec for c in cases)

    return SummaryStats(
        session_name=session_name,
        project_name=project_name,
        start_time=start_time,
        end_time=end_time,
        duration_sec=total_time,
        computer=socket.gethostname(),
        user=_local_username(),
        run_count=run_count,
        passed=passed,
        failed=failed,
        warnings=warnings,
        errors=errors,
        skipped=skipped,
        cases=cases,
    )


def _format_duration(seconds: float) -> str:
    if seconds < 60:
        return f"{seconds:.2f} 秒"
    minutes = int(seconds // 60)
    secs = seconds % 60
    return f"{minutes} 分 {secs:.1f} 秒"


def _pct(part: int, total: int) -> str:
    if total <= 0:
        return "0%"
    return f"{(part / total) * 100:.1f}%"


def render_markdown(stats: SummaryStats, prompt_ref: str) -> str:
    lines: list[str] = [
        "# TestComplete Summary Report",
        "",
        f"> 格式依據：[report_prompt.md]({prompt_ref})",
        "",
        "## 1. 執行摘要（Session Overview）",
        "",
        f"| 項目 | 值 |",
        f"|------|-----|",
        f"| **Session 名稱** | {stats.session_name} |",
        f"| **Run test cases** | {stats.run_count} |",
        f"| **Passed** | {stats.passed} ({_pct(stats.passed, stats.run_count)}) |",
        f"| **Failed** | {stats.failed} ({_pct(stats.failed, stats.run_count)}) |",
        f"| **Errors** | {stats.errors} ({_pct(stats.errors, stats.run_count)}) |",
        f"| **Warnings** | {stats.warnings} ({_pct(stats.warnings, stats.run_count)}) |",
        f"| **Skipped** | {stats.skipped} ({_pct(stats.skipped, stats.run_count)}) |",
        f"| **Rerun** | {stats.rerun} |",
        f"| **Unexecuted** | {stats.unexecuted} |",
        f"| **Test run duration** | {_format_duration(stats.duration_sec)} |",
        "",
        "## 2. General",
        "",
        "| 欄位 | 值 |",
        "|------|-----|",
        f"| Start Time | {stats.start_time} |",
        f"| End Time | {stats.end_time} |",
        f"| Computer | {stats.computer} |",
        f"| User | {stats.user} |",
        f"| Platform | {platform.system()} {platform.release()} |",
        "",
        "## 3. Test Case 清單",
        "",
        "| Test Case | Project | Start Time | Duration | 狀態 |",
        "|-----------|---------|------------|----------|------|",
    ]

    for c in stats.cases:
        dur = _format_duration(c.duration_sec)
        lines.append(
            f"| {c.name} | {c.project} | {c.start_time} | {dur} | {c.status} |"
        )

    failures = [c for c in stats.cases if c.status in ("Failed", "Error", "Warning")]
    if failures:
        lines.extend(["", "## 4. 失敗與警告明細", ""])
        for c in failures:
            lines.append(f"### {c.name} ({c.status})")
            lines.append("")
            if c.detail:
                lines.append(f"```\n{c.detail}\n```")
            else:
                lines.append("_（無附加訊息，請查閱詳細 log）_")
            lines.append("")

    lines.extend(
        [
            "---",
            "",
            f"*報告產生時間：{stats.end_time}*",
        ]
    )
    return "\n".join(lines)


def render_html(stats: SummaryStats, prompt_ref: str) -> str:
    def esc(s: str) -> str:
        return html.escape(s, quote=True)

    rows_html = "".join(
        f"<tr class='status-{esc(c.status.lower())}'>"
        f"<td>{esc(c.name)}</td>"
        f"<td>{esc(c.project)}</td>"
        f"<td>{esc(c.start_time)}</td>"
        f"<td>{esc(_format_duration(c.duration_sec))}</td>"
        f"<td>{esc(c.status)}</td></tr>"
        for c in stats.cases
    )

    fail_blocks = ""
    for c in [x for x in stats.cases if x.status in ("Failed", "Error", "Warning")]:
        detail = f"<pre>{esc(c.detail)}</pre>" if c.detail else "<p><em>無附加訊息</em></p>"
        fail_blocks += f"<h3>{esc(c.name)} ({esc(c.status)})</h3>{detail}"

    return f"""<!DOCTYPE html>
<html lang="zh-Hant">
<head>
  <meta charset="utf-8"/>
  <title>Summary Report — {esc(stats.session_name)}</title>
  <style>
    body {{ font-family: Segoe UI, sans-serif; margin: 2rem; color: #222; }}
    h1 {{ color: #1a5276; }}
    table {{ border-collapse: collapse; width: 100%; margin: 1rem 0; }}
    th, td {{ border: 1px solid #ccc; padding: 0.5rem 0.75rem; text-align: left; }}
    th {{ background: #eaf2f8; }}
    .metrics {{ display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 0.75rem; }}
    .metric {{ background: #f4f6f7; padding: 1rem; border-radius: 6px; }}
    .metric strong {{ display: block; font-size: 1.4rem; }}
    tr.status-failed td, tr.status-error td {{ background: #fadbd8; }}
    tr.status-warning td {{ background: #fcf3cf; }}
    tr.status-skipped td {{ background: #ebf5fb; }}
    pre {{ background: #f8f9f9; padding: 0.75rem; overflow-x: auto; }}
  </style>
</head>
<body>
  <h1>TestComplete Summary Report</h1>
  <p>格式依據 <a href="{esc(prompt_ref)}">report_prompt.md</a></p>

  <h2>1. 執行摘要</h2>
  <p><strong>Session：</strong>{esc(stats.session_name)}</p>
  <div class="metrics">
    <div class="metric"><span>Run</span><strong>{stats.run_count}</strong></div>
    <div class="metric"><span>Passed</span><strong>{stats.passed}</strong></div>
    <div class="metric"><span>Failed</span><strong>{stats.failed}</strong></div>
    <div class="metric"><span>Errors</span><strong>{stats.errors}</strong></div>
    <div class="metric"><span>Warnings</span><strong>{stats.warnings}</strong></div>
    <div class="metric"><span>Skipped</span><strong>{stats.skipped}</strong></div>
    <div class="metric"><span>Duration</span><strong>{esc(_format_duration(stats.duration_sec))}</strong></div>
  </div>

  <h2>2. General</h2>
  <table>
    <tr><th>Start Time</th><td>{esc(stats.start_time)}</td></tr>
    <tr><th>End Time</th><td>{esc(stats.end_time)}</td></tr>
    <tr><th>Computer</th><td>{esc(stats.computer)}</td></tr>
    <tr><th>User</th><td>{esc(stats.user)}</td></tr>
    <tr><th>Platform</th><td>{esc(platform.system())} {esc(platform.release())}</td></tr>
  </table>

  <h2>3. Test Case 清單</h2>
  <table>
    <thead><tr><th>Test Case</th><th>Project</th><th>Start Time</th><th>Duration</th><th>狀態</th></tr></thead>
    <tbody>{rows_html}</tbody>
  </table>

  {"<h2>4. 失敗與警告明細</h2>" + fail_blocks if fail_blocks else ""}

  <footer><p><em>報告產生時間：{esc(stats.end_time)}</em></p></footer>
</body>
</html>
"""


def find_junit_file(path: Path) -> Path:
    if path.is_file():
        return path
    if path.is_dir():
        candidates = sorted(
            path.glob("**/*.xml"),
            key=lambda p: p.stat().st_mtime,
            reverse=True,
        )
        for c in candidates:
            try:
                root = ET.parse(c).getroot()
                if root.tag in ("testsuites", "testsuite") or root.find(".//testcase") is not None:
                    return c
            except ET.ParseError:
                continue
    raise FileNotFoundError(f"找不到 JUnit XML：{path}")


def main() -> int:
    parser = argparse.ArgumentParser(
        description="依 report_prompt.md 格式從 JUnit XML 產生 Summary Report"
    )
    parser.add_argument(
        "--junit",
        required=True,
        help="JUnit XML 檔案或含 XML 的目錄",
    )
    parser.add_argument(
        "--output-dir",
        default="reports",
        help="輸出目錄（預設：reports）",
    )
    parser.add_argument(
        "--session",
        default="",
        help="Session 名稱（覆寫 XML 中的名稱）",
    )
    parser.add_argument(
        "--project",
        default="",
        help="預設 Project 名稱（testcase 無 classname 時使用）",
    )
    parser.add_argument(
        "--prompt-ref",
        default="../Project_Testcomplete/report_prompt.md",
        help="report_prompt.md 相對路徑（寫入報告連結）",
    )
    args = parser.parse_args()

    junit_path = find_junit_file(Path(args.junit))
    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    default_project = args.project or "TestProject"
    stats = _collect_cases(junit_path, default_project)

    if args.session:
        stats.session_name = args.session

    md = render_markdown(stats, args.prompt_ref)
    html_out = render_html(stats, args.prompt_ref)

    md_path = output_dir / "summary_report.md"
    html_path = output_dir / "summary_report.html"
    md_path.write_text(md, encoding="utf-8")
    html_path.write_text(html_out, encoding="utf-8")

    print(f"JUnit 來源：{junit_path}")
    print(f"Summary MD：  {md_path.resolve()}")
    print(f"Summary HTML：{html_path.resolve()}")
    print(
        f"結果：Run={stats.run_count} Passed={stats.passed} "
        f"Failed={stats.failed} Errors={stats.errors}"
    )
    return 0 if stats.failed == 0 and stats.errors == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
