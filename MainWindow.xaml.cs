using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ControlPanel.Services; // Подключаем нашу папку сервисов

namespace ControlPanel {
    public partial class MainWindow : Window {
        // Нанимаем сервисы (они будут жить, пока живо окно)
        private readonly BrightnessService _brightness = new BrightnessService();
        private readonly VolumeService _volume = new VolumeService();
        private readonly HotkeyService _hotkey = new HotkeyService();
        private readonly ProcessService _process = new ProcessService();

        private bool _isPanelVisible = true;
        private nint _windowHandle;

        public MainWindow() {
            InitializeComponent();
            this.SourceInitialized += OnSourceInitialized;
            this.Closed += OnClosed;
        }

        private void OnSourceInitialized(object? sender, EventArgs e) {
            _windowHandle = new WindowInteropHelper(this).Handle;

            // Говорим сервису: "Следи за клавишами. Когда нажмут — вызови мой метод TogglePanelVisibility"
            bool isRegistered = _hotkey.Register(_windowHandle, TogglePanelVisibility);

            if (!isRegistered)
                MessageBox.Show("Не удалось зарегистрировать Alt+Space", "Ошибка");
        }

        private void OnClosed(object? sender, EventArgs e) {
            _hotkey.Unregister(_windowHandle);
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

        // ===== Обработчики интерфейса =====

        private void BrightnessSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
            int brightness = (int)e.NewValue;
            if (BrightnessValue != null) BrightnessValue.Text = brightness + "%";

            // Просто просим сервис сделать это
            _brightness.SetBrightness(brightness);
        }

        private void VolumeSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
            int volume = (int)e.NewValue;
            if (VolumeValue != null) VolumeValue.Text = volume + "%";
            _volume.SetVolume(volume);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        private void OpenBrowser_Click(object sender, RoutedEventArgs e) => _process.OpenBrowser();
        private void OpenVS_Click(object sender, RoutedEventArgs e) => _process.OpenVS();
        private void OpenCalc_Click(object sender, RoutedEventArgs e) => _process.OpenCalc();
    }
}