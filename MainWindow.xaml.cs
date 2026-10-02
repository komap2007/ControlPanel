using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using ControlPanel.Services;

namespace ControlPanel {
    public partial class MainWindow : Window {
        private readonly BrightnessService _brightness = new BrightnessService();
        private readonly VolumeService _volume = new VolumeService();
        private readonly HotkeyService _hotkey = new HotkeyService();
        private readonly ProcessService _process = new ProcessService();

        private bool _isPanelVisible = false;
        private nint _windowHandle;
        private bool _isReady = false;

        public MainWindow() {
            InitializeComponent();
            this.SourceInitialized += OnSourceInitialized;
            this.Closed += OnClosed;
        }

        private void OnSourceInitialized(object? sender, EventArgs e) {
            _windowHandle = new WindowInteropHelper(this).Handle;

            bool isRegistered = _hotkey.Register(_windowHandle, TogglePanelVisibility);
            if (!isRegistered)
                MessageBox.Show("Не удалось зарегистрировать Alt+Space", "Ошибка");

            // Синхронизация UI с системой
            int currentSysVolume = _volume.GetCurrentVolume();
            if (VolumeSlider != null) VolumeSlider.Value = currentSysVolume;
            if (VolumeValue != null) VolumeValue.Text = currentSysVolume + "%";

            if (BrightnessSlider != null) BrightnessSlider.Value = 100;
            if (BrightnessValue != null) BrightnessValue.Text = "100%";

            _isReady = true;

            // Скрываем окно после полной загрузки
            this.Hide();
        }

        private void OnClosed(object? sender, EventArgs e) {
            _hotkey.Unregister(_windowHandle);
            _brightness.SetBrightness(100);
            _volume.Dispose();
        }

        private void TogglePanelVisibility() {

            if (_isPanelVisible) {

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                fadeOut.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };

                fadeOut.Completed += (s, e) => {
                    this.Hide();
                    _isPanelVisible = false;
                };

                this.BeginAnimation(Window.OpacityProperty, fadeOut);
            }
            else {

                this.Show();
                this.Activate();

                this.Opacity = 0;

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250));
                fadeIn.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

                this.BeginAnimation(Window.OpacityProperty, fadeIn);
                _isPanelVisible = true;
            }
        }

        private void BrightnessSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
            if (!_isReady) return;
            int brightness = (int)e.NewValue;
            if (BrightnessValue != null) BrightnessValue.Text = brightness + "%";
            _brightness.SetBrightness(brightness);
        }

        private void VolumeSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
            if (!_isReady) return;
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