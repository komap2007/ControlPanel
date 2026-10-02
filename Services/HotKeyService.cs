using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace ControlPanel.Services {
    public class HotkeyService {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(nint hWnd, int id);

        private const int HOTKEY_ID = 9000;
        private const uint MOD_ALT = 0x0001;
        private const uint VK_SPACE = 0x20;
        private const int WM_HOTKEY = 0x0312;

        private HwndSource? _source;

        // "Коробка" для метода главного окна
        private Action? _onHotkeyPressed;

        public bool Register(nint hwnd, Action onHotkeyPressed) {
            // 1. Кладем метод главного окна в коробку
            _onHotkeyPressed = onHotkeyPressed;
            _source = HwndSource.FromHwnd(hwnd);

            // 2. Говорим Windows следить за Alt+Space
            bool isRegistered = RegisterHotKey(hwnd, HOTKEY_ID, MOD_ALT, VK_SPACE);

            if (isRegistered)
                _source.AddHook(WndProc);
            return isRegistered;
        }

        public void Unregister(nint hwnd) {
            if (_source != null)
                _source.RemoveHook(WndProc);
            UnregisterHotKey(hwnd, HOTKEY_ID);
        }

        private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
            if (msg == WM_HOTKEY) {
                if (wParam == (nint)HOTKEY_ID) {
                    _onHotkeyPressed?.Invoke();
                    handled = true;
                }
            }
            return 0;
        }
    }
}