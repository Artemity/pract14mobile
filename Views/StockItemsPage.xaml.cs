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

            // Загружаем данные сразу при создании страницы
            LoadDataOnStart();
        }

        private async void LoadDataOnStart()
        {
            await LoadData();
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

                // Пробуем разные варианты endpoints
                string[] endpoints = new[]
                {
                    "api/StockItems/Info",          // Добавляем api/ префикс
                    "api/StockItems/info",          // в нижнем регистре
                    "api/StockItems/GetInfo",       // другой возможный вариант
                    "StockItems/GetInfo"           // без префикса
                };

                List<StockItemInfoDTO> items = null;
                Exception lastException = null;

                foreach (var endpoint in endpoints)
                {
                    try
                    {
                        Console.WriteLine($"Пробую endpoint: {endpoint}");
                        items = await APIService.GetListAsync<StockItemInfoDTO>(endpoint);
                        if (items != null && items.Count > 0)
                        {
                            Console.WriteLine($"Успешно загружено {items.Count} записей из {endpoint}");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        Console.WriteLine($"Ошибка для {endpoint}: {ex.Message}");
                        continue;
                    }
                }

                if (items == null)
                {
                    // Если не удалось загрузить через специальный endpoint, пробуем получить базовые данные
                    try
                    {
                        var basicItems = await APIService.GetListAsync<StockItemDTO>("api/StockItems");
                        if (basicItems != null && basicItems.Count > 0)
                        {
                            // Преобразуем базовые данные в нужный формат
                            items = new List<StockItemInfoDTO>();
                            foreach (var item in basicItems)
                            {
                                items.Add(new StockItemInfoDTO
                                {
                                    Id = item.Id,
                                    Quantity = item.Quantity,
                                    WarehouseAddress = $"Склад ID: {item.WarehouseId}",
                                    ProductName = $"Товар ID: {item.ProductId}",
                                    ProductCategory = "Не указано",
                                    Manufacturer = "Не указано"
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }
                }

                _stockItems.Clear();

                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        _stockItems.Add(item);
                    }
                    lblStatus.Text = $"Найдено записей: {_stockItems.Count}";
                    lblStatus.TextColor = Color.FromHex("#27ae60");
                }
                else
                {
                    lblStatus.Text = "Данные не найдены или произошла ошибка";
                    lblStatus.TextColor = Color.FromHex("#e74c3c");

                    // Добавляем тестовые данные для демонстрации
                    _stockItems.Add(new StockItemInfoDTO
                    {
                        Id = 1,
                        ProductName = "Тестовый товар",
                        ProductCategory = "Тест",
                        Manufacturer = "Тестовый производитель",
                        WarehouseAddress = "Тестовый склад",
                        Quantity = 100
                    });

                    if (lastException != null)
                    {
                        await DisplayAlert("Информация",
                            $"Используются тестовые данные. Ошибка API: {lastException.Message}",
                            "OK");
                    }
                }

                lblStatus.IsVisible = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
                lblStatus.Text = $"Ошибка: {ex.Message}";
                lblStatus.TextColor = Color.FromHex("#e74c3c");
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

            //if (sender is ListView listView)
            //{
            //    // Оставляем выделение для визуальной обратной связи
            //    // Убираем выделение через небольшой таймаут
            //    Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            //    {
            //        Device.BeginInvokeOnMainThread(() =>
            //        {
            //            listView.SelectedItem = null;
            //        });
            //        return false;
            //    });
            //}
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
                    var success = await APIService.DeleteAsync("api/StockItems", _selectedItem.Id);
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