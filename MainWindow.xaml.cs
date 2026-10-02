using System;                      // Базовые типы C#
using System.Diagnostics;          // Для запуска программ (Process)
using System.Runtime.InteropServices; // Для связи с Windows API
using System.Windows;              // WPF: окна, MessageBox
using System.Windows.Input;        // Мышь и клавиатура
using System.Windows.Interop;      // Связь WPF с системными окнами Windows

namespace ControlPanel {
    public partial class MainWindow : Window {

        // Подключение функций Windows
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk); // fsModifiers - вспомогательная клавиша, vk - основная

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(nint hWnd, int id);

        private const int HOTKEY_ID = 9000;
        private const uint MOD_ALT = 0x0001;
        private const uint VK_SPACE = 0x20;
        private const int WM_HOTKEY = 0x0312;

        private bool _isPanelVisible = true;
        private nint _windowHandle; // паспорт окна

        public MainWindow() {
            InitializeComponent(); // Читает файл MainWindow.xaml

            this.SourceInitialized += OnSourceInitialized;
            // SourceInitialized: Это событие. В WPF окно рождается в несколько этапов.Сначала оно рисуется(InitializeComponent), а потом оно получает свой настоящий системный паспорт от Windows(SourceInitialized).
            // +=: Знак подписки. Мы говорим: "Подписываемся на это событие. Как только паспорт будет получен, вызови метод OnSourceInitialized".
            
            this.Closed += OnClosed;
            // Closed: Событие "Окно закрылось / умерло".
            // += OnClosed: "Когда окно будет закрываться, вызови метод OnClosed".
        }

        private void OnSourceInitialized(object? sender, EventArgs e) {
            // получение паспорта окна
            _windowHandle = new WindowInteropHelper(this).Handle;

            // начать следить за комбинацией клавиш
            bool isRegistered = RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_ALT, VK_SPACE);

            if (isRegistered)
                // регистрирует обработчик хука (Hook) для перехвата и обработки системных сообщений Windows (Win32) для конкретного окна.
                HwndSource.FromHwnd(_windowHandle)?.AddHook(WndProc);
            else
                MessageBox.Show("Не удалось зарегистрировать Alt+Space", "Ошибка");
        }

        private void OnClosed(object? sender, EventArgs e) {
            if (_windowHandle != 0)
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
        }

        // обработка сообщения
        private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
            if (msg == WM_HOTKEY) {
                TogglePanelVisibility();
                handled = true; // сообщает оконной системе (WPF), что вы успешно перехватили и обработали данное сообщение, и его больше не нужно передавать дальше по цепочке другим обработчикам.
            }
            return 0;
        }

        // метод, который скроет или покажет панель
        private void TogglePanelVisibility() {
            if (_isPanelVisible) {
                this.Hide();
                _isPanelVisible = false;
            }
            else {
                this.Show();
                this.Activate(); // Окно на передний план
            }
        }

        // перетаскивание окна
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void OpenBrowser_Click(object sender, RoutedEventArgs e) {
            var psi = new ProcessStartInfo { FileName = "https://www.google.com", UseShellExecute = true };
            Process.Start(psi);
        }

        private void OpenVS_Click(object sender, RoutedEventArgs e) {
            string vsPath = @"C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.exe";
            try { Process.Start(vsPath); }
            catch { MessageBox.Show("VS не найдена", "Ошибка"); }
        }

        private void OpenCalc_Click(object sender, RoutedEventArgs e) {
            Process.Start("calc");
        }
    }
}