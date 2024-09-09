using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SpiWpf.Wpf.Helpers
{
    public class CallPing
    {
        public void PingIp(string IpAddress)
        {
            string ip = IpAddress;
            string argumento = $"/C ping {ip} -t";

            // Crear el proceso para ejecutar CMD
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = argumento;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.CreateNoWindow = false; // Muestra la ventana del CMD
            process.StartInfo.UseShellExecute = false; // Permite usar el shell
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

            // Iniciar el proceso
            process.Start();
        }
    }
}
