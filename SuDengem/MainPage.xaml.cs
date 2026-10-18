
namespace SuDengem
{
    public partial class MainPage : ContentPage
    {
        public static bool KullaniciSecilecek = false;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Kayitlar.EskiKaydiAktar();
            KullanicilariGoster();

            ReturnButton.IsVisible = Kayitlar.AktifId != "";
            KullaniciSecilecek = false;
        }

        private void KullanicilariGoster()
        {
            UsersLayout.Children.Clear();

            foreach (string id in Kayitlar.Kullanicilar)
            {
                string ad = Preferences.Get(id + "_Ad", "");

                Button sec = new Button
                {
                    Text = ad,
                    BackgroundColor = Color.FromArgb("#EAF6FC"),
                    TextColor = Color.FromArgb("#1686C9")
                };

                sec.Clicked += async (s, e) =>
                {
                    Kayitlar.AktifId = id;
                    await Shell.Current.GoToAsync("//TakipPage");
                };

                Button sil = new Button
                {
                    Text = "Sil",
                    BackgroundColor = Color.FromArgb("#FCEAEA"),
                    TextColor = Color.FromArgb("#C04A4A"),
                    WidthRequest = 70
                };

                sil.Clicked += async (s, e) =>
                {
                    bool onay = await DisplayAlertAsync(
                        "Kullanıcı sil",
                        ad + " ve kayıtları silinsin mi?",
                        "Evet, sil", "Vazgeç");

                    if (!onay)
                        return;

                    Kayitlar.KullaniciSil(id);
                    KullanicilariGoster();

                    ReturnButton.IsVisible = Kayitlar.AktifId != "";
                };

                Grid satir = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    },
                    ColumnSpacing = 10
                };

                satir.Add(sec, 0, 0);
                satir.Add(sil, 1, 0);

                UsersLayout.Children.Add(satir);
            }
        }

        private async void OnCreateClicked(object sender, EventArgs e)
        {
            string ad = NameEntry.Text?.Trim() ?? "";

            if (ad == "" ||
                !int.TryParse(WeightEntry.Text, out int kilo))
            {
                ResultLabel.Text = "Adını ve geçerli bir kilo gir.";
                return;
            }

            if (kilo < 30 || kilo > 250)
            {
                ResultLabel.Text = "Kilo 30 ile 250 arasında olmalı.";
                return;
            }

            Kayitlar.YeniKullanici(ad, kilo);

            NameEntry.Text = "";
            WeightEntry.Text = "";
            ResultLabel.Text = "";

            await Shell.Current.GoToAsync("//TakipPage");
        }

        private async void OnReturnClicked(object sender, EventArgs e)
        {
            if (Kayitlar.AktifId != "")
                await Shell.Current.GoToAsync("//TakipPage");
        }
    }
}
