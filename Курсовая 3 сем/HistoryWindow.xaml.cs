using System.Collections.ObjectModel; // Для ObservableCollection
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_Generator
{
    public partial class HistoryWindow : Window
    {
        // Используем ObservableCollection, чтобы список обновлялся сам при удалении
        public ObservableCollection<HistoryItem> HistoryItems { get; set; } = [];
        public HistoryWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var data = DataManager.GetUserHistory();
            HistoryItems = new ObservableCollection<HistoryItem>(data);
            listHistory.ItemsSource = HistoryItems;

            if (HistoryItems.Count == 0)
            {
                // Можно показать надпись "Пусто", но пока просто оставим список пустым
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Получаем кнопку, на которую нажали
            if (sender is Button btn && btn.Tag is HistoryItem itemToDelete)
            {
                // 1. Удаляем визуально
                HistoryItems.Remove(itemToDelete);

                // 2. Удаляем из файла (надо реализовать метод в DataManager)
                DataManager.DeleteHistoryItem(itemToDelete.ID);
            }
        }

        private void BtnClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryItems.Count > 0)
            {
                if (MessageBox.Show("Вы уверены, что хотите очистить всю историю?", "Очистка", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    HistoryItems.Clear();
                    DataManager.ClearUserHistory();
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }
    }
}