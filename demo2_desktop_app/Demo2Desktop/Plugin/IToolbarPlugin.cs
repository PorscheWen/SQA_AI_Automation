using System.Drawing;
using System.Windows.Forms;

namespace Demo2Desktop.Plugin
{
    /// <summary>
    /// 工具列外掛介面。實作此介面的類別可放在 Plugins 資料夾的 DLL 中自動載入。
    /// </summary>
    public interface IToolbarPlugin
    {
        string Name { get; }
        string Tooltip { get; }
        Image Icon { get; }
        void Execute(IWin32Window owner);
    }
}
