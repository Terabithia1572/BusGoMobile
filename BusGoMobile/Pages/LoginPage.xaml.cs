using BusGoMobile.Services;

namespace BusGoMobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _db;
    private bool _emailMode = true;   // true = e-posta, false = telefon

    public LoginPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    // E-posta sekmesi
    private void OnEmailTabTapped(object sender, TappedEventArgs e)
    {
        _emailMode = true;

        TabEmailBorder.BackgroundColor = (Color)Application.Current.Resources["BgSurfaceContainerLowest"];
        TabEmailLabel.FontFamily = "InterSemiBold";
        TabEmailLabel.TextColor = (Color)Application.Current.Resources["BgOnSurface"];

        TabPhoneBorder.BackgroundColor = Colors.Transparent;
        TabPhoneLabel.FontFamily = "InterRegular";
        TabPhoneLabel.TextColor = (Color)Application.Current.Resources["BgOnSurfaceVariant"];

        PrimaryInputLabel.Text = "E-POSTA ADRESİ";
        PrimaryEntry.Placeholder = "ornek@email.com";
        PrimaryEntry.Keyboard = Keyboard.Email;
        PrimaryInputIcon.Text = (string)Application.Current.Resources["IconAlternateEmail"];
    }

    // Telefon sekmesi
    private void OnPhoneTabTapped(object sender, TappedEventArgs e)
    {
        _emailMode = false;

        TabPhoneBorder.BackgroundColor = (Color)Application.Current.Resources["BgSurfaceContainerLowest"];
        TabPhoneLabel.FontFamily = "InterSemiBold";
        TabPhoneLabel.TextColor = (Color)Application.Current.Resources["BgOnSurface"];

        TabEmailBorder.BackgroundColor = Colors.Transparent;
        TabEmailLabel.FontFamily = "InterRegular";
        TabEmailLabel.TextColor = (Color)Application.Current.Resources["BgOnSurfaceVariant"];

        PrimaryInputLabel.Text = "TELEFON NUMARASI";
        PrimaryEntry.Placeholder = "05XX XXX XX XX";
        PrimaryEntry.Keyboard = Keyboard.Telephone;
        PrimaryInputIcon.Text = (string)Application.Current.Resources["IconPhoneIphone"];
    }

    // Şifre göster/gizle
    private void OnTogglePasswordTapped(object sender, TappedEventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;

        PassEyeIcon.Text = PasswordEntry.IsPassword
            ? (string)Application.Current.Resources["IconVisibility"]
            : (string)Application.Current.Resources["IconVisibilityOff"];
    }

    // Giriş
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PrimaryEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Eksik Bilgi", "Lütfen e-posta ve şifrenizi girin.", "Tamam");
            return;
        }

        if (!_emailMode)
        {
            await DisplayAlert("Bilgi", "Telefon ile giriş henüz aktif değil, lütfen e-posta ile giriş yapın.", "Tamam");
            return;
        }

        var user = await _db.GetUserAsync(PrimaryEntry.Text.Trim(), PasswordEntry.Text);

        if (user is null)
        {
            await DisplayAlert("Giriş Başarısız", "E-posta veya şifre hatalı.", "Tamam");
            return;
        }

        await DisplayAlert("Hoş Geldiniz", $"Giriş başarılı! Merhaba {user.FirstName}.", "Tamam");

        // Sonraki adımda burada arama sayfasına yönlendireceğiz (navigasyon).
    }

    // Kayıt ol sayfasına git (navigasyon henüz kurulmadı)
    private async void OnGoToRegisterTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Bilgi", "Kayıt sayfasına geçiş navigasyon adımında bağlanacak.", "Tamam");
    }
}
