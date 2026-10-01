using System.Windows;       // Подключаем библиотеку для работы с окнами
using System.Windows.Input; // Подключаем библиотеку для работы с мышкой
using System.Diagnostics;   // Запуск программ

// Папка для кода
namespace ControlPanel {
    // Класс окна, наследуем от WPF Window
    public partial class MainWindow : Window {
        // Конструктор окна
        public MainWindow() {
            InitializeComponent(); // Читает XAML и рисует интерфейс
        }

        // Метод для перетаскивания окна мышкой
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            // Проверяем что нажата ЛКМ
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        // Кнопка 1: Открыть браузер
        private void OpenBrowser_Click(object sender, RoutedEventArgs e) {
            // Создаем настройки запуска
            var psi = new ProcessStartInfo {
                FileName = "https://www.google.com", // Что открываем
                UseShellExecute = true               // Говорим Windows: "Открой это как обычно"
            };

            Process.Start(psi);
            MessageBox.Show("Браузер открыт!", "Успех");
        }

        // Метод для кнопки "Открыть VS"
        private void OpenVS_Click(object sender, RoutedEventArgs e) {
            // Полный путь к Visual Studio 2022 Community (самая частая версия)
            // Символ @ перед строкой позволяет использовать обратные слеши \ без экранирования
            string vsPath = @"C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.exe";
            Process.Start(vsPath);
        }

        // Кнопка 3: Калькулятор
        private void OpenCalc_Click(object sender, RoutedEventArgs e) {
            Process.Start("calc");
            MessageBox.Show("Калькулятор открыт!", "Успех");
        }
    }
}