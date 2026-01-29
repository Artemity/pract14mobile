using pract14mobile.DTOs;
using pract14mobile.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace pract14mobile.Views;

public partial class AddEditProductPage : ContentPage, INotifyPropertyChanged
{
    private bool isEditMode = false;
    private ProductDTO currentProduct;

    private string _productName;
    private string _category;
    private string _manufacturer;

    public string TitleText => isEditMode ? "Редактировать продукт" : "Добавить новый продукт";

    public string ProductName
    {
        get => _productName;
        set
        {
            if (_productName != value)
            {
                _productName = value;
                OnPropertyChanged();
            }
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            if (_category != value)
            {
                _category = value;
                OnPropertyChanged();
            }
        }
    }

    public string Manufacturer
    {
        get => _manufacturer;
        set
        {
            if (_manufacturer != value)
            {
                _manufacturer = value;
                OnPropertyChanged();
            }
        }
    }

    public AddEditProductPage()
    {
        InitializeComponent();
        BindingContext = this;

        if (Data.Product != null)
        {
            isEditMode = true;
            currentProduct = Data.Product;

            // Устанавливаем значения через свойства с уведомлением
            ProductName = currentProduct.Name;
            Category = currentProduct.Category;
            Manufacturer = currentProduct.Manufacturer;
        }
        else
        {
            isEditMode = false;
        }
    }

    private async void btnSave_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryName.Text))
        {
            await DisplayAlert("Ошибка", "Введите название продукта", "OK");
            return;
        }

        try
        {
            var product = new ProductDTO
            {
                Name = entryName.Text,
                Category = entryCategory.Text,
                Manufacturer = entryManufacturer.Text
            };

            if (isEditMode)
            {
                product.Id = currentProduct.Id;
                await APIService.Put(product, product.Id, "api/products");
                await DisplayAlert("Успешно", "Продукт обновлен", "OK");
            }
            else
            {
                await APIService.Post(product, "api/products");
                await DisplayAlert("Успешно", "Продукт добавлен", "OK");
            }

            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось сохранить продукт: {ex.Message}", "OK");
        }
    }

    private async void btnCancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    // Реализация INotifyPropertyChanged
    public new event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}