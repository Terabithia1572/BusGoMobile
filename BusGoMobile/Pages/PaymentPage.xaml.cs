using BusGoMobile.Services;

namespace BusGoMobile.Pages;

[QueryProperty(nameof(TripId), "tripId")]
[QueryProperty(nameof(SeatNumber), "seatNumber")]
public partial class PaymentPage : ContentPage
{
    private readonly DatabaseService _db;

    public int TripId { get; set; }
    public int SeatNumber { get; set; }

    public PaymentPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var trip = await _db.GetTripByIdAsync(TripId);
        if (trip != null)
            PriceLabel.Text = trip.Price.ToString();
    }

    // Kart numarası: 4'lü gruplar halinde biçimlendir + karta yansıt
    private void OnCardNumberChanged(object sender, TextChangedEventArgs e)
    {
        // Sadece rakamları al
        string digits = new string((e.NewTextValue ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length > 16) digits = digits.Substring(0, 16);

        // 4'lü gruplara böl
        var gruplu = string.Join(" ",
            Enumerable.Range(0, (digits.Length + 3) / 4)
                      .Select(i => digits.Substring(i * 4, Math.Min(4, digits.Length - i * 4))));

        // Input'u biçimlenmiş haliyle güncelle (sonsuz döngü olmasın diye kontrol)
        if (CardNumberEntry.Text != gruplu)
        {
            CardNumberEntry.Text = gruplu;
            CardNumberEntry.CursorPosition = gruplu.Length;
            return; // TextChanged tekrar tetiklenecek, aşağısı o turda çalışsın
        }

        // Karta yansıt: girilen kısmı göster, kalanı • ile doldur
        var kartGosterim = "";
        for (int i = 0; i < 16; i++)
        {
            kartGosterim += i < digits.Length ? digits[i].ToString() : "•";
            if ((i + 1) % 4 == 0 && i != 15) kartGosterim += " ";
        }
        CardNumberDisplay.Text = kartGosterim;
    }

    private void OnCardHolderChanged(object sender, TextChangedEventArgs e)
    {
        string isim = (e.NewTextValue ?? "").ToUpper();
        CardHolderDisplay.Text = string.IsNullOrWhiteSpace(isim) ? "AD SOYAD" : isim;
    }

    // Son kullanma: AA/YY biçimi + karta yansıt
    private void OnExpiryChanged(object sender, TextChangedEventArgs e)
    {
        string digits = new string((e.NewTextValue ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length > 4) digits = digits.Substring(0, 4);

        string bicimli = digits;
        if (digits.Length >= 3)
            bicimli = digits.Substring(0, 2) + "/" + digits.Substring(2);

        if (ExpiryEntry.Text != bicimli)
        {
            ExpiryEntry.Text = bicimli;
            ExpiryEntry.CursorPosition = bicimli.Length;
            return;
        }

        CardExpiryDisplay.Text = string.IsNullOrWhiteSpace(bicimli) ? "AA/YY" : bicimli;
    }

    private async void OnPayClicked(object sender, EventArgs e)
    {
        string digits = new string((CardNumberEntry.Text ?? "").Where(char.IsDigit).ToArray());

        if (digits.Length < 16 ||
            string.IsNullOrWhiteSpace(CardHolderEntry.Text) ||
            string.IsNullOrWhiteSpace(ExpiryEntry.Text) ||
            string.IsNullOrWhiteSpace(CvvEntry.Text))
        {
            await DisplayAlert("Eksik Bilgi", "Lütfen tüm kart bilgilerini eksiksiz girin.", "Tamam");
            return;
        }

        await DisplayAlert("Ödeme Başarılı",
            $"Ödemeniz alındı. Koltuk {SeatNumber} için biletiniz oluşturuluyor. (Bilet sayfası sonra eklenecek.)",
            "Tamam");
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        // Ödeme sayfasına git
        await Shell.Current.GoToAsync(
            $"PaymentPage?tripId={TripId}&seatNumber={SeatNumber}");

    }
}
