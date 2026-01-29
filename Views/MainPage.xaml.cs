using System.Collections.ObjectModel;
using pract14mobile.DTOs;
using pract14mobile.Services;

namespace pract14mobile.Views
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<ProductDTO> _products;
        private ProductDTO _selectedProduct;

        public MainPage()
        {
            InitializeComponent();
            _products = new ObservableCollection<ProductDTO>();
            lvProducts.ItemsSource = _products;
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = APIService.Get<ProductDTO>("api/Products");
                _products.Clear();
                foreach (var product in products)
                {
                    _products.Add(product);
                }
            }


            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Ошибка", $"Не удалось загрузить продукты: {ex.Message}", "OK");
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProducts();
        }

        private void OnProductSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _selectedProduct = e.SelectedItem as ProductDTO;
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            lvProducts.SelectedItem = null;
            _selectedProduct = null;
            await Navigation.PushModalAsync(new AddEditProductPage());
        }

        private async void btnEdit_Clicked(object sender, EventArgs e)
        {
            if (_selectedProduct == null)
            {
                await DisplayAlert("Ошибка", "Выберите продукт для редактирования", "OK");
                return;
            }

            Data.Product = _selectedProduct;
            await Navigation.PushModalAsync(new AddEditProductPage());
        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            if (_selectedProduct == null)
            {
                await DisplayAlert("Ошибка", "Выберите продукт для удаления", "OK");
                return;
            }

            bool confirm = await DisplayAlert("Подтверждение",
                $"Удалить продукт: {_selectedProduct.Name}?", "Да", "Нет");

            if (confirm)
            {
                try
                {
                    bool success = APIService.Delete("api/Products", _selectedProduct.Id);
                    if (success)
                    {
                        await DisplayAlert("Успех", "Продукт удален", "OK");
                        // Сбрасываем выделение
                        lvProducts.SelectedItem = null;
                        _selectedProduct = null;
                        LoadProducts();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось удалить продукт", "OK");
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