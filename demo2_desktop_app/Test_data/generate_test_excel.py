# -*- coding: utf-8 -*-
"""Generate 20 Excel test files: test type <-> defect number."""
import os
import random
import zipfile
import xml.etree.ElementTree as ET
from xml.sax.saxutils import escape

TEST_TYPES = [
    ("01", "Functional", 1001),
    ("02", "Regression", 1002),
    ("03", "Smoke", 1003),
    ("04", "Integration", 1004),
    ("05", "UI_UX", 1005),
    ("06", "API", 1006),
    ("07", "Performance", 1007),
    ("08", "Security", 1008),
    ("09", "Compatibility", 1009),
    ("10", "Usability", 1010),
    ("11", "Accessibility", 1011),
    ("12", "Localization", 1012),
    ("13", "Database", 1013),
    ("14", "Network", 1014),
    ("15", "Memory_Leak", 1015),
    ("16", "Boundary", 1016),
    ("17", "Negative", 1017),
    ("18", "Stress", 1018),
    ("19", "Recovery", 1019),
    ("20", "Acceptance", 1020),
]

NS = "http://schemas.openxmlformats.org/spreadsheetml/2006/main"
REL_NS = "http://schemas.openxmlformats.org/package/2006/relationships"
CT_NS = "http://schemas.openxmlformats.org/package/2006/content-types"


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
                lines.append(
                    '<c r="%s"><v>%s</v></c>' % (ref, val)
                )
            else:
                text = escape(str(val))
                lines.append(
                    '<c r="%s" t="inlineStr"><is><t>%s</t></is></c>'
                    % (ref, text)
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
  <sheets><sheet name="TestData" sheetId="1" r:id="rId1"/></sheets>
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


def rows_for_test(seq, test_type, defect_no):
    return [
        ["Test ID", "Test Type", "Defect Number", "Status", "Severity", "Description"],
        [
            "T-%s" % seq,
            test_type,
            defect_no,
            "Open",
            "Medium",
            "Demo defect for %s test (Defect #%d)" % (test_type, defect_no),
        ],
        ["", "", "", "", "", ""],
        ["Metric", "Value", "", "", "", ""],
        ["Defect Count", 1, "", "", "", ""],
        ["Test Case Count", 5, "", "", "", ""],
        ["Pass Rate", 0.85, "", "", "", ""],
    ]


def rows_for_index():
    header = ["No", "Test Type", "Defect Number", "File Name"]
    rows = [header]
    for seq, test_type, defect_no in TEST_TYPES:
        fname = "Test_%s_%s_Defect_%d.xlsx" % (seq, test_type, defect_no)
        rows.append([int(seq), test_type, defect_no, fname])
    return rows


def rows_for_master_all():
    """Single workbook: all 20 test types mapped to 20 defect numbers."""
    rows = [
        [
            "No",
            "Test Type",
            "Defect Number",
            "Test ID",
            "Status",
            "Severity",
            "Description",
        ]
    ]
    for seq, test_type, defect_no in TEST_TYPES:
        rows.append(
            [
                int(seq),
                test_type,
                defect_no,
                "T-%s" % seq,
                "Open",
                "Medium",
                "Defect #%d for %s test type" % (defect_no, test_type),
            ]
        )
    return rows


def rows_for_x_random(seed=42):
    """File X: 20 test types with random positive integer defect numbers."""
    rng = random.Random(seed)
    used = set()
    defects = []
    while len(defects) < 20:
        n = rng.randint(1, 99999)
        if n not in used:
            used.add(n)
            defects.append(n)

    rows = [["No", "Test Type", "Defect Number"]]
    for i, (seq, test_type, _) in enumerate(TEST_TYPES):
        rows.append([int(seq), test_type, defects[i]])
    return rows


def main():
    base = os.path.dirname(os.path.abspath(__file__))
    os.makedirs(base, exist_ok=True)

    for seq, test_type, defect_no in TEST_TYPES:
        fname = "Test_%s_%s_Defect_%d.xlsx" % (seq, test_type, defect_no)
        path = os.path.join(base, fname)
        build_xlsx(path, rows_for_test(seq, test_type, defect_no))
        print("Created:", fname)

    index_path = os.path.join(base, "Test_Index_All_20_Types.xlsx")
    build_xlsx(index_path, rows_for_index())
    print("Created:", "Test_Index_All_20_Types.xlsx")

    master_path = os.path.join(base, "Test_All_20_Types_Defects.xlsx")
    build_xlsx(master_path, rows_for_master_all())
    print("Created:", "Test_All_20_Types_Defects.xlsx")

    x_path = os.path.join(base, "X.xlsx")
    build_xlsx(x_path, rows_for_x_random())
    print("Created:", "X.xlsx (20 test types, random defect integers)")
    print("Done: 23 xlsx files in", base)


if __name__ == "__main__":
    main()
