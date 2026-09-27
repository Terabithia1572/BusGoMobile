namespace BusGoMobile.Pages;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();

        // Şehir listesi
        var sehirler = new List<string>
        {
            "Adana", "Ankara", "Antalya", "Bursa", "Denizli",
            "Diyarbakır", "Erzurum", "Eskişehir", "Gaziantep", "İstanbul",
            "İzmir", "Kayseri", "Konya", "Malatya", "Mersin",
            "Samsun", "Trabzon", "Van"
        };

        FromPicker.ItemsSource = sehirler;
        ToPicker.ItemsSource = sehirler;

        // Tarih: bugünden itibaren seçilebilsin
        GoDatePicker.MinimumDate = DateTime.Today;
        GoDatePicker.Date = DateTime.Today;

        // Yolcu sayısı stepper'ı değişince etiketi güncelle
        PassengerStepper.ValueChanged += (s, e) =>
        {
            PassengerLabel.Text = $"{(int)e.NewValue} Yolcu";
        };

        // Yer değiştir: Nereden ↔ Nereye
        SwapButton.Clicked += (s, e) =>
        {
            var temp = FromPicker.SelectedIndex;
            FromPicker.SelectedIndex = ToPicker.SelectedIndex;
            ToPicker.SelectedIndex = temp;
        };
    }
}
