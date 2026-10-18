
namespace SuDengem
{
    public partial class TakipPage : ContentPage
    {
        int miktar = 200;
        bool hatirlaticiHazir = false;

        public TakipPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Guncelle();
            ButonlariBoya(SmallButton);

            hatirlaticiHazir = false;
            ReminderPicker.SelectedIndex = Preferences.Get("Hatirlatma", 0);
            hatirlaticiHazir = true;
        }

        private void Guncelle()
        {
            int icilen = Kayitlar.Su(Kayitlar.Bugun);
            int hedef = Kayitlar.Hedef;

            WelcomeLabel.Text = "Merhaba, " + Kayitlar.Ad;
            DrankLabel.Text = icilen + " ml";
            GoalLabel.Text = "Günlük hedef: " + hedef + " ml";

            if (hedef > 0)
            {
                double oran = (double)icilen / hedef;
                WaterProgress.Progress = Math.Min(1, oran);

                if (oran >= 1)
                    ProgressLabel.Text = "Günlük hedefine ulaştın";
                else
                    ProgressLabel.Text = "%" + (int)(oran * 100) + " tamamlandı";

                RemainingLabel.Text = Math.Max(0, hedef - icilen) + " ml kaldı";
            }

            UndoButton.IsEnabled = Kayitlar.SonEklenen > 0;
            MiktariGuncelle();
        }

        private void MiktariGuncelle()
        {
            if (TotalLabel == null)
                return;

            int adet = (int)WaterStepper.Value;

            AmountLabel.Text = miktar + " ml seçildi";
            TotalLabel.Text = adet + " adet = " + (miktar * adet) + " ml";
        }

        private void ButonlariBoya(Button secilen)
        {
            Button[] butonlar = {
                SmallButton, LargeButton, BottleButton, CustomButton
            };

            foreach (Button buton in butonlar)
            {
                buton.BackgroundColor = Color.FromArgb("#EAF6FC");
                buton.TextColor = Color.FromArgb("#1686C9");
            }

            secilen.BackgroundColor = Color.FromArgb("#1686C9");
            secilen.TextColor = Colors.White;
        }

        private void OnChooseClicked(object sender, EventArgs e)
        {
            Button secilen = (Button)sender;
            int secim = int.Parse(secilen.CommandParameter.ToString()!);

            AmountSlider.IsVisible = secim == 0;

            if (secim == 0)
                miktar = (int)(Math.Round(AmountSlider.Value / 50) * 50);
            else
                miktar = secim;

            ButonlariBoya(secilen);
            MiktariGuncelle();
        }

        private void OnSliderChanged(object sender, ValueChangedEventArgs e)
        {
            if (AmountSlider == null || !AmountSlider.IsVisible)
                return;

            miktar = (int)(Math.Round(e.NewValue / 50) * 50);
            MiktariGuncelle();
        }

        private void OnStepperChanged(object sender, ValueChangedEventArgs e)
        {
            if (TotalLabel == null)
                return;

            MiktariGuncelle();
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            int toplam = miktar * (int)WaterStepper.Value;
            int bugunkuSu = Kayitlar.Su(Kayitlar.Bugun);

            if (toplam >= 2000 || bugunkuSu + toplam > 4000)
            {
                bool onay = await DisplayAlertAsync(
                    "Su miktarını kontrol et",
                    "Yüksek miktarda su kaydediyorsun. Bu miktarı gerçekten içtin mi?",
                    "Evet, kaydet", "Vazgeç");

                if (!onay)
                    return;
            }

            Kayitlar.SuEkle(toplam);
            WaterStepper.Value = 1;
            Guncelle();
        }

        private async void OnUndoClicked(object sender, EventArgs e)
        {
            if (Kayitlar.SonEklenen == 0)
                return;

            bool onay = await DisplayAlertAsync(
                "Son kaydı geri al",
                "Son eklenen suyu geri almak istiyor musun?",
                "Geri al", "Vazgeç");

            if (!onay)
                return;

            Kayitlar.GeriAl();
            Guncelle();
        }

        private async void OnResetClicked(object sender, EventArgs e)
        {
            bool onay = await DisplayAlertAsync(
                "Bugünkü kaydı sıfırla",
                "Bugünkü su kayıtların silinecek. Emin misin?",
                "Evet, sıfırla", "Vazgeç");

            if (!onay)
                return;

            Kayitlar.BugunuSifirla();
            Guncelle();
        }

        private async void OnReminderChanged(object sender, EventArgs e)
        {
            if (!hatirlaticiHazir)
                return;

            int secim = ReminderPicker.SelectedIndex;

            if (secim < 0)
                return;

            int dakika = 0;

            if (secim == 1)
                dakika = 60;
            else if (secim == 2)
                dakika = 90;

#if ANDROID
            if (dakika > 0)
            {
                PermissionStatus izin =
                    await Permissions.RequestAsync<Permissions.PostNotifications>();

                if (izin != PermissionStatus.Granted)
                {
                    await DisplayAlertAsync(
                        "Bildirim izni",
                        "Bildirim göndermek için izin vermelisin.",
                        "Tamam");

                    hatirlaticiHazir = false;
                    ReminderPicker.SelectedIndex = 0;
                    hatirlaticiHazir = true;

                    Hatirlatici.Ayarla(0);
                    Preferences.Set("Hatirlatma", 0);
                    return;
                }
            }

            Hatirlatici.Ayarla(dakika);
#endif

            Preferences.Set("Hatirlatma", secim);
        }

        private async void OnTestNotificationClicked(object sender, EventArgs e)
        {
#if ANDROID
            PermissionStatus izin =
                await Permissions.RequestAsync<Permissions.PostNotifications>();

            if (izin != PermissionStatus.Granted)
            {
                await DisplayAlertAsync(
                    "Bildirim izni",
                    "Önce bildirim iznine onay vermelisin.",
                    "Tamam");
                return;
            }

            Hatirlatici.TestBildirimi();
#else
            await DisplayAlertAsync(
                "Bilgi",
                "Bu bildirim testi Android telefon içindir.",
                "Tamam");
#endif
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("GecmisPage");
        }

        private async void OnSwitchClicked(object sender, EventArgs e)
        {
            MainPage.KullaniciSecilecek = true;
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
