namespace Demo2Desktop
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileNew;
        private System.Windows.Forms.ToolStripMenuItem menuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuFileCloseExcel;
        private System.Windows.Forms.ToolStripMenuItem menuFileRefreshTree;
        private System.Windows.Forms.ToolStripSeparator menuFileSep1;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuToolsImportExcel;
        private System.Windows.Forms.ToolStripMenuItem menuToolsOpenExcel;
        private System.Windows.Forms.ToolStripMenuItem menuToolsDrawData;
        private System.Windows.Forms.ToolStripMenuItem menuToolsReloadPlugins;
        private System.Windows.Forms.ToolStripMenuItem menuToolsAbout;
        private System.Windows.Forms.ToolStrip toolBarStrip0;
        private System.Windows.Forms.ToolStripButton btnToolbar0ImportExcel;
        private System.Windows.Forms.ToolStripSeparator toolBarSeparator0;
        private System.Windows.Forms.ToolStripButton btnToolbar0About;
        private System.Windows.Forms.ToolStrip toolBarStrip1;
        private System.Windows.Forms.ToolStripButton btnToolbar1OpenExcel;
        private System.Windows.Forms.ToolStripSeparator toolBarSeparatorIcons;
        private System.Windows.Forms.ToolStripButton btnToolbar2DrawData;
        private System.Windows.Forms.ImageList toolbarImageList;
        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.Panel panelFileTree;
        private System.Windows.Forms.Label lblFileTree;
        private System.Windows.Forms.TreeView treeFiles;
        private System.Windows.Forms.Panel panelToolPlugin;
        private System.Windows.Forms.Label lblToolPlugin;
        private System.Windows.Forms.SplitContainer splitToolWorkspace;
        private System.Windows.Forms.TabControl toolTabControl;
        private System.Windows.Forms.TabPage tabPageDataTable;
        private System.Windows.Forms.TabPage tabPageChart;
        private System.Windows.Forms.DataGridView dataGridExcel;
        private System.Windows.Forms.PictureBox pictureBoxChart;
        private System.Windows.Forms.TextBox txtToolLog;
        private System.Windows.Forms.Panel panelPluginHost;
        private System.Windows.Forms.TextBox txtFileContent;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.OpenFileDialog excelOpenFileDialog;
        private System.Windows.Forms.OpenFileDialog jsonImportFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ImageList treeImageList;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileCloseExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileRefreshTree = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsImportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsOpenExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsDrawData = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsReloadPlugins = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolsAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarStrip0 = new System.Windows.Forms.ToolStrip();
            this.btnToolbar0ImportExcel = new System.Windows.Forms.ToolStripButton();
            this.toolBarSeparator0 = new System.Windows.Forms.ToolStripSeparator();
            this.btnToolbar0About = new System.Windows.Forms.ToolStripButton();
            this.toolBarStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnToolbar1OpenExcel = new System.Windows.Forms.ToolStripButton();
            this.toolBarSeparatorIcons = new System.Windows.Forms.ToolStripSeparator();
            this.btnToolbar2DrawData = new System.Windows.Forms.ToolStripButton();
            this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.panelFileTree = new System.Windows.Forms.Panel();
            this.lblFileTree = new System.Windows.Forms.Label();
            this.treeFiles = new System.Windows.Forms.TreeView();
            this.panelToolPlugin = new System.Windows.Forms.Panel();
            this.lblToolPlugin = new System.Windows.Forms.Label();
            this.splitToolWorkspace = new System.Windows.Forms.SplitContainer();
            this.toolTabControl = new System.Windows.Forms.TabControl();
            this.tabPageDataTable = new System.Windows.Forms.TabPage();
            this.tabPageChart = new System.Windows.Forms.TabPage();
            this.dataGridExcel = new System.Windows.Forms.DataGridView();
            this.pictureBoxChart = new System.Windows.Forms.PictureBox();
            this.txtToolLog = new System.Windows.Forms.TextBox();
            this.panelPluginHost = new System.Windows.Forms.Panel();
            this.txtFileContent = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.excelOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.jsonImportFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.mainMenuStrip.SuspendLayout();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            this.panelFileTree.SuspendLayout();
            this.panelToolPlugin.SuspendLayout();
            this.splitToolWorkspace.Panel1.SuspendLayout();
            this.splitToolWorkspace.Panel2.SuspendLayout();
            this.splitToolWorkspace.SuspendLayout();
            this.toolTabControl.SuspendLayout();
            this.tabPageDataTable.SuspendLayout();
            this.tabPageChart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridExcel)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // mainMenuStrip
            //
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFile,
                this.menuTools});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(900, 24);
            this.mainMenuStrip.TabIndex = 0;
            //
            // menuFile
            //
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuFileNew,
                this.menuFileOpen,
                this.menuFileSave,
                this.menuFileCloseExcel,
                this.menuFileRefreshTree,
                this.menuFileSep1,
                this.menuFileExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(35, 20);
            this.menuFile.Text = "File";
            //
            // menuFileNew
            //
            this.menuFileNew.Name = "menuFileNew";
            this.menuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuFileNew.Size = new System.Drawing.Size(180, 22);
            this.menuFileNew.Text = "New";
            this.menuFileNew.Click += new System.EventHandler(this.menuFileNew_Click);
            //
            // menuFileOpen
            //
            this.menuFileOpen.Name = "menuFileOpen";
            this.menuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuFileOpen.Size = new System.Drawing.Size(180, 22);
            this.menuFileOpen.Text = "Open...";
            this.menuFileOpen.Click += new System.EventHandler(this.menuFileOpen_Click);
            //
            // menuFileSave
            //
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuFileSave.Size = new System.Drawing.Size(180, 22);
            this.menuFileSave.Text = "Save...";
            this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
            //
            // menuFileCloseExcel
            //
            this.menuFileCloseExcel.Name = "menuFileCloseExcel";
            this.menuFileCloseExcel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.menuFileCloseExcel.Size = new System.Drawing.Size(220, 22);
            this.menuFileCloseExcel.Text = "Close Excel File";
            this.menuFileCloseExcel.Click += new System.EventHandler(this.menuFileCloseExcel_Click);
            //
            // menuFileRefreshTree
            //
            this.menuFileRefreshTree.Name = "menuFileRefreshTree";
            this.menuFileRefreshTree.Size = new System.Drawing.Size(180, 22);
            this.menuFileRefreshTree.Text = "Refresh Tree";
            this.menuFileRefreshTree.Click += new System.EventHandler(this.menuFileRefreshTree_Click);
            //
            // menuFileExit
            //
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new System.Drawing.Size(180, 22);
            this.menuFileExit.Text = "Exit";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            //
            // menuTools
            //
            this.menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.menuToolsImportExcel,
                this.menuToolsOpenExcel,
                this.menuToolsDrawData,
                this.menuToolsReloadPlugins,
                this.menuToolsAbout});
            this.menuTools.Name = "menuTools";
            this.menuTools.Size = new System.Drawing.Size(44, 20);
            this.menuTools.Text = "Tools";
            //
            // menuToolsImportExcel
            //
            this.menuToolsImportExcel.Name = "menuToolsImportExcel";
            this.menuToolsImportExcel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.menuToolsImportExcel.Size = new System.Drawing.Size(220, 22);
            this.menuToolsImportExcel.Text = "Import JSON";
            this.menuToolsImportExcel.Click += new System.EventHandler(this.btnToolbar0ImportExcel_Click);
            //
            // menuToolsOpenExcel
            //
            this.menuToolsOpenExcel.Name = "menuToolsOpenExcel";
            this.menuToolsOpenExcel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.menuToolsOpenExcel.Size = new System.Drawing.Size(220, 22);
            this.menuToolsOpenExcel.Text = "Data Table";
            this.menuToolsOpenExcel.Click += new System.EventHandler(this.btnToolbar1OpenExcel_Click);
            //
            // menuToolsDrawData
            //
            this.menuToolsDrawData.Name = "menuToolsDrawData";
            this.menuToolsDrawData.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.menuToolsDrawData.Size = new System.Drawing.Size(220, 22);
            this.menuToolsDrawData.Text = "Chart";
            this.menuToolsDrawData.Click += new System.EventHandler(this.btnToolbar2DrawData_Click);
            //
            // menuToolsReloadPlugins
            //
            this.menuToolsReloadPlugins.Name = "menuToolsReloadPlugins";
            this.menuToolsReloadPlugins.Size = new System.Drawing.Size(180, 22);
            this.menuToolsReloadPlugins.Text = "Reload Plugins";
            this.menuToolsReloadPlugins.Click += new System.EventHandler(this.menuToolsReloadPlugins_Click);
            //
            // menuToolsAbout
            //
            this.menuToolsAbout.Name = "menuToolsAbout";
            this.menuToolsAbout.Size = new System.Drawing.Size(180, 22);
            this.menuToolsAbout.Text = "About...";
            this.menuToolsAbout.Click += new System.EventHandler(this.menuToolsAbout_Click);
            //
            // toolBarStrip0
            //
            this.toolBarStrip0.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnToolbar0ImportExcel,
                this.toolBarSeparator0,
                this.btnToolbar0About});
            this.toolBarStrip0.Location = new System.Drawing.Point(0, 24);
            this.toolBarStrip0.Name = "toolBarStrip0";
            this.toolBarStrip0.Size = new System.Drawing.Size(900, 25);
            this.toolBarStrip0.TabIndex = 6;
            this.toolBarStrip0.Text = "Toolbar 0";
            //
            // btnToolbar0ImportExcel
            //
            this.btnToolbar0ImportExcel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnToolbar0ImportExcel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnToolbar0ImportExcel.Name = "btnToolbar0ImportExcel";
            this.btnToolbar0ImportExcel.Size = new System.Drawing.Size(92, 22);
            this.btnToolbar0ImportExcel.Text = "Import JSON";
            this.btnToolbar0ImportExcel.ToolTipText = "匯入 JSON 至 Test_data 並載入資料表";
            this.btnToolbar0ImportExcel.Click += new System.EventHandler(this.btnToolbar0ImportExcel_Click);
            //
            // toolBarSeparator0
            //
            this.toolBarSeparator0.Name = "toolBarSeparator0";
            //
            // btnToolbar0About
            //
            this.btnToolbar0About.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnToolbar0About.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnToolbar0About.Name = "btnToolbar0About";
            this.btnToolbar0About.Size = new System.Drawing.Size(52, 22);
            this.btnToolbar0About.Text = "About";
            this.btnToolbar0About.ToolTipText = "關於 Demo2 Desktop";
            this.btnToolbar0About.Click += new System.EventHandler(this.btnToolbar0About_Click);
            //
            // toolBarStrip1
            //
            this.toolBarStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnToolbar1OpenExcel,
                this.toolBarSeparatorIcons,
                this.btnToolbar2DrawData});
            this.toolBarStrip1.Location = new System.Drawing.Point(0, 49);
            this.toolBarStrip1.Name = "toolBarStrip1";
            this.toolBarStrip1.Size = new System.Drawing.Size(900, 25);
            this.toolBarStrip1.TabIndex = 4;
            this.toolBarStrip1.Text = "Tools";
            //
            // btnToolbar1OpenExcel
            //
            this.btnToolbar1OpenExcel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnToolbar1OpenExcel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnToolbar1OpenExcel.Name = "btnToolbar1OpenExcel";
            this.btnToolbar1OpenExcel.Size = new System.Drawing.Size(78, 22);
            this.btnToolbar1OpenExcel.Text = "Data Table";
            this.btnToolbar1OpenExcel.ToolTipText = "切換至資料表檢視";
            this.btnToolbar1OpenExcel.Click += new System.EventHandler(this.btnToolbar1OpenExcel_Click);
            //
            // toolBarSeparatorIcons
            //
            this.toolBarSeparatorIcons.Name = "toolBarSeparatorIcons";
            //
            // btnToolbar2DrawData
            //
            this.btnToolbar2DrawData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnToolbar2DrawData.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnToolbar2DrawData.Name = "btnToolbar2DrawData";
            this.btnToolbar2DrawData.Size = new System.Drawing.Size(78, 22);
            this.btnToolbar2DrawData.Text = "Draw data";
            this.btnToolbar2DrawData.ToolTipText = "切換至曲線圖（X=Test Type, Y=Defect）";
            this.btnToolbar2DrawData.Click += new System.EventHandler(this.btnToolbar2DrawData_Click);
            //
            // toolbarImageList
            //
            this.toolbarImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.toolbarImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.toolbarImageList.TransparentColor = System.Drawing.Color.Transparent;
            //
            // mainToolStrip (plugin bar hidden — Hello/Sample removed)
            //
            this.mainToolStrip.Location = new System.Drawing.Point(0, 74);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(900, 25);
            this.mainToolStrip.TabIndex = 1;
            this.mainToolStrip.Visible = false;
            //
            // mainSplitContainer
            //
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 74);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Panel1.Controls.Add(this.panelFileTree);
            this.mainSplitContainer.Panel2.Controls.Add(this.panelToolPlugin);
            this.mainSplitContainer.Size = new System.Drawing.Size(900, 465);
            this.mainSplitContainer.SplitterDistance = 260;
            this.mainSplitContainer.TabIndex = 2;
            //
            // panelFileTree
            //
            this.panelFileTree.Controls.Add(this.treeFiles);
            this.panelFileTree.Controls.Add(this.lblFileTree);
            this.panelFileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFileTree.Name = "panelFileTree";
            this.panelFileTree.Padding = new System.Windows.Forms.Padding(4);
            //
            // lblFileTree
            //
            this.lblFileTree.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFileTree.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFileTree.Name = "lblFileTree";
            this.lblFileTree.Size = new System.Drawing.Size(252, 20);
            this.lblFileTree.TabIndex = 0;
            this.lblFileTree.Text = "File Tree";
            //
            // treeFiles
            //
            this.treeFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeFiles.HideSelection = false;
            this.treeFiles.ImageList = this.treeImageList;
            this.treeFiles.Location = new System.Drawing.Point(4, 24);
            this.treeFiles.Name = "treeFiles";
            this.treeFiles.Size = new System.Drawing.Size(252, 462);
            this.treeFiles.TabIndex = 1;
            this.treeFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeFiles_AfterSelect);
            this.treeFiles.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeFiles_NodeMouseDoubleClick);
            //
            // panelToolPlugin
            //
            this.panelToolPlugin.Controls.Add(this.splitToolWorkspace);
            this.panelToolPlugin.Controls.Add(this.panelPluginHost);
            this.panelToolPlugin.Controls.Add(this.lblToolPlugin);
            this.panelToolPlugin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelToolPlugin.Name = "panelToolPlugin";
            this.panelToolPlugin.Padding = new System.Windows.Forms.Padding(4);
            //
            // lblToolPlugin
            //
            this.lblToolPlugin.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblToolPlugin.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblToolPlugin.Name = "lblToolPlugin";
            this.lblToolPlugin.Size = new System.Drawing.Size(628, 20);
            this.lblToolPlugin.TabIndex = 0;
            this.lblToolPlugin.Text = "Tool Plugin Workspace";
            //
            // splitToolWorkspace
            //
            this.splitToolWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitToolWorkspace.Name = "splitToolWorkspace";
            this.splitToolWorkspace.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitToolWorkspace.Panel1.Controls.Add(this.toolTabControl);
            this.splitToolWorkspace.Panel2.Controls.Add(this.txtToolLog);
            this.splitToolWorkspace.Size = new System.Drawing.Size(628, 412);
            this.splitToolWorkspace.SplitterDistance = 280;
            this.splitToolWorkspace.TabIndex = 3;
            //
            // toolTabControl
            //
            this.toolTabControl.Controls.Add(this.tabPageDataTable);
            this.toolTabControl.Controls.Add(this.tabPageChart);
            this.toolTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.toolTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolTabControl.ItemSize = new System.Drawing.Size(0, 1);
            this.toolTabControl.Name = "toolTabControl";
            this.toolTabControl.SelectedIndex = 0;
            this.toolTabControl.Size = new System.Drawing.Size(628, 276);
            this.toolTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.toolTabControl.TabIndex = 4;
            //
            // tabPageDataTable
            //
            this.tabPageDataTable.Controls.Add(this.dataGridExcel);
            this.tabPageDataTable.Location = new System.Drawing.Point(0, 0);
            this.tabPageDataTable.Name = "tabPageDataTable";
            this.tabPageDataTable.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDataTable.Size = new System.Drawing.Size(628, 275);
            this.tabPageDataTable.TabIndex = 0;
            this.tabPageDataTable.Text = "";
            this.tabPageDataTable.UseVisualStyleBackColor = true;
            //
            // tabPageChart
            //
            this.tabPageChart.Controls.Add(this.pictureBoxChart);
            this.tabPageChart.Location = new System.Drawing.Point(0, 0);
            this.tabPageChart.Name = "tabPageChart";
            this.tabPageChart.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageChart.Size = new System.Drawing.Size(628, 275);
            this.tabPageChart.TabIndex = 1;
            this.tabPageChart.Text = "";
            this.tabPageChart.UseVisualStyleBackColor = true;
            //
            // pictureBoxChart
            //
            this.pictureBoxChart.BackColor = System.Drawing.Color.White;
            this.pictureBoxChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxChart.Name = "pictureBoxChart";
            this.pictureBoxChart.TabIndex = 0;
            this.pictureBoxChart.TabStop = false;
            this.pictureBoxChart.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxChart_Paint);
            this.pictureBoxChart.Resize += new System.EventHandler(this.pictureBoxChart_Resize);
            //
            // dataGridExcel
            //
            this.dataGridExcel.AllowUserToAddRows = false;
            this.dataGridExcel.AllowUserToDeleteRows = false;
            this.dataGridExcel.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridExcel.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridExcel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridExcel.Name = "dataGridExcel";
            this.dataGridExcel.ReadOnly = true;
            this.dataGridExcel.RowHeadersVisible = true;
            this.dataGridExcel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridExcel.TabIndex = 0;
            //
            // panelPluginHost
            //
            this.panelPluginHost.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelPluginHost.Location = new System.Drawing.Point(4, 400);
            this.panelPluginHost.Name = "panelPluginHost";
            this.panelPluginHost.Size = new System.Drawing.Size(628, 86);
            this.panelPluginHost.TabIndex = 2;
            this.panelPluginHost.Visible = false;
            //
            // txtToolLog
            //
            this.txtToolLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtToolLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtToolLog.Multiline = true;
            this.txtToolLog.ReadOnly = true;
            this.txtToolLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtToolLog.Name = "txtToolLog";
            this.txtToolLog.TabIndex = 1;
            this.txtToolLog.WordWrap = false;
            this.txtToolLog.Text = "[Tool Plugin] 點選工具列外掛按鈕在此工作區執行。\r\n";
            //
            // txtFileContent (hidden buffer for File menu open/save)
            //
            this.txtFileContent.Name = "txtFileContent";
            this.txtFileContent.Visible = false;
            this.txtFileContent.TabStop = false;
            //
            // treeImageList
            //
            this.treeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.treeImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 539);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(900, 22);
            this.statusStrip.TabIndex = 3;
            //
            // statusLabel
            //
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(885, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // openFileDialog / saveFileDialog
            //
            this.openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            this.excelOpenFileDialog.Filter = "Excel files (*.xlsx;*.xls;*.xlsm)|*.xlsx;*.xls;*.xlsm|All files (*.*)|*.*";
            this.excelOpenFileDialog.Title = "開啟 Excel 檔案";
            this.jsonImportFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            this.jsonImportFileDialog.Title = "匯入 JSON 檔案";
            this.saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 561);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.txtFileContent);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolBarStrip1);
            this.Controls.Add(this.toolBarStrip0);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo2 Desktop App";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.ResumeLayout(false);
            this.panelFileTree.ResumeLayout(false);
            this.panelToolPlugin.ResumeLayout(false);
            this.splitToolWorkspace.Panel1.ResumeLayout(false);
            this.splitToolWorkspace.Panel2.ResumeLayout(false);
            this.splitToolWorkspace.ResumeLayout(false);
            this.toolTabControl.ResumeLayout(false);
            this.tabPageDataTable.ResumeLayout(false);
            this.tabPageChart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridExcel)).EndInit();
            this.panelToolPlugin.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
