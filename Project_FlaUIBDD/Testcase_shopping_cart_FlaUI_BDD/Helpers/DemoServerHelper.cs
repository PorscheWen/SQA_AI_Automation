using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace ShoppingCartTests.Helpers
{
    /// <summary>
    /// 確保 shopping_cart demo HTTP 伺服器 (預設 :8888) 已啟動。
    /// </summary>
    public static class DemoServerHelper
    {
        private static Process? _startedProcess;
        private static bool _weStartedServer;

        public static void EnsureRunning(string applicationUrl)
        {
            if (!ConfigHelper.AutoStartDemoServer())
            {
                if (!IsReachable(applicationUrl))
                {
                    throw new InvalidOperationException(
                        $"Demo 網頁未啟動：{applicationUrl}。請先執行 demo\\shopping_cart\\serve.py 或設定 AutoStartDemoServer=true。");
                }

                return;
            }

            if (IsReachable(applicationUrl))
            {
                Console.WriteLine($"Demo 網頁已在執行：{applicationUrl}");
                return;
            }

            var serveScript = ResolveServeScriptPath();
            if (!File.Exists(serveScript))
            {
                throw new FileNotFoundException($"找不到 demo 伺服器腳本：{serveScript}");
            }

            var workingDir = Path.GetDirectoryName(serveScript)!;
            var python = ResolvePythonExecutable();

            Console.WriteLine($"正在啟動 Demo 網頁伺服器：{serveScript}");

            var startInfo = new ProcessStartInfo
            {
                FileName = python,
                Arguments = $"\"{serveScript}\"",
                WorkingDirectory = workingDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _startedProcess = Process.Start(startInfo);
            if (_startedProcess == null)
            {
                throw new InvalidOperationException("無法啟動 demo/shopping_cart/serve.py");
            }

            _weStartedServer = true;

            if (!WaitUntilReachable(applicationUrl, TimeSpan.FromSeconds(20)))
            {
                throw new TimeoutException(
                    $"Demo 網頁伺服器啟動逾時：{applicationUrl}。請確認 Python 可用且埠 8888 未被占用。");
            }

            Console.WriteLine($"Demo 網頁已就緒：{applicationUrl}");
        }

        public static void StopIfStarted()
        {
            if (!_weStartedServer || _startedProcess == null)
            {
                return;
            }

            try
            {
                if (!_startedProcess.HasExited)
                {
                    _startedProcess.Kill(entireProcessTree: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"停止 Demo 伺服器時發生錯誤: {ex.Message}");
            }
            finally
            {
                _startedProcess.Dispose();
                _startedProcess = null;
                _weStartedServer = false;
            }
        }

        private static bool IsReachable(string url)
        {
            var urls = new[] { url, "http://127.0.0.1:8888/" };
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };

            foreach (var target in urls)
            {
                try
                {
                    using var response = client.GetAsync(target).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
                catch
                {
                    // try next
                }
            }

            return false;
        }

        private static bool WaitUntilReachable(string url, TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline)
            {
                if (IsReachable(url))
                {
                    return true;
                }

                Thread.Sleep(500);
            }

            return false;
        }

        private static string ResolveServeScriptPath()
        {
            var configured = ConfigHelper.GetDemoServerScriptPath();
            if (!string.IsNullOrWhiteSpace(configured) && File.Exists(configured))
            {
                return Path.GetFullPath(configured);
            }

            var dir = AppContext.BaseDirectory;
            for (var i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
            {
                var candidate = Path.Combine(dir, "demo", "shopping_cart", "serve.py");
                if (File.Exists(candidate))
                {
                    return Path.GetFullPath(candidate);
                }

                dir = Path.GetDirectoryName(dir);
            }

            return Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "demo", "shopping_cart", "serve.py"));
        }

        private static string ResolvePythonExecutable()
        {
            var configured = Environment.GetEnvironmentVariable("PYTHON_EXE");
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return configured;
            }

            return "python";
        }
    }
}
