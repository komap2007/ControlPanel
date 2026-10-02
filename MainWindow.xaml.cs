using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace ControlPanel {
    public partial class MainWindow : Window {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(nint hWnd, int id);

        [DllImport("gdi32.dll")]
        private static extern bool SetDeviceGammaRamp(nint hDC, ref RAMP ramp);

        [DllImport("user32.dll")]
        private static extern nint GetDC(nint hWnd);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct RAMP {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Blue;
        }

        private const int HOTKEY_ID = 9000;
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint VK_SPACE = 0x20;
        private const uint VK_R = 0x52;
        private const int WM_HOTKEY = 0x0312;

        private const int HOTKEY_ID_RESET = 9001;

        private bool _isPanelVisible = true;
        private nint _windowHandle;

        public MainWindow() {
            ResetBrightness();
            InitializeComponent();
            this.SourceInitialized += OnSourceInitialized;
            this.Closed += OnClosed;
        }

        private void OnSourceInitialized(object? sender, EventArgs e) {
            _windowHandle = new WindowInteropHelper(this).Handle;

            // Ctrl+Alt+Space (показать/скрыть панель)
            bool isRegistered = RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, VK_SPACE);

            // Ctrl+Alt+Shift+R (сброс яркости)
            bool isResetRegistered = RegisterHotKey(_windowHandle, HOTKEY_ID_RESET, MOD_CONTROL | MOD_ALT | MOD_SHIFT, VK_R);

            if (isRegistered && isResetRegistered) {
                HwndSource.FromHwnd(_windowHandle)?.AddHook(WndProc);
            }
            else {
                MessageBox.Show("Не удалось зарегистрировать горячие клавиши. Возможно, они заняты другой программой.", "Ошибка");
            }
        }

        private void OnClosed(object? sender, EventArgs e) {
            if (_windowHandle != 0) {
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
                UnregisterHotKey(_windowHandle, HOTKEY_ID_RESET);
            }
            SetBrightness(100);
        }

        private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
            if (msg == WM_HOTKEY) {
                int hotkeyId = wParam.ToInt32();

                if (hotkeyId == HOTKEY_ID) {
                    TogglePanelVisibility();
                }
                else if (hotkeyId == HOTKEY_ID_RESET) {
                    ResetBrightness();
                    BrightnessSlider.Value = 100;
                    BrightnessValue.Text = "100%";
                }

                handled = true;
            }
            return 0;
        }

        private void TogglePanelVisibility() {
            if (_isPanelVisible) {
                this.Hide();
                _isPanelVisible = false;
            }
            else {
                this.Show();
                this.Activate();
                _isPanelVisible = true;
            }
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            int brightness = (int)e.NewValue;

            if (BrightnessValue != null)
                BrightnessValue.Text = brightness + "%";

            SetBrightness(brightness);
        }

        private void SetBrightness(int brightness) {
            if (brightness < 30) brightness = 30;
            if (brightness > 100) brightness = 100;

            RAMP ramp = new RAMP();
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];

            for (int i = 0; i < 256; i++) {
                ushort value = (ushort)(i * (brightness / 100.0) * (ushort.MaxValue / 255.0));
                ramp.Red[i] = value;
                ramp.Green[i] = value;
                ramp.Blue[i] = value;
            }

            nint hdc = GetDC(nint.Zero);
            SetDeviceGammaRamp(hdc, ref ramp);
        }

        private void ResetBrightness() {
            RAMP ramp = new RAMP();
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];

            for (int i = 0; i < 256; i++) {
                ushort value = (ushort)(i * (ushort.MaxValue / 255.0));
                ramp.Red[i] = value;
                ramp.Green[i] = value;
                ramp.Blue[i] = value;
            }

            nint hdc = GetDC(nint.Zero);
            SetDeviceGammaRamp(hdc, ref ramp);
        }

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