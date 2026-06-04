using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Demo2Desktop.Excel;
using Demo2Desktop.Plugin;
using Demo2Desktop.UI;

namespace Demo2Desktop
{
    public partial class MainForm : Form
    {
        const string DefaultTestDataPath =
            @"C:\Users\BaoGo\Documents\ClaudeCode_Project\SQA_AI_Automation\demo2_desktop_app\Test_data";
        const int ToolTabDataTable = 0;
        const int ToolTabChart = 1;

        readonly PluginManager pluginManager = new PluginManager();
        readonly List<string> chartTestTypes = new List<string>();
        readonly List<int> chartDefectValues = new List<int>();
        string workspaceRoot;
        string currentFilePath;

        public MainForm()
        {
            InitializeComponent();
            SetupTreeIcons();
            SetupToolbar0();
            SetupToolbar1();
            SetupToolbar2();
            ApplyToolbarAccessibility();
            workspaceRoot = ResolveTestDataRoot();
            lblFileTree.Text = "File Tree (Test_data)";
            excelOpenFileDialog.InitialDirectory = workspaceRoot;
            RefreshFileTree();
            LoadPlugins();
            HideToolTabHeaders();
            AppendToolLog("File Tree 路徑: " + workspaceRoot);
        }

        void HideToolTabHeaders()
        {
            toolTabControl.Appearance = TabAppearance.FlatButtons;
            toolTabControl.SizeMode = TabSizeMode.Fixed;
            toolTabControl.ItemSize = new Size(0, 1);
            toolTabControl.Padding = new Point(0, 0);
        }

        static string ResolveTestDataRoot()
        {
            if (Directory.Exists(DefaultTestDataPath))
                return DefaultTestDataPath;

            string besideExe = Path.Combine(Application.StartupPath, "Test_data");
            if (Directory.Exists(besideExe))
                return Path.GetFullPath(besideExe);

            string relative = Path.GetFullPath(
                Path.Combine(Application.StartupPath, @"..\..\..\Test_data"));
            if (Directory.Exists(relative))
                return relative;

            return DefaultTestDataPath;
        }

        void SetupToolbar0()
        {
            Image importIcon = ToolbarIcons.CreateImportExcelIcon();
            toolbarImageList.Images.Add("importexcel", importIcon);
            btnToolbar0ImportExcel.Image = toolbarImageList.Images["importexcel"];
            btnToolbar0ImportExcel.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;

            Image aboutIcon = ToolbarIcons.CreateAboutIcon();
            toolbarImageList.Images.Add("about", aboutIcon);
            btnToolbar0About.Image = toolbarImageList.Images["about"];
            btnToolbar0About.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        }

        void btnToolbar0About_Click(object sender, EventArgs e)
        {
            ShowAboutDialog();
        }

        void btnToolbar0ImportExcel_Click(object sender, EventArgs e)
        {
            ImportExcelFile();
        }

        void ImportExcelFile()
        {
            if (!Directory.Exists(workspaceRoot))
                Directory.CreateDirectory(workspaceRoot);

            if (Directory.Exists(workspaceRoot))
                excelOpenFileDialog.InitialDirectory = workspaceRoot;

            excelOpenFileDialog.Title = "Import Excel";
            if (excelOpenFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            string sourcePath = excelOpenFileDialog.FileName;
            if (!IsExcelFile(sourcePath))
            {
                MessageBox.Show(this, "請選擇 .xls / .xlsx / .xlsm 檔案。", "Import Excel",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string fileName = Path.GetFileName(sourcePath);
                string destPath = Path.Combine(workspaceRoot, fileName);
                File.Copy(sourcePath, destPath, true);

                OpenExcelFile(destPath);
                RefreshFileTree();
                AppendToolLog("Import Excel: " + sourcePath + " -> " + destPath);
                statusLabel.Text = "已匯入: " + destPath;
            }
            catch (Exception ex)
            {
                AppendToolLog("Import Excel 失敗: " + ex.Message);
                MessageBox.Show(this, ex.Message, "Import Excel",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void SetupToolbar1()
        {
            Image tableIcon = ToolbarIcons.CreateDataTableIcon();
            toolbarImageList.Images.Add("datatable", tableIcon);
            btnToolbar1OpenExcel.Image = toolbarImageList.Images["datatable"];
            btnToolbar1OpenExcel.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        }

        void SwitchToToolTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= toolTabControl.TabCount)
                return;
            toolTabControl.SelectedIndex = tabIndex;
        }

        void SetupToolbar2()
        {
            Image chartIcon = ToolbarIcons.CreateChartIcon();
            toolbarImageList.Images.Add("chart", chartIcon);
            btnToolbar2DrawData.Image = toolbarImageList.Images["chart"];
            btnToolbar2DrawData.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        }

        /// <summary>讓 UI Automation / FlaUI 能以顯示文字找到 ToolStrip 按鈕。</summary>
        void ApplyToolbarAccessibility()
        {
            btnToolbar0ImportExcel.AccessibleName = btnToolbar0ImportExcel.Text;
            btnToolbar0About.AccessibleName = btnToolbar0About.Text;
            btnToolbar1OpenExcel.AccessibleName = btnToolbar1OpenExcel.Text;
            btnToolbar2DrawData.AccessibleName = btnToolbar2DrawData.Text;
            dataGridExcel.AccessibleName = dataGridExcel.Name;
            txtToolLog.AccessibleName = txtToolLog.Name;
            treeFiles.AccessibleName = treeFiles.Name;
        }

        void SetupTreeIcons()
        {
            treeImageList.Images.Clear();
            treeImageList.Images.Add("folder", SystemIcons.WinLogo.ToBitmap());
            treeImageList.Images.Add("file", SystemIcons.Application.ToBitmap());
        }

        public void AppendToolLog(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            txtToolLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + message + Environment.NewLine);
        }

        public Panel PluginHostPanel
        {
            get { return panelPluginHost; }
        }

        void LoadPlugins()
        {
            string pluginsDir = Path.Combine(Application.StartupPath, "Plugins");
            pluginManager.LoadAll(pluginsDir);
            AppendToolLog("外掛已載入（工具列不顯示 Hello/Sample）: " + pluginManager.Plugins.Count + " 個。");
        }

        void RefreshFileTree()
        {
            treeFiles.BeginUpdate();
            treeFiles.Nodes.Clear();
            if (!Directory.Exists(workspaceRoot))
            {
                statusLabel.Text = "找不到 Test_data: " + workspaceRoot;
                treeFiles.EndUpdate();
                return;
            }

            TreeNode root = new TreeNode(Path.GetFileName(workspaceRoot));
            root.Tag = workspaceRoot;
            root.ImageKey = root.SelectedImageKey = "folder";
            BuildDirectoryNodes(root, workspaceRoot);
            treeFiles.Nodes.Add(root);
            root.Expand();
            statusLabel.Text = "File Tree: " + workspaceRoot;
            treeFiles.EndUpdate();
        }

        void BuildDirectoryNodes(TreeNode parent, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                Array.Sort(dirs, StringComparer.OrdinalIgnoreCase);
                foreach (string dir in dirs)
                {
                    string name = Path.GetFileName(dir);
                    if (name.StartsWith(".") || name == "obj" || name == "bin")
                        continue;
                    TreeNode node = new TreeNode(name);
                    node.Tag = dir;
                    node.ImageKey = node.SelectedImageKey = "folder";
                    parent.Nodes.Add(node);
                    BuildDirectoryNodes(node, dir);
                }

                string[] files = Directory.GetFiles(path);
                Array.Sort(files, StringComparer.OrdinalIgnoreCase);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    TreeNode node = new TreeNode(name);
                    node.Tag = file;
                    node.ImageKey = node.SelectedImageKey = "file";
                    parent.Nodes.Add(node);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        void treeFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                return;
            string path = e.Node.Tag as string;
            if (path == null)
                return;
            if (Directory.Exists(path))
                statusLabel.Text = "資料夾: " + path;
            else
                statusLabel.Text = "檔案: " + path;
        }

        void treeFiles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
                return;
            string path = e.Node.Tag as string;
            if (path == null || Directory.Exists(path))
            {
                if (Directory.Exists(path))
                    e.Node.Expand();
                return;
            }
            if (IsExcelFile(path))
                OpenExcelFile(path);
            else
                OpenFileInWorkspace(path);
        }

        static bool IsExcelFile(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext == ".xls" || ext == ".xlsx" || ext == ".xlsm";
        }

        void btnToolbar1OpenExcel_Click(object sender, EventArgs e)
        {
            SwitchToToolTab(ToolTabDataTable);
            AppendToolLog("Data Table: 切換至資料表檢視");
            EnsureDataTableLoaded();
        }

        void EnsureDataTableLoaded()
        {
            if (dataGridExcel.DataSource != null)
                return;

            string xPath = Path.Combine(workspaceRoot, "X.xlsx");
            if (File.Exists(xPath))
            {
                OpenExcelFile(xPath);
                return;
            }

            if (Directory.Exists(workspaceRoot))
                excelOpenFileDialog.InitialDirectory = workspaceRoot;
            if (excelOpenFileDialog.ShowDialog(this) == DialogResult.OK)
                OpenExcelFile(excelOpenFileDialog.FileName);
        }

        void OpenExcelFile(string filePath)
        {
            try
            {
                ExcelFileReader.ExcelLoadResult result = ExcelFileReader.LoadFirstSheet(filePath);
                ShowExcelInToolWindow(result);
            }
            catch (Exception ex)
            {
                string hint = ExcelFileReader.GetProviderHint(Path.GetExtension(filePath).ToLowerInvariant());
                AppendToolLog("Excel 開啟失敗: " + ex.Message);
                MessageBox.Show(
                    this,
                    ex.Message + "\r\n\r\n" + hint,
                    "開啟 Excel",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        void ShowExcelInToolWindow(ExcelFileReader.ExcelLoadResult result)
        {
            SwitchToToolTab(ToolTabDataTable);
            dataGridExcel.DataSource = result.Table;
            lblToolPlugin.Text = "Data Table - " + Path.GetFileName(result.FilePath)
                + " [" + result.SheetName + "]";
            AppendToolLog("已載入 Excel: " + result.FilePath);
            AppendToolLog("工作表: " + result.SheetName + "，"
                + result.Table.Rows.Count + " 列 x " + result.Table.Columns.Count + " 欄");
            statusLabel.Text = "Excel: " + result.FilePath;
        }

        void btnToolbar2DrawData_Click(object sender, EventArgs e)
        {
            SwitchToToolTab(ToolTabChart);
            AppendToolLog("Draw data: 切換至圖表檢視");
            DrawDataChart();
        }

        void DrawDataChart()
        {
            try
            {
                DataTable table = LoadChartDataTable();
                if (table == null)
                {
                    MessageBox.Show(this,
                        "找不到可繪圖的 Excel 資料。請確認 Test_data\\X.xlsx 存在，或先用 Excel 按鈕載入資料。",
                        "Draw data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                dataGridExcel.DataSource = table;
                ExtractChartSeries(table);
                if (chartTestTypes.Count == 0)
                {
                    MessageBox.Show(this,
                        "資料表中找不到 Test Type 與 Defect Number 欄位。",
                        "Draw data",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                SwitchToToolTab(ToolTabChart);
                pictureBoxChart.Invalidate();
                lblToolPlugin.Text = "Chart (" + chartTestTypes.Count + " points)";
                AppendToolLog("Draw data: 已繪製 " + chartTestTypes.Count + " 筆 Test Type / Defect 曲線圖。");
                statusLabel.Text = "Chart: " + chartTestTypes.Count + " test types";
            }
            catch (Exception ex)
            {
                AppendToolLog("Draw data 失敗: " + ex.Message);
                MessageBox.Show(this, ex.Message, "Draw data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        DataTable LoadChartDataTable()
        {
            string xPath = Path.Combine(workspaceRoot, "X.xlsx");
            if (File.Exists(xPath))
            {
                ExcelFileReader.ExcelLoadResult result = ExcelFileReader.LoadFirstSheet(xPath);
                AppendToolLog("Draw data 資料來源: " + xPath);
                return result.Table;
            }

            DataTable bound = dataGridExcel.DataSource as DataTable;
            if (bound != null)
            {
                AppendToolLog("Draw data 資料來源: 目前 DataGrid 資料");
                return bound;
            }

            return null;
        }

        void ExtractChartSeries(DataTable table)
        {
            chartTestTypes.Clear();
            chartDefectValues.Clear();

            int typeCol = -1;
            int defectCol = -1;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string name = table.Columns[i].ColumnName.Trim().ToLowerInvariant().Replace(" ", "");
                if (name == "testtype" || name.Contains("testtype"))
                    typeCol = i;
                if (name == "defectnumber" || name.Contains("defectnumber") || name == "defect")
                    defectCol = i;
            }

            if (typeCol < 0 || defectCol < 0)
                throw new InvalidOperationException("需要 Test Type 與 Defect Number 欄位。");

            foreach (DataRow row in table.Rows)
            {
                if (row.IsNull(typeCol) || row.IsNull(defectCol))
                    continue;

                string testType = Convert.ToString(row[typeCol]).Trim();
                if (string.IsNullOrEmpty(testType))
                    continue;

                int defectNo;
                try
                {
                    defectNo = Convert.ToInt32(row[defectCol]);
                }
                catch
                {
                    continue;
                }

                if (defectNo <= 0)
                    continue;

                chartTestTypes.Add(testType);
                chartDefectValues.Add(defectNo);
            }
        }

        void pictureBoxChart_Paint(object sender, PaintEventArgs e)
        {
            if (chartTestTypes.Count == 0)
            {
                using (Font font = new Font("Microsoft JhengHei UI", 9f))
                using (SolidBrush brush = new SolidBrush(Color.Gray))
                {
                    string hint = "按 Icon2「Draw data」顯示曲線圖";
                    e.Graphics.DrawString(hint, font, brush, 12, 12);
                }
                return;
            }

            DefectChartRenderer.PaintChart(e.Graphics, pictureBoxChart.ClientRectangle,
                chartTestTypes, chartDefectValues);
        }

        void pictureBoxChart_Resize(object sender, EventArgs e)
        {
            pictureBoxChart.Invalidate();
        }

        void OpenFileInWorkspace(string path)
        {
            currentFilePath = path;
            try
            {
                txtFileContent.Text = File.ReadAllText(path);
                AppendToolLog("開啟檔案: " + path);
                AppendToolLog("---");
                AppendToolLog(txtFileContent.Text);
                AppendToolLog("---");
                statusLabel.Text = "已開啟: " + path;
            }
            catch (Exception ex)
            {
                AppendToolLog("無法開啟檔案: " + ex.Message);
            }
        }

        void menuFileNew_Click(object sender, EventArgs e)
        {
            txtFileContent.Clear();
            currentFilePath = null;
            AppendToolLog("新文件");
            statusLabel.Text = "File: 新文件";
        }

        void menuFileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                return;
            OpenFileInWorkspace(openFileDialog.FileName);
        }

        void menuFileSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                    return;
                currentFilePath = saveFileDialog.FileName;
            }
            File.WriteAllText(currentFilePath, txtFileContent.Text);
            AppendToolLog("已儲存: " + currentFilePath);
            statusLabel.Text = "File: 已儲存 " + currentFilePath;
            RefreshFileTree();
        }

        void menuFileRefreshTree_Click(object sender, EventArgs e)
        {
            RefreshFileTree();
            AppendToolLog("檔案樹已重新整理。");
        }

        void menuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        void menuToolsReloadPlugins_Click(object sender, EventArgs e)
        {
            LoadPlugins();
        }

        void menuToolsAbout_Click(object sender, EventArgs e)
        {
            ShowAboutDialog();
        }

        void ShowAboutDialog()
        {
            MessageBox.Show(
                this,
                "Demo2 Desktop App\r\n\r\nFile Tree: " + workspaceRoot
                + "\r\n\r\nToolbar0: Import Excel | About"
                + "\r\nToolbar1: Data Table | Draw data",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
