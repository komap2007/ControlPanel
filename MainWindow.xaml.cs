using System.Windows;       // Подключаем библиотеку для работы с окнами
using System.Windows.Input; // Подключаем библиотеку для работы с мышкой

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

        // Метод при клике на кнопку
        private void Button_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Привет!");
        }
    }
}