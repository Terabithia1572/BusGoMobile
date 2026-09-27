using System.Text.RegularExpressions;
using BusGoMobile.Models;
using BusGoMobile.Services;

namespace BusGoMobile.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly DatabaseService _db;

    public RegisterPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
            string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Eksik Bilgi", "Lütfen tüm zorunlu alanları doldurun.", "Tamam");
            return;
        }

        if (!TermsCheck.IsChecked)
        {
            await DisplayAlert("Onay Gerekli", "Kullanım koşullarını kabul etmelisiniz.", "Tamam");
            return;
        }

        if (await _db.EmailExistsAsync(EmailEntry.Text.Trim()))
        {
            await DisplayAlert("Kayıtlı E-posta", "Bu e-posta ile zaten bir hesap var.", "Tamam");
            return;
        }

        var user = new User
        {
            FirstName = FirstNameEntry.Text.Trim(),
            LastName = LastNameEntry.Text.Trim(),
            Email = EmailEntry.Text.Trim(),
            Phone = PhoneEntry.Text?.Trim(),
            Password = PasswordEntry.Text,
            MarketingOptIn = MarketingCheck.IsChecked ? 1 : 0,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        try
        {
            await _db.AddUserAsync(user);
            await DisplayAlert("Başarılı", "Hesabınız oluşturuldu!", "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", "Kayıt sırasında bir sorun oluştu: " + ex.Message, "Tamam");
        }
    }
}
