
using System.Globalization;

namespace SuDengem
{
    public partial class GecmisPage : ContentPage
    {
        public GecmisPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GecmisiGoster();
        }

        private void GecmisiGoster()
        {
            HistoryLayout.Children.Clear();

            for (int i = 0; i < 7; i++)
            {
                DateTime gun = DateTime.Today.AddDays(-i);
                string tarih = gun.ToString("yyyy-MM-dd");
                string tarihYazisi = gun.ToString(
                    "dd MMMM dddd", new CultureInfo("tr-TR"));

                int su = Kayitlar.Su(tarih);
                int hedef = Kayitlar.GunHedefi(tarih);

                string durum = "Kayıt bulunmuyor";

                if (Kayitlar.KayitVar(tarih))
                {
                    durum = su + " ml içildi";

                    if (hedef > 0)
                    {
                        if (su >= hedef)
                            durum += " - Hedef tamamlandı";
                        else
                            durum += " / Hedef: " + hedef + " ml";
                    }
                }

                Label gunLabel = new Label
                {
                    Text = tarihYazisi,
                    FontSize = 17,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#183B56")
                };

                Label suLabel = new Label
                {
                    Text = durum,
                    FontSize = 14,
                    TextColor = Color.FromArgb("#1686C9")
                };

                VerticalStackLayout icerik = new VerticalStackLayout
                {
                    Spacing = 7
                };

                icerik.Children.Add(gunLabel);
                icerik.Children.Add(suLabel);

                Border kart = new Border
                {
                    BackgroundColor = Colors.White,
                    StrokeThickness = 0,
                    Padding = new Thickness(18),
                    Content = icerik
                };

                HistoryLayout.Children.Add(kart);
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
