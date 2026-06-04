import os
path = r"c:\Users\BaoGo\Documents\ClaudeCode_Project\SQA_AI_Automation\demo2_desktop_app\Test_data\X.xlsx"
try:
    import openpyxl
    wb = openpyxl.load_workbook(path, read_only=True, data_only=True)
    ws = wb.active
    rows = list(ws.iter_rows(values_only=True))
    wb.close()
    print("METHOD: openpyxl")
    print("ROW_COUNT:", len(rows))
    for i, row in enumerate(rows):
        print("ROW_%d:" % i, row)
    pairs = []
    start = 1 if rows and rows[0] else 0
    for row in rows[start:]:
        if row and len(row) >= 2 and row[0] is not None:
            pairs.append((row[0], row[1]))
    print("--- PAIRS ---")
    for t, d in pairs:
        print("%s -> %s" % (t, d))
except ImportError:
    import zipfile, re
    print("METHOD: unzip (no openpyxl)")
    with zipfile.ZipFile(path) as z:
        xml = z.read("xl/worksheets/sheet1.xml").decode("utf-8")
    print("XML_LEN:", len(xml))
    print("XML_SNIPPET_START")
    print(xml[:2500])
    print("XML_SNIPPET_END")
