namespace StarAirDesktop;

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.IO.Compression;
using System.Reflection;

public partial class Form1 : Form
{
    private readonly WebView2 _webView;

    public Form1()
    {
        InitializeComponent();
        _webView = new WebView2 { Dock = DockStyle.Fill };
        Controls.Add(_webView);
        Shown += async (_, _) => await LoadOfflineAppAsync();
    }

    private async Task LoadOfflineAppAsync()
    {
        var webRoot = EnsureWebAssetsExtracted();
        var indexPath = Path.Combine(webRoot, "index.html");
        if (!File.Exists(indexPath))
        {
            MessageBox.Show("web-assets.zip is missing or invalid.", "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
            return;
        }

        var envPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StarAirADM",
            "WebView2Data"
        );
        Directory.CreateDirectory(envPath);

        var env = await CoreWebView2Environment.CreateAsync(null, envPath);
        await _webView.EnsureCoreWebView2Async(env);
        _webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "app.local",
            webRoot,
            CoreWebView2HostResourceAccessKind.Allow
        );
        _webView.Source = new Uri("https://app.local/index.html");
    }

    private static string EnsureWebAssetsExtracted()
    {
        var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StarAirADM"
        );
        var zipPath = Path.Combine(appData, "web-assets.zip");
        var extractPath = Path.Combine(appData, "web-dist");

        Directory.CreateDirectory(appData);

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StarAirDesktop.web-assets.zip"))
        {
            if (stream == null) throw new InvalidOperationException("Embedded web-assets.zip not found.");
            using var file = File.Create(zipPath);
            stream.CopyTo(file);
        }

        if (Directory.Exists(extractPath))
        {
            Directory.Delete(extractPath, recursive: true);
        }
        ZipFile.ExtractToDirectory(zipPath, extractPath);
        return extractPath;
    }
}
