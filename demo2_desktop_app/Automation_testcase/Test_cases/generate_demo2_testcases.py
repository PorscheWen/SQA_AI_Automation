# -*- coding: utf-8 -*-
"""Generate Demo2 Desktop App - 10 test cases (6 functional, 3 negative, 1 compatibility)."""
import os
import zipfile
from xml.sax.saxutils import escape

NS = "http://schemas.openxmlformats.org/spreadsheetml/2006/main"
REL_NS = "http://schemas.openxmlformats.org/package/2006/relationships"
CT_NS = "http://schemas.openxmlformats.org/package/2006/content-types"

TEST_CASES = [
    ("TC01", "Functional", "Import JSON", "Import JSON 至 Test_data",
     "Demo2Desktop.exe 已啟動；Test_data 資料夾可寫入",
     "1. 點 Toolbar0「Import JSON」\n2. 選擇有效 .json\n3. 確認 Tab1 資料表",
     "檔案複製到 Test_data；DataGrid 顯示 JSON 資料；檔案樹已更新", "High", 2001, "Ready"),
    ("TC02", "Functional", "File Tree", "File Tree 顯示 Test_data",
     "應用程式已啟動",
     "1. 檢視左側 File Tree\n2. 展開 Test_data 根節點",
     "顯示 Test_data 路徑下檔案與子資料夾（含 .xlsx）", "High", 2002, "Ready"),
    ("TC03", "Functional", "Data Table", "Icon1 切換 Data Table",
     "Test_data 內有 X.xlsx",
     "1. 點 Toolbar1「Data Table」\n2. 確認工作區為資料表",
     "切換至 Tab1；自動載入 X.xlsx（若尚無資料）；DataGrid 有欄位列", "High", 2003, "Ready"),
    ("TC04", "Functional", "Chart", "Icon2 繪製曲線圖",
     "已載入含 Test Type、Defect Number 的 Excel",
     "1. 點 Toolbar1「Draw data」\n2. 檢視圖表區",
     "切換至 Tab2；顯示 X=Test Type、Y=Defect 曲線圖與資料點", "High", 2004, "Ready"),
    ("TC05", "Functional", "File Tree Open", "檔案樹雙擊開啟 Excel",
     "Test_data 有 Demo2_10_TestCases.xlsx 或 X.xlsx",
     "1. 在 File Tree 雙擊 .xlsx\n2. 檢視 Tab1",
     "Tab1 顯示該檔第一個工作表內容；狀態列顯示檔案路徑", "Medium", 2005, "Ready"),
    ("TC06", "Functional", "About", "About 對話框",
     "應用程式已啟動",
     "1. 點 Toolbar0「About」",
     "顯示 About 訊息（含 File Tree 路徑與工具列說明）", "Low", 2006, "Ready"),
    ("TC07", "Negative", "Invalid Import", "匯入非 Excel 檔",
     "應用程式已啟動",
     "1. 點 Import JSON\n2. 選擇 .txt 或非 json",
     "顯示警告「請選擇 .xls / .xlsx / .xlsm」；不寫入 Test_data", "High", 2007, "Ready"),
    ("TC08", "Negative", "Chart No Data", "無資料時繪圖",
     "尚未載入任何 Excel 至 DataGrid",
     "1. 確認 DataGrid 無資料\n2. 點 Draw data",
     "提示找不到可繪圖資料或 Test_data\\X.xlsx；不當機", "Medium", 2008, "Ready"),
    ("TC09", "Negative", "Missing File", "開啟不存在檔案",
     "準備不存在的路徑或已刪除的 xlsx",
     "1. 以程式嘗試載入不存在檔案（或 OLEDB 失敗情境）",
     "顯示錯誤訊息與 OLEDB 提示；應用程式仍可繼續操作", "Medium", 2009, "Ready"),
    ("TC10", "Compatibility", "Excel Format", "xls 與 xlsx 相容",
     "已安裝 Jet（.xls）與 ACE 12.0（.xlsx）；Test_data 有兩種格式樣本",
     "1. 開啟 .xlsx（如 X.xlsx）\n2. 若有 .xls 樣本則開啟 .xls\n3. 比對 DataGrid",
     "兩種格式皆可讀取第一個工作表；欄位顯示正常", "High", 2010, "Ready"),
]


def col_letter(n):
    s = ""
    while n:
        n, r = divmod(n - 1, 26)
        s = chr(65 + r) + s
    return s


def sheet_xml(rows):
    lines = [
        '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>',
        '<worksheet xmlns="%s">' % NS,
        "<sheetData>",
    ]
    for r_idx, row in enumerate(rows, 1):
        lines.append('<row r="%d">' % r_idx)
        for c_idx, val in enumerate(row, 1):
            ref = "%s%d" % (col_letter(c_idx), r_idx)
            if isinstance(val, (int, float)) and not isinstance(val, bool):
                lines.append('<c r="%s"><v>%s</v></c>' % (ref, val))
            else:
                text = escape(str(val))
                lines.append(
                    '<c r="%s" t="inlineStr"><is><t>%s</t></is></c>' % (ref, text)
                )
        lines.append("</row>")
    lines.append("</sheetData></worksheet>")
    return "\n".join(lines)


def build_xlsx(path, rows):
    sheet = sheet_xml(rows)
    content_types = """<?xml version="1.0" encoding="UTF-8"?>
<Types xmlns="%s">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>
  <Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>
  <Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>
</Types>""" % CT_NS
    rels_root = """<?xml version="1.0" encoding="UTF-8"?>
<Relationships xmlns="%s">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>
</Relationships>""" % REL_NS
    workbook = """<?xml version="1.0" encoding="UTF-8"?>
<workbook xmlns="%s" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships">
  <sheets><sheet name="TestCases" sheetId="1" r:id="rId1"/></sheets>
</workbook>""" % NS
    wb_rels = """<?xml version="1.0" encoding="UTF-8"?>
<Relationships xmlns="%s">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/>
  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
</Relationships>""" % REL_NS
    styles = """<?xml version="1.0" encoding="UTF-8"?>
<styleSheet xmlns="%s"><fonts count="1"/><fills count="1"/><borders count="1"/><cellStyleXfs count="1"/><cellXfs count="1"/></styleSheet>""" % NS
    with zipfile.ZipFile(path, "w", zipfile.ZIP_DEFLATED) as zf:
        zf.writestr("[Content_Types].xml", content_types)
        zf.writestr("_rels/.rels", rels_root)
        zf.writestr("xl/workbook.xml", workbook)
        zf.writestr("xl/_rels/workbook.xml.rels", wb_rels)
        zf.writestr("xl/worksheets/sheet1.xml", sheet)
        zf.writestr("xl/styles.xml", styles)


def main():
    header = [
        "Test ID", "Category", "Test Type", "Title", "Preconditions",
        "Steps", "Expected Result", "Priority", "Defect Number", "Status",
    ]
    rows = [header]
    for tc in TEST_CASES:
        rows.append(list(tc))

    base = os.path.dirname(os.path.abspath(__file__))
    test_data = os.path.join(base, "..", "Test_data")
    os.makedirs(test_data, exist_ok=True)

    out_cases = os.path.join(base, "Demo2_10_TestCases.xlsx")
    out_data = os.path.join(test_data, "Demo2_10_TestCases.xlsx")
    build_xlsx(out_cases, rows)
    build_xlsx(out_data, rows)
    print("Created:", out_cases)
    print("Created:", out_data)


if __name__ == "__main__":
    main()
