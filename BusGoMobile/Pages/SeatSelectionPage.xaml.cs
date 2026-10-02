using BusGoMobile.Models;

namespace BusGoMobile.Pages;

public partial class SeatSelectionPage : ContentPage
{
    private readonly List<Seat> _seats = new();
    private Seat _selectedSeat;
    private const int BasePrice = 520;
    private const int ToplamKoltuk = 28;

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
        { 24, SeatStatus.Male },
    };

    public SeatSelectionPage()
    {
        InitializeComponent();
        UretKoltuklar();
        CizKoltuklar();
        OzetGuncelle();
    }

    private void UretKoltuklar()
    {
        for (int i = 1; i <= ToplamKoltuk; i++)
        {
            _seats.Add(new Seat
            {
                Number = i,
                Status = _occupied.ContainsKey(i) ? _occupied[i] : SeatStatus.Empty
            });
        }
    }

    // Bir koltuğun tipini döndür (sol tek / sağ pencere / sağ koridor / arka)
    private string KoltukTipi(int number)
    {
        // İlk 24 koltuk: 8 sıra × 3 (2+1). Son 4 (25-28): arka sıra
        if (number >= 25) return "Arka Koltuk";

        int siraIcindeki = (number - 1) % 3; // 0=sol tek, 1=sağ pencere, 2=sağ koridor
        return siraIcindeki switch
        {
            0 => "Tekli Koltuk",
            1 => "Çiftli (Pencere)",
            2 => "Çiftli (Koridor)",
            _ => "Koltuk"
        };
    }

    // Bir koltuğun 2+1 çiftindeki komşusunu bul (sadece sağ çift koltuklar için)
    private Seat CiftKomsu(int number)
    {
        if (number >= 25) return null; // arka sıranın çift kuralı yok

        int siraIcindeki = (number - 1) % 3;
        if (siraIcindeki == 1) return _seats.FirstOrDefault(s => s.Number == number + 1); // pencere -> koridor
        if (siraIcindeki == 2) return _seats.FirstOrDefault(s => s.Number == number - 1); // koridor -> pencere
        return null; // sol tek koltuğun çifti yok
    }

    private void CizKoltuklar()
    {
        SeatsContainer.Children.Clear();

        // Ön kısım: 1-24 arası, 8 sıra 2+1
        int index = 0;
        int siraSayaci = 0;

        while (index < 24)
        {
            var rowGrid = YeniSira();

            // sol tek
            var sol = KoltukGorseli(_seats[index]);
            Grid.SetColumn(sol, 0);
            rowGrid.Children.Add(sol);
            index++;

            // sağ pencere
            var sag1 = KoltukGorseli(_seats[index]);
            Grid.SetColumn(sag1, 2);
            rowGrid.Children.Add(sag1);
            index++;

            // sağ koridor
            var sag2 = KoltukGorseli(_seats[index]);
            Grid.SetColumn(sag2, 3);
            rowGrid.Children.Add(sag2);
            index++;

            SeatsContainer.Children.Add(rowGrid);
            siraSayaci++;

            // 4. sıradan sonra orta kapı bandı
            if (siraSayaci == 4)
                SeatsContainer.Children.Add(OrtaKapiBandi());
        }

        // Arka sıradan önce ikram/bagaj bilgisi
        SeatsContainer.Children.Add(BilgiBandi());

        // Arka 4'lü sıra başlığı
        SeatsContainer.Children.Add(new Label
        {
            Text = "ARKA 4'LÜ KOLTUK SIRASI",
            FontFamily = "InterBold",
            FontSize = 11,
            TextColor = (Color)Application.Current.Resources["BgOnSurfaceVariant"],
            HorizontalOptions = LayoutOptions.Center
        });

        // Arka 4'lü sıra (25-28)
        var arkaGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            },
            ColumnSpacing = 8
        };
        for (int i = 24; i < 28; i++)
        {
            var koltuk = KoltukGorseli(_seats[i]);
            Grid.SetColumn(koltuk, i - 24);
            arkaGrid.Children.Add(koltuk);
        }
        SeatsContainer.Children.Add(arkaGrid);
    }

    private Grid YeniSira() => new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // sol tek
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // koridor
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // sağ pencere
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // sağ koridor
        },
        ColumnSpacing = 8
    };

    // Orta kapı & acil çıkış bandı
    private View OrtaKapiBandi()
    {
        var border = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            BackgroundColor = (Color)Application.Current.Resources["BgSurfaceContainer"],
            Padding = new Thickness(10, 6)
        };
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            }
        };
        var sol = new Label
        {
            Text = "⛞  ORTA KAPI & ACİL ÇIKIŞ",
            FontFamily = "InterBold",
            FontSize = 11,
            TextColor = (Color)Application.Current.Resources["BgSecondary"],
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(sol, 0);
        grid.Children.Add(sol);
        border.Content = grid;
        return border;
    }

    // İkram & WC / bagaj bilgi bandı
    private View BilgiBandi()
    {
        var border = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            BackgroundColor = (Color)Application.Current.Resources["BgSurfaceContainer"],
            Padding = new Thickness(10, 6)
        };
        border.Content = new Label
        {
            Text = "İkram & WC  •  Geniş Diz Mesafesi",
            FontFamily = "InterMedium",
            FontSize = 11,
            TextColor = (Color)Application.Current.Resources["BgOnSurfaceVariant"],
            HorizontalOptions = LayoutOptions.Center
        };
        return border;
    }

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

        content.Children.Add(new Label
        {
            Text = seat.Number.ToString(),
            FontFamily = "InterBold",
            FontSize = 12,
            TextColor = KoltukYaziRengi(seat),
            HorizontalOptions = LayoutOptions.Center
        });

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

    private async void KoltugaTiklandi(Seat seat)
    {
        // Cinsiyet kuralı uyarısı: yanı dolu çift koltuk mu?
        if (seat.Status == SeatStatus.Empty)
        {
            var komsu = CiftKomsu(seat.Number);
            if (komsu != null && (komsu.Status == SeatStatus.Male || komsu.Status == SeatStatus.Female))
            {
                string cinsiyet = komsu.Status == SeatStatus.Female ? "kadın" : "erkek";
                await DisplayAlert("Cinsiyet Kuralı",
                    $"Yanınızdaki {komsu.Number} numaralı koltukta {cinsiyet} yolcu var. " +
                    $"Bu koltuk yalnızca {cinsiyet} yolcu içindir.",
                    "Anladım");
                // Uyarı verdik ama yine de seçime izin veriyoruz
            }
        }

        if (_selectedSeat == seat)
        {
            seat.Status = SeatStatus.Empty;
            _selectedSeat = null;
        }
        else
        {
            if (_selectedSeat != null)
                _selectedSeat.Status = SeatStatus.Empty;

            seat.Status = SeatStatus.Selected;
            _selectedSeat = seat;
        }

        CizKoltuklar();
        OzetGuncelle();
    }

    private void OzetGuncelle()
    {
        // Kalan boş koltuk sayısı
        int bos = _seats.Count(s => s.Status == SeatStatus.Empty);
        RemainingSeatsLabel.Text = $"{bos} boş koltuk";

        if (_selectedSeat == null)
        {
            SelectedSeatLabel.Text = "Koltuk Seçiniz";
            TotalPriceLabel.Text = "0 TL";
        }
        else
        {
            SelectedSeatLabel.Text = $"No: {_selectedSeat.Number} ({KoltukTipi(_selectedSeat.Number)})";
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

        await DisplayAlert("Seçim",
            $"{_selectedSeat.Number} numaralı koltuk ({KoltukTipi(_selectedSeat.Number)}) seçildi. (Sonraki adım eklenecek.)",
            "Tamam");
    }
}
