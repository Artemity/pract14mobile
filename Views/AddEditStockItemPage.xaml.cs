using pract14mobile.Services;
using pract14mobile.DTOs;
using System.Collections.ObjectModel;

namespace pract14mobile.Views
{
    public partial class AddEditStockItemPage : ContentPage
    {
        private StockItemEditDTO _stockItem;
        private int _stockItemId;
        private ObservableCollection<WarehouseDTO> _warehouses;
        private ObservableCollection<ProductDTO> _products;

        public AddEditStockItemPage(StockItemEditDTO existingItem = null, int id = 0)
        {
            InitializeComponent();

            _warehouses = new ObservableCollection<WarehouseDTO>();
            _products = new ObservableCollection<ProductDTO>();

            pickerWarehouse.ItemsSource = _warehouses;
            pickerProduct.ItemsSource = _products;

            if (existingItem != null && id > 0)
            {
                _stockItem = existingItem;
                _stockItemId = id;
                Title = "Редактирование позиции";
            }
            else
            {
                _stockItem = new StockItemEditDTO();
                Title = "Добавление позиции";
            }

            tvStockItem.BindingContext = _stockItem;

            LoadWarehouses();
            LoadProducts();
        }

        private async void LoadWarehouses()
        {
            try
            {
                var warehouses = APIService.Get<List<WarehouseDTO>>("api/Warehouses");
                if (warehouses != null)
                {
                    _warehouses.Clear();
                    foreach (var warehouse in warehouses)
                        _warehouses.Add(warehouse);

                    if (_stockItem.WarehouseId > 0)
                    {
                        var warehouseToSelect = _warehouses.FirstOrDefault(w => w.Id == _stockItem.WarehouseId);
                        if (warehouseToSelect != null)
                        {
                            pickerWarehouse.SelectedItem = warehouseToSelect;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить склады: {ex.Message}", "OK");
            }
        }

        private async void LoadProducts()
        {
            try
            {
                var products = APIService.Get<List<ProductDTO>>("api/Products");
                if (products != null)
                {
                    _products.Clear();
                    foreach (var product in products)
                        _products.Add(product);

                    if (_stockItem.ProductId > 0)
                    {
                        var productToSelect = _products.FirstOrDefault(p => p.Id == _stockItem.ProductId);
                        if (productToSelect != null)
                        {
                            pickerProduct.SelectedItem = productToSelect;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить товары: {ex.Message}", "OK");
            }
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (pickerWarehouse.SelectedItem == null)
            {
                await DisplayAlert("Ошибка", "Выберите склад", "OK");
                return;
            }

            if (pickerProduct.SelectedItem == null)
            {
                await DisplayAlert("Ошибка", "Выберите товар", "OK");
                return;
            }

            _stockItem.WarehouseId = ((WarehouseDTO)pickerWarehouse.SelectedItem).Id;
            _stockItem.ProductId = ((ProductDTO)pickerProduct.SelectedItem).Id;

            if (_stockItem.Quantity <= 0)
            {
                await DisplayAlert("Ошибка", "Введите корректное количество", "OK");
                return;
            }

            try
            {
                if (_stockItemId == 0)
                {
                    var result = APIService.Post(_stockItem, "api/StockItems");
                    await DisplayAlert("Успех", "Позиция добавлена", "OK");
                }
                else
                {
                    var success = APIService.Put(_stockItem, _stockItemId, "api/StockItems");
                    if (success)
                    {
                        await DisplayAlert("Успех", "Позиция обновлена", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось обновить позицию", "OK");
                    }
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
            }
        }
    }
}