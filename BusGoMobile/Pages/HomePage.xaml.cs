using BusGoMobile.Services;

namespace BusGoMobile.Pages;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _db;

    public HomePage(DatabaseService db)
    {
       InitializeComponent();
        _db = db;
    }

    // Sayfa her göründüğünde kategorileri yükle
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (CategoriesView.ItemsSource == null)
        {
            var kategoriler = await _db.GetCategoriesAsync();
            CategoriesView.ItemsSource = kategoriler;
        }

        if (CampaignCarousel.ItemsSource == null)
        {
            var kampanyalar = await _db.GetCampaignsAsync();
            CampaignCarousel.ItemsSource = kampanyalar;
            StartAutoSlide(kampanyalar.Count);
        }
    }

    // Her 4 saniyede bir sonraki kampanyaya geç
    private void StartAutoSlide(int count)
    {
        if (count <= 1) return;

        Dispatcher.StartTimer(TimeSpan.FromSeconds(4), () =>
        {
            if (CampaignCarousel.ItemsSource == null)
                return false;

            int next = CampaignCarousel.Position + 1;
            if (next >= count) next = 0;
            CampaignCarousel.Position = next;

            return true;
        });
    }

}
