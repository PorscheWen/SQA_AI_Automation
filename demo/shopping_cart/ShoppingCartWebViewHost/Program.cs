using Microsoft.Web.WebView2.WinForms;

namespace ShoppingCartWebViewHost;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var targetUrl = args.FirstOrDefault() ?? "http://localhost:8888/";
        Application.Run(new MainForm(targetUrl));
    }
}

internal sealed class MainForm : Form
{
    private readonly WebView2 _webView = new()
    {
        Dock = DockStyle.Fill
    };

    public MainForm(string targetUrl)
    {
        Text = "簡易購物車 - Demo Shop";
        Width = 1280;
        Height = 900;
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(_webView);
        Load += async (_, _) => await InitializeWebViewAsync(targetUrl);
    }

    private async Task InitializeWebViewAsync(string targetUrl)
    {
        await _webView.EnsureCoreWebView2Async();
        _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
        _webView.CoreWebView2.Navigate(targetUrl);
    }
}
