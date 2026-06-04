using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Demo2Desktop.Excel
{
    public sealed class ExcelFileReader
    {
        public sealed class ExcelLoadResult
        {
            public DataTable Table;
            public string SheetName;
            public string FilePath;
        }

        public static ExcelLoadResult LoadFirstSheet(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                throw new FileNotFoundException("找不到 Excel 檔案。", filePath);

            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext != ".xls" && ext != ".xlsx" && ext != ".xlsm")
                throw new NotSupportedException("僅支援 .xls / .xlsx / .xlsm 格式。");

            string connectionString = BuildConnectionString(filePath, ext);
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                string sheetName = GetFirstSheetName(connection);
                if (string.IsNullOrEmpty(sheetName))
                    throw new InvalidOperationException("Excel 檔案中找不到工作表。");

                string query = "SELECT * FROM [" + sheetName + "]";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                {
                    DataTable table = new DataTable(sheetName);
                    adapter.Fill(table);
                    return new ExcelLoadResult
                    {
                        Table = table,
                        SheetName = sheetName,
                        FilePath = filePath
                    };
                }
            }
        }

        static string BuildConnectionString(string filePath, string ext)
        {
            if (ext == ".xls")
            {
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath
                    + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            }

            return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath
                + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";
        }

        static string GetFirstSheetName(OleDbConnection connection)
        {
            DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (schema == null)
                return null;

            foreach (DataRow row in schema.Rows)
            {
                string tableType = Convert.ToString(row["TABLE_TYPE"]);
                if (tableType != "TABLE")
                    continue;

                string name = Convert.ToString(row["TABLE_NAME"]);
                if (!string.IsNullOrEmpty(name) && name.EndsWith("$"))
                    return name;
            }
            return null;
        }

        public static string GetProviderHint(string ext)
        {
            if (ext == ".xls")
                return "請確認已安裝 Microsoft Jet 4.0（.xls）。";
            return "請安裝 Microsoft Access Database Engine（ACE OLEDB 12.0）以讀取 .xlsx。";
        }
    }
}
