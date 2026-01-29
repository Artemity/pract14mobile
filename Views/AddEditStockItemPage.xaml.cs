using System.Collections.ObjectModel;
using pract14mobile.DTOs;
using pract14mobile.Services;

namespace pract14mobile.Views
{
    public partial class AddEditStockItemPage : ContentPage
    {
        private ObservableCollection<WarehouseDTO> _warehouses = new();
        private ObservableCollection<ProductDTO> _products = new();
        private bool _isEditMode = false;
        private int _currentId = 0;

        public AddEditStockItemPage()
        {
            InitializeComponent();

            pickerWarehouse.ItemsSource = _warehouses;
            pickerProduct.ItemsSource = _products;

            _isEditMode = Data.StockItem != null;
            lblTitle.Text = _isEditMode ? "Редактировать позицию" : "Добавить позицию";

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                loadingIndicator.IsRunning = true;
                loadingIndicator.IsVisible = true;

                // Загружаем склады - FIX: исправляем тип возвращаемого значения
                var warehouses = await APIService.GetListAsync<WarehouseDTO>("api/Warehouses");
                _warehouses.Clear();
                foreach (var w in warehouses)
                    _warehouses.Add(w);

                // Загружаем товары - FIX: исправляем тип возвращаемого значения
                var products = await APIService.GetListAsync<ProductDTO>("api/Products");
                _products.Clear();
                foreach (var p in products)
                    _products.Add(p);

                // Если редактирование - заполняем поля
                if (_isEditMode && Data.StockItem != null)
                {
                    _currentId = Data.StockItem.Id;

                    var warehouse = _warehouses.FirstOrDefault(w =>
                        w.Address == Data.StockItem.WarehouseAddress);
                    if (warehouse != null)
                        pickerWarehouse.SelectedItem = warehouse;

                    var product = _products.FirstOrDefault(p =>
                        p.Name == Data.StockItem.ProductName);
                    if (product != null)
                        pickerProduct.SelectedItem = product;

                    entryQuantity.Text = Data.StockItem.Quantity.ToString();
                }
                else
                {
                    entryQuantity.Text = "1";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
                await Navigation.PopModalAsync();
            }
            finally
            {
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
            }
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            // Валидация
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

            if (!int.TryParse(entryQuantity.Text, out int quantity) || quantity < 1)
            {
                await DisplayAlert("Ошибка", "Введите корректное количество (больше 0)", "OK");
                return;
            }

            try
            {
                loadingIndicator.IsRunning = true;
                loadingIndicator.IsVisible = true;

                var warehouse = (WarehouseDTO)pickerWarehouse.SelectedItem;
                var product = (ProductDTO)pickerProduct.SelectedItem;

                var stockItem = new StockItemDTO
                {
                    WarehouseId = warehouse.Id,
                    ProductId = product.Id,
                    Quantity = quantity
                };

                bool success;

                if (_isEditMode)
                {
                    stockItem.Id = _currentId;
                    // Используем правильный метод
                    success = await APIService.PutAsync(stockItem, _currentId, "api/StockItems");
                }
                else
                {
                    // Используем правильный метод
                    success = await APIService.PostAsync(stockItem, "api/StockItems");
                }

                if (success)
                {
                    await DisplayAlert("Успех",
                        _isEditMode ? "Данные обновлены" : "Позиция добавлена",
                        "OK");
                    Data.StockItem = null;
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось сохранить данные", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка сохранения: {ex.Message}", "OK");
            }
            finally
            {
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
            }
        }

        private async void btnCancel_Clicked(object sender, EventArgs e)
        {
            Data.StockItem = null;
            await Navigation.PopModalAsync();
        }
    }
}