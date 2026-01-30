using pract14mobile.DTOs;
using pract14mobile.Services;
using System.Collections.ObjectModel;

namespace pract14mobile.Views
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<StockItemInfoDTO> _stockItems = new ObservableCollection<StockItemInfoDTO>();

        public MainPage()
        {
            InitializeComponent();
            lvStockItems.ItemsSource = _stockItems;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadStockItems();
        }

        private void LoadStockItems()
        {
            try
            {
                var items = APIService.Get<List<StockItemInfoDTO>>("api/StockItems/Info");
                if (items != null)
                {
                    _stockItems.Clear();
                    foreach (var item in items)
                    {
                        _stockItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
            }
        }

        private async void btnAddStockItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditStockItemPage());
        }

        private async void btnEditStockItem_Clicked(object sender, EventArgs e)
        {
            var selected = lvStockItems.SelectedItem as StockItemInfoDTO;
            if (selected == null)
            {
                await DisplayAlert("Ошибка", "Выберите позицию для редактирования", "OK");
                return;
            }

            try
            {
                var stockItem = APIService.Get<StockItemDTO>($"api/StockItems/{selected.Id}");
                var editDTO = new StockItemEditDTO
                {
                    WarehouseId = stockItem.WarehouseId,
                    ProductId = stockItem.ProductId,
                    Quantity = stockItem.Quantity
                };

                await Navigation.PushAsync(new AddEditStockItemPage(editDTO, selected.Id));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить запись: {ex.Message}", "OK");
            }
        }

        private async void btnDeleteStockItem_Clicked(object sender, EventArgs e)
        {
            var selected = lvStockItems.SelectedItem as StockItemInfoDTO;
            if (selected == null)
            {
                await DisplayAlert("Ошибка", "Выберите позицию для удаления", "OK");
                return;
            }

            var confirm = await DisplayAlert("Подтверждение", "Удалить позицию?", "Да", "Нет");
            if (confirm)
            {
                try
                {
                    var success = APIService.Delete(selected.Id, "api/StockItems");
                    if (success)
                    {
                        LoadStockItems();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось удалить позицию", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Не удалось удалить запись: {ex.Message}", "OK");
                }
            }
        }

        private async void btnProducts_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductsPage());
        }

        private async void btnWarehouses_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WarehousesPage());
        }
    }
}