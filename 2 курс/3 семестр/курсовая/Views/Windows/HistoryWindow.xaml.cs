using QR_generator.Models;
using QR_generator.Services;
using QR_generator.Views.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QR_generator.Views.Windows
{
    public partial class HistoryWindow : Window
    {
        public ObservableCollection<HistoryItem> HistoryItems { get; set; } = []; // список обновляется сам
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
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is HistoryItem itemToDelete)
            {
                HistoryItems.Remove(itemToDelete);
                DataManager.DeleteHistoryItem(itemToDelete.ID);
            }
        }

        private void BtnClearAll_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryItems.Count > 0)
            {
                var dialog = new ConfirmLogoutWindow("Вы уверены, что хотите\nочистить всю историю?")
                {
                    Owner = this // Чтобы диалог открылся поверх этого окна
                };

                if (dialog.ShowDialog() == true)
                {
                    HistoryItems.Clear();
                    DataManager.ClearUserHistory();
                }
                Close();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) 
                DragMove();
        }
    }
}