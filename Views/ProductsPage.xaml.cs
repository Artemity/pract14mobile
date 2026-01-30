using pract14mobile.DTOs;
using pract14mobile.Services;

namespace pract14mobile.Views
{
    public partial class ProductsPage : ContentPage
    {
        public ProductsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = APIService.Get<List<ProductDTO>>("api/Products");
                lvProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                DisplayAlert("Ошибка", $"Не удалось загрузить товары: {ex.Message}", "OK");
            }
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddEditProductPage());
        }

        private async void btnEdit_Clicked(object sender, EventArgs e)
        {
            var selectedProduct = (ProductDTO)lvProducts.SelectedItem;
            if (selectedProduct != null)
            {
                await Navigation.PushAsync(new AddEditProductPage(selectedProduct));
            }
            else
            {
                await DisplayAlert("Ошибка", "Выберите товар для редактирования", "OK");
            }
        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            var selectedProduct = (ProductDTO)lvProducts.SelectedItem;
            if (selectedProduct != null)
            {
                var confirm = await DisplayAlert("Подтверждение",
                    $"Удалить товар {selectedProduct.Name}?", "Да", "Нет");

                if (confirm)
                {
                    var success = APIService.Delete(selectedProduct.Id, "api/Products");
                    if (success)
                    {
                        LoadProducts();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось удалить товар", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Выберите товар для удаления", "OK");
            }
        }
    }
}