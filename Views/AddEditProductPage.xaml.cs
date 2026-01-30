using pract14mobile.DTOs;
using pract14mobile.Services;

namespace pract14mobile.Views
{
    public partial class AddEditProductPage : ContentPage
    {
        private ProductDTO _product;

        public AddEditProductPage(ProductDTO existingProduct = null)
        {
            InitializeComponent();

            if (existingProduct != null)
            {
                _product = existingProduct;
                Title = "Редактировать товар";
            }
            else
            {
                _product = new ProductDTO();
                Title = "Добавить товар";
            }

            tvProduct.BindingContext = _product;
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_product.Name))
            {
                await DisplayAlert("Ошибка", "Введите название товара", "OK");
                return;
            }

            try
            {
                if (_product.Id == 0)
                {
                    var result = APIService.Post(_product, "api/Products");
                    await DisplayAlert("Успех", "Товар добавлен", "OK");
                }
                else
                {
                    var success = APIService.Put(_product, _product.Id, "api/Products");
                    if (success)
                    {
                        await DisplayAlert("Успех", "Товар обновлен", "OK");
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