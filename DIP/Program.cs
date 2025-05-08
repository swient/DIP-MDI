using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DIP
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string systemDir = Environment.SystemDirectory;
            string dllPath = System.IO.Path.Combine(systemDir, "VCRUNTIME140.dll");
            if (!System.IO.File.Exists(dllPath))
            {
                MessageBox.Show(
                    "您的電腦缺少 VCRUNTIME140.dll，請安裝 Visual C++ Redistributable\n\n將為您開啟官方下載頁面，請依指示安裝後再重新啟動本程式",
                    "缺少元件",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://aka.ms/vs/17/release/vc_redist.x64.exe",
                    UseShellExecute = true
                });
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DIPSample());
        }
    }
}
