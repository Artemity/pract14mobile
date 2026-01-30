using pract14mobile.DTOs;
using pract14mobile.Services;

namespace pract14mobile.Views
{
    public partial class AddEditWarehousePage : ContentPage
    {
        private WarehouseDTO _warehouse;

        public AddEditWarehousePage(WarehouseDTO existingWarehouse = null)
        {
            InitializeComponent();

            if (existingWarehouse != null)
            {
                _warehouse = existingWarehouse;
                Title = "Редактировать склад";
            }
            else
            {
                _warehouse = new WarehouseDTO();
                Title = "Добавить склад";
            }

            tvWarehouse.BindingContext = _warehouse;
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(_warehouse.Address))
            {
                await DisplayAlert("Ошибка", "Введите адрес склада", "OK");
                return;
            }

            if (_warehouse.Phone <= 0)
            {
                await DisplayAlert("Ошибка", "Введите корректный телефон", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(_warehouse.ManagerLastName))
            {
                await DisplayAlert("Ошибка", "Введите фамилию менеджера", "OK");
                return;
            }

            try
            {
                if (_warehouse.Id == 0)
                {
                    // Добавление нового склада
                    var result = APIService.Post(_warehouse, "api/Warehouses");
                    await DisplayAlert("Успех", "Склад добавлен", "OK");
                }
                else
                {
                    // Обновление существующего склада
                    var success = APIService.Put(_warehouse, _warehouse.Id, "api/Warehouses");
                    if (success)
                    {
                        await DisplayAlert("Успех", "Склад обновлен", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось обновить склад", "OK");
                        return;
                    }
                }

                // Возврат на предыдущую страницу
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось сохранить: {ex.Message}", "OK");
            }
        }
    }
}