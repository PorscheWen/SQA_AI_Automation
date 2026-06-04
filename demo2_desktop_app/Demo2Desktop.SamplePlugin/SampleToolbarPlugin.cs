using System.Drawing;
using System.Windows.Forms;
using Demo2Desktop;
using Demo2Desktop.Plugin;

namespace Demo2Desktop.SamplePlugin
{
    public sealed class SampleToolbarPlugin : IToolbarPlugin
    {
        public string Name { get { return "Sample"; } }
        public string Tooltip { get { return "範例外掛 DLL (SamplePlugin)"; } }
        public Image Icon { get { return null; } }

        public void Execute(IWin32Window owner)
        {
            MainForm form = owner as MainForm;
            if (form != null)
                form.AppendToolLog("Sample plugin executed.");
            MessageBox.Show(owner, "這是從 Plugins\\Demo2Desktop.SamplePlugin.dll 載入的外掛。", "Sample Plugin");
        }
    }
}
