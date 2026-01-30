using pract14mobile.Views;

namespace pract14mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Регистрируем маршруты для навигации
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(AddEditStockItemPage), typeof(AddEditStockItemPage));
            Routing.RegisterRoute(nameof(AddEditProductPage), typeof(AddEditProductPage));
            Routing.RegisterRoute(nameof(AddEditWarehousePage), typeof(AddEditWarehousePage));
            Routing.RegisterRoute(nameof(ProductsPage), typeof(ProductsPage));
            Routing.RegisterRoute(nameof(WarehousesPage), typeof(WarehousesPage));
        }
    }
}