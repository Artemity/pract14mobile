using pract14mobile.DTOs;
using pract14mobile.Services;

namespace pract14mobile.Views
{
    public partial class WarehousesPage : ContentPage
    {
        public WarehousesPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadWarehouses();
        }

        private void LoadWarehouses()
        {
            try
            {
                var warehouses = APIService.Get<List<WarehouseDTO>>("api/Warehouses");
                lvWarehouses.ItemsSource = warehouses;
            }
            catch (Exception ex)
            {
                DisplayAlert("Ошибка", $"Не удалось загрузить склады: {ex.Message}", "OK");
            }
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditWarehousePage());
        }

        private async void btnEdit_Clicked(object sender, EventArgs e)
        {
            var selectedWarehouse = (WarehouseDTO)lvWarehouses.SelectedItem;
            if (selectedWarehouse != null)
            {
                await Navigation.PushAsync(new AddEditWarehousePage(selectedWarehouse));
            }
            else
            {
                await DisplayAlert("Ошибка", "Выберите склад для редактирования", "OK");
            }
        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            var selectedWarehouse = (WarehouseDTO)lvWarehouses.SelectedItem;
            if (selectedWarehouse != null)
            {
                var confirm = await DisplayAlert("Подтверждение",
                    $"Удалить склад {selectedWarehouse.Address}?", "Да", "Нет");

                if (confirm)
                {
                    var success = APIService.Delete(selectedWarehouse.Id, "api/Warehouses");
                    if (success)
                    {
                        LoadWarehouses();
                        await DisplayAlert("Успех", "Склад удален", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось удалить склад", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Выберите склад для удаления", "OK");
            }
        }
    }
}