using System;
using System.Diagnostics;
using System.Windows;

namespace ControlPanel.Services {
    public class ProcessService {

        public void OpenBrowser() {
            var psi = new ProcessStartInfo { FileName = "https://www.google.com", UseShellExecute = true };
            Process.Start(psi);
        }

        public void OpenVS() {
            // Проверь путь, если у тебя другая версия VS!
            string vsPath = @"C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.exe";
            try { Process.Start(vsPath); }
            catch { MessageBox.Show("VS не найдена", "Ошибка"); }
        }

        public void OpenCalc() {
            Process.Start("calc");
        }
    }
}