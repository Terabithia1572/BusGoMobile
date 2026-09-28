using BusGoMobile.Services;

namespace BusGoMobile.Pages;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _db;

    public HomePage(DatabaseService db)
    {
      //  InitializeComponent();
        _db = db;
    }

    // Sayfa her göründüğünde kategorileri yükle
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var kategoriler = await _db.GetCategoriesAsync();
        CategoriesView.ItemsSource = kategoriler;
    }
}
