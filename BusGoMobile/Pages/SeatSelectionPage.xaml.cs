using BusGoMobile.Models;

namespace BusGoMobile.Pages;

public partial class SeatSelectionPage : ContentPage
{
    private readonly List<Seat> _seats = new();
    private Seat _selectedSeat;
    private const int BasePrice = 520;

    // Başlangıçta dolu koltuklar (sabit): koltuk no -> cinsiyet
    private readonly Dictionary<int, SeatStatus> _occupied = new()
    {
        { 1, SeatStatus.Female },
        { 2, SeatStatus.Male },
        { 3, SeatStatus.Male },
        { 8, SeatStatus.Female },
        { 12, SeatStatus.Male },
        { 13, SeatStatus.Male },
        { 17, SeatStatus.Male },
        { 18, SeatStatus.Male },
        { 19, SeatStatus.Female },
    };

    public SeatSelectionPage()
    {
        InitializeComponent();
        UretKoltuklar();
        CizKoltuklar();
    }

    // 26 koltuğu oluştur, dolu olanları işaretle
    private void UretKoltuklar()
    {
        for (int i = 1; i <= 26; i++)
        {
            var seat = new Seat
            {
                Number = i,
                Status = _occupied.ContainsKey(i) ? _occupied[i] : SeatStatus.Empty
            };
            _seats.Add(seat);
        }
    }

    // Koltukları 2+1 düzeninde ekrana çiz
    private void CizKoltuklar()
    {
        SeatsContainer.Children.Clear();

        // 26 koltuk: her sırada 3 koltuk (sol 1 + sağ 2). 8 sıra = 24, kalan 2 son sırada.
        int index = 0;
        while (index < _seats.Count)
        {
            // Bir sıra: sol tek koltuk + koridor + sağ iki koltuk
            var rowGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // sol
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // koridor
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // sağ-1
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // sağ-2
                },
                ColumnSpacing = 8
            };

            // Sol tek koltuk
            if (index < _seats.Count)
            {
                var solKoltuk = KoltukGorseli(_seats[index]);
                Grid.SetColumn(solKoltuk, 0);
                rowGrid.Children.Add(solKoltuk);
                index++;
            }

            // Koridor (1. sütun boş)

            // Sağ birinci koltuk
            if (index < _seats.Count)
            {
                var sag1 = KoltukGorseli(_seats[index]);
                Grid.SetColumn(sag1, 2);
                rowGrid.Children.Add(sag1);
                index++;
            }

            // Sağ ikinci koltuk
            if (index < _seats.Count)
            {
                var sag2 = KoltukGorseli(_seats[index]);
                Grid.SetColumn(sag2, 3);
                rowGrid.Children.Add(sag2);
                index++;
            }

            SeatsContainer.Children.Add(rowGrid);
        }
    }

    // Tek bir koltuğun görselini oluştur
    private View KoltukGorseli(Seat seat)
    {
        var border = new Border
        {
            HeightRequest = 46,
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            BackgroundColor = KoltukRengi(seat)
        };

        var content = new VerticalStackLayout
        {
            Spacing = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var numLabel = new Label
        {
            Text = seat.Number.ToString(),
            FontFamily = "InterBold",
            FontSize = 12,
            TextColor = KoltukYaziRengi(seat),
            HorizontalOptions = LayoutOptions.Center
        };
        content.Children.Add(numLabel);

        // Duruma göre alt sembol
        if (seat.Status == SeatStatus.Male || seat.Status == SeatStatus.Female)
        {
            var iconKey = seat.Status == SeatStatus.Male ? "IconMan" : "IconWoman";
            content.Children.Add(new Label
            {
                Text = (string)Application.Current.Resources[iconKey],
                FontFamily = "MaterialSymbols",
                FontSize = 13,
                TextColor = KoltukYaziRengi(seat),
                HorizontalOptions = LayoutOptions.Center
            });
        }
        else if (seat.Status == SeatStatus.Selected)
        {
            content.Children.Add(new Label
            {
                Text = (string)Application.Current.Resources["IconCheck"],
                FontFamily = "MaterialSymbols",
                FontSize = 13,
                TextColor = KoltukYaziRengi(seat),
                HorizontalOptions = LayoutOptions.Center
            });
        }

        border.Content = content;

        // Sadece seçilebilir koltuklara tıklama ekle
        if (seat.IsSelectable)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => KoltugaTiklandi(seat);
            border.GestureRecognizers.Add(tap);
        }

        return border;
    }

    private Color KoltukRengi(Seat seat) => seat.Status switch
    {
        SeatStatus.Selected => (Color)Application.Current.Resources["BgSecondaryContainer"],
        SeatStatus.Male => (Color)Application.Current.Resources["BgSurfaceContainerHighest"],
        SeatStatus.Female => (Color)Application.Current.Resources["BgErrorContainer"],
        _ => (Color)Application.Current.Resources["BgSurfaceContainerLow"]
    };

    private Color KoltukYaziRengi(Seat seat) => seat.Status switch
    {
        SeatStatus.Selected => (Color)Application.Current.Resources["BgOnSecondary"],
        SeatStatus.Male => (Color)Application.Current.Resources["BgPrimary"],
        SeatStatus.Female => (Color)Application.Current.Resources["BgError"],
        _ => (Color)Application.Current.Resources["BgOnSurface"]
    };

    private void KoltugaTiklandi(Seat seat)
    {
        // Aynı koltuğa tekrar tıklandıysa seçimi kaldır
        if (_selectedSeat == seat)
        {
            seat.Status = SeatStatus.Empty;
            _selectedSeat = null;
        }
        else
        {
            // Önceki seçimi boşalt
            if (_selectedSeat != null)
                _selectedSeat.Status = SeatStatus.Empty;

            seat.Status = SeatStatus.Selected;
            _selectedSeat = seat;
        }

        CizKoltuklar();     // koltukları yeniden çiz (renkler güncellensin)
        OzetGuncelle();
    }

    private void OzetGuncelle()
    {
        if (_selectedSeat == null)
        {
            SelectedSeatLabel.Text = "Koltuk Seçiniz";
            TotalPriceLabel.Text = "0 TL";
        }
        else
        {
            SelectedSeatLabel.Text = $"No: {_selectedSeat.Number} (Tekli/Çiftli)";
            TotalPriceLabel.Text = $"{BasePrice} TL";
        }
    }

    private async void OnProceedClicked(object sender, EventArgs e)
    {
        if (_selectedSeat == null)
        {
            await DisplayAlert("Koltuk Seçimi", "Lütfen devam etmek için bir koltuk seçin.", "Tamam");
            return;
        }

        await DisplayAlert("Seçim", $"{_selectedSeat.Number} numaralı koltuk seçildi. (Sonraki adım eklenecek.)", "Tamam");
    }
}
