using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace OpenMsxMcpLauncher
{
    class Program
    {
        static int Main(string[] args)
        {
            string exe = Environment.GetEnvironmentVariable("OPENMSX_REAL_EXECUTABLE");
            if (string.IsNullOrWhiteSpace(exe))
                exe = @"C:\Program Files\openMSX\openmsx.exe";

            if (!File.Exists(exe))
            {
                Console.Error.WriteLine("Error: openMSX not found: " + exe);
                return 1;
            }

            string tempDir = Environment.GetEnvironmentVariable("TEMP");
            if (string.IsNullOrWhiteSpace(tempDir))
                tempDir = Path.GetTempPath();
            string socketDir = Path.Combine(tempDir, "openmsx-default");
            Directory.CreateDirectory(socketDir);

            int wrapperPid = Process.GetCurrentProcess().Id;
            var before = GetOpenMsxPids();

            // Prefer explorer.exe on a .lnk (supports args). Direct CreateProcess exits 1.
            string launchTarget;
            string tempLnk = Path.Combine(Path.GetTempPath(), "openmsx-mcp-launch-" + wrapperPid + ".lnk");
            try
            {
                Type t = Type.GetTypeFromProgID("WScript.Shell");
                object sh = Activator.CreateInstance(t);
                object shortcut = t.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, sh, new object[] { tempLnk });
                Type st = shortcut.GetType();
                st.InvokeMember("TargetPath", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { exe });
                st.InvokeMember("WorkingDirectory", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { Path.GetDirectoryName(exe) });
                st.InvokeMember("Arguments", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { QuoteArgs(args) });
                st.InvokeMember("Save", System.Reflection.BindingFlags.InvokeMethod, null, shortcut, null);
                launchTarget = tempLnk;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[openmsx-mcp-launcher] shortcut failed: " + ex.Message + "; launching exe only");
                launchTarget = exe;
            }
            var psi = new ProcessStartInfo();
            psi.FileName = "explorer.exe";
            psi.Arguments = "\"" + launchTarget + "\"";
            psi.UseShellExecute = false;
            Process.Start(psi);
            Console.Error.WriteLine("[openmsx-mcp-launcher] explorer launch wrapperPid=" + wrapperPid + " target=" + launchTarget);

            int omsxPid = 0;
            for (int i = 0; i < 50; i++)
            {
                Thread.Sleep(100);
                foreach (int pid in GetOpenMsxPids())
                {
                    if (!before.Contains(pid))
                    {
                        omsxPid = pid;
                        break;
                    }
                }
                if (omsxPid != 0) break;
            }
            if (omsxPid == 0)
            {
                Console.Error.WriteLine("Error: openMSX process did not appear");
                return 1;
            }
            Console.Error.WriteLine("[openmsx-mcp-launcher] openMSX pid=" + omsxPid);

            string src = Path.Combine(socketDir, "socket." + omsxPid);
            string dst = Path.Combine(socketDir, "socket." + wrapperPid);
            string port = null;
            for (int i = 0; i < 150; i++)
            {
                // Ensure openMSX is still alive while waiting for the control socket
                bool alive = false;
                foreach (int pid in GetOpenMsxPids())
                {
                    if (pid == omsxPid) { alive = true; break; }
                }
                if (!alive)
                {
                    Console.Error.WriteLine("Error: openMSX exited before creating control socket");
                    return 1;
                }
                if (File.Exists(src))
                {
                    try
                    {
                        port = File.ReadAllText(src).Trim();
                        if (!string.IsNullOrEmpty(port)) break;
                    }
                    catch { }
                }
                Thread.Sleep(100);
            }
            if (string.IsNullOrEmpty(port))
            {
                Console.Error.WriteLine("Error: openMSX socket not found: " + src);
                try
                {
                    foreach (var f in Directory.GetFiles(socketDir, "socket.*"))
                        Console.Error.WriteLine("[openmsx-mcp-launcher] seen " + f);
                }
                catch { }
                TryKill(omsxPid);
                return 1;
            }

            File.WriteAllText(dst, port + Environment.NewLine);
            Console.Error.WriteLine("[openmsx-mcp-launcher] mirrored socket port=" + port);

            Console.CancelKeyPress += (s, e) => { e.Cancel = true; Cleanup(dst, omsxPid); Environment.Exit(0); };
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Cleanup(dst, omsxPid);

            while (true)
            {
                Thread.Sleep(1000);
                bool alive = false;
                foreach (int pid in GetOpenMsxPids())
                {
                    if (pid == omsxPid) { alive = true; break; }
                }
                if (!alive)
                {
                    Console.Error.WriteLine("[openmsx-mcp-launcher] openMSX exited");
                    Cleanup(dst, omsxPid);
                    return 0;
                }
            }
        }

        static string QuoteArgs(string[] args)
        {
            if (args == null || args.Length == 0) return "";
            var parts = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];
                if (a.IndexOf(' ') >= 0 || a.IndexOf('"') >= 0)
                    parts[i] = "\"" + a.Replace("\"", "\\\"") + "\"";
                else
                    parts[i] = a;
            }
            return string.Join(" ", parts);
        }

        static System.Collections.Generic.HashSet<int> GetOpenMsxPids()
        {
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (var p in Process.GetProcessesByName("openmsx"))
            {
                try { set.Add(p.Id); } catch { }
            }
            return set;
        }

        static void TryKill(int pid)
        {
            try
            {
                var p = Process.GetProcessById(pid);
                p.Kill();
            }
            catch { }
        }

        static void Cleanup(string dst, int omsxPid)
        {
            try { if (File.Exists(dst)) File.Delete(dst); } catch { }
            TryKill(omsxPid);
        }
    }
}
