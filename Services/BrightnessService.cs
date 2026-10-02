using System;                           // база
using System.Runtime.InteropServices;   // WinAPI
using System.Windows.Interop;           // WPF

namespace ControlPanel.Services {
    public class BrightnessService {

        // WinAPI для яркости
        [DllImport("gdi32.dll")]
        private static extern bool SetDeviceGammaRamp(nint hDC, ref RAMP ramp);

        [DllImport("user32.dll")]
        private static extern nint GetDC(nint hWnd);

        // Коробка цветов
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct RAMP {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] Blue;
        }

        // Установить яркость (30-100%)
        public void SetBrightness(int brightness) {
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

            // Ключ от всего экрана
            nint hdc = GetDC(nint.Zero);

            // Отправить ramp в Windows
            SetDeviceGammaRamp(hdc, ref ramp);
        }
    }
}