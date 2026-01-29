using System.Collections.ObjectModel;
using pract14mobile.DTOs;
using pract14mobile.Services;

namespace pract14mobile.Views
{
    public partial class StockItemsPage : ContentPage
    {
        private ObservableCollection<StockItemInfoDTO> _stockItems;
        private StockItemInfoDTO _selectedItem;

        public StockItemsPage()
        {
            InitializeComponent();
            _stockItems = new ObservableCollection<StockItemInfoDTO>();
            lvStockItems.ItemsSource = _stockItems;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                loadingIndicator.IsRunning = true;
                loadingIndicator.IsVisible = true;
                lblStatus.IsVisible = false;

                var items = await APIService.GetListAsync<StockItemInfoDTO>("StockItems/Info");

                _stockItems.Clear();
                foreach (var item in items)
                {
                    _stockItems.Add(item);
                }

                lblStatus.Text = $"Найдено: {_stockItems.Count} записей";
                lblStatus.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
                lblStatus.Text = "Ошибка загрузки";
                lblStatus.IsVisible = true;
            }
            finally
            {
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
            }
        }

        private void LvStockItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedItem = e.SelectedItem as StockItemInfoDTO;
            // Снимаем выделение для лучшего UX
            if (sender is ListView listView)
            {
                listView.SelectedItem = null;
            }
        }

        private async void BtnAdd_Clicked(object sender, EventArgs e)
        {
            Data.StockItem = null;
            await Navigation.PushModalAsync(new AddEditStockItemPage());
        }

        private async void BtnEdit_Clicked(object sender, EventArgs e)
        {
            if (_selectedItem == null)
            {
                await DisplayAlert("Внимание", "Выберите элемент для редактирования", "OK");
                return;
            }

            Data.StockItem = _selectedItem;
            await Navigation.PushModalAsync(new AddEditStockItemPage());
        }

        private async void BtnDelete_Clicked(object sender, EventArgs e)
        {
            if (_selectedItem == null)
            {
                await DisplayAlert("Внимание", "Выберите элемент для удаления", "OK");
                return;
            }

            var confirm = await DisplayAlert("Подтвердить удаление",
                $"Вы действительно хотите удалить позицию '{_selectedItem.ProductName}'?",
                "Да", "Нет");

            if (confirm)
            {
                try
                {
                    // FIX: Используем правильный метод DeleteAsync
                    var success = await APIService.DeleteAsync("StockItems", _selectedItem.Id);
                    if (success)
                    {
                        await DisplayAlert("Успех", "Позиция успешно удалена", "OK");
                        _selectedItem = null;
                        await LoadData();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось удалить позицию", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Ошибка при удалении: {ex.Message}", "OK");
                }
            }
        }
    }
}