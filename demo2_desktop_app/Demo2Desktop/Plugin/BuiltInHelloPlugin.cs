using System.Drawing;
using System.Windows.Forms;

namespace Demo2Desktop.Plugin
{
    public sealed class BuiltInHelloPlugin : IToolbarPlugin
    {
        public string Name { get { return "Hello"; } }
        public string Tooltip { get { return "內建示例外掛"; } }
        public Image Icon { get { return null; } }

        public void Execute(IWin32Window owner)
        {
            MainForm form = owner as MainForm;
            if (form != null)
                form.AppendToolLog("Hello from built-in plugin!");
            MessageBox.Show(owner, "Hello from built-in plugin!", "Demo2 Desktop");
        }
    }
}
