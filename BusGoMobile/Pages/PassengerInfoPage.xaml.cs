namespace BusGoMobile.Pages;

public partial class PassengerInfoPage : ContentPage
{
    private string _gender = "Male"; // varsayılan

    public PassengerInfoPage()
    {
        InitializeComponent();
    }

    private void OnMaleTapped(object sender, TappedEventArgs e)
    {
        _gender = "Male";

        MaleButton.BackgroundColor = (Color)Application.Current.Resources["BgSecondaryContainer"];
        MaleIcon.TextColor = (Color)Application.Current.Resources["BgOnSecondary"];
        MaleText.TextColor = (Color)Application.Current.Resources["BgOnSecondary"];
        MaleText.FontFamily = "InterSemiBold";

        FemaleButton.BackgroundColor = (Color)Application.Current.Resources["BgSurfaceContainerLow"];
        FemaleIcon.TextColor = (Color)Application.Current.Resources["BgOnSurfaceVariant"];
        FemaleText.TextColor = (Color)Application.Current.Resources["BgOnSurfaceVariant"];
        FemaleText.FontFamily = "InterRegular";
    }

    private void OnFemaleTapped(object sender, TappedEventArgs e)
    {
        _gender = "Female";

        FemaleButton.BackgroundColor = (Color)Application.Current.Resources["BgSecondaryContainer"];
        FemaleIcon.TextColor = (Color)Application.Current.Resources["BgOnSecondary"];
        FemaleText.TextColor = (Color)Application.Current.Resources["BgOnSecondary"];
        FemaleText.FontFamily = "InterSemiBold";

        MaleButton.BackgroundColor = (Color)Application.Current.Resources["BgSurfaceContainerLow"];
        MaleIcon.TextColor = (Color)Application.Current.Resources["BgOnSurfaceVariant"];
        MaleText.TextColor = (Color)Application.Current.Resources["BgOnSurfaceVariant"];
        MaleText.FontFamily = "InterRegular";
    }

    private async void OnPayClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
            string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
            string.IsNullOrWhiteSpace(PhoneEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Eksik Bilgi", "Lütfen tüm alanları doldurun.", "Tamam");
            return;
        }

        if (!TermsCheck.IsChecked)
        {
            await DisplayAlert("Onay Gerekli", "Lütfen seyahat şartlarını onaylayın.", "Tamam");
            return;
        }

        await DisplayAlert("Bilgiler Alındı",
            $"{FirstNameEntry.Text} {LastNameEntry.Text} için ödeme adımına geçiliyor. (Ödeme sayfası sonra eklenecek.)",
            "Tamam");
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
