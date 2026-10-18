
namespace SuDengem
{
    public static class Kayitlar
    {
        public static string AktifId
        {
            get => Preferences.Get("AktifId", "");
            set => Preferences.Set("AktifId", value);
        }

        public static string[] Kullanicilar =>
            Preferences.Get("Kullanicilar", "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries);

        public static string Ad =>
            Preferences.Get(AktifId + "_Ad", "");

        public static int Hedef =>
            Preferences.Get(AktifId + "_Hedef", 0);

        public static string Bugun =>
            DateTime.Today.ToString("yyyy-MM-dd");

        public static int Su(string tarih)
        {
            return Preferences.Get(AktifId + "_Su_" + tarih, 0);
        }

        public static bool KayitVar(string tarih)
        {
            return Preferences.ContainsKey(AktifId + "_Su_" + tarih);
        }

        public static int GunHedefi(string tarih)
        {
            return Preferences.Get(AktifId + "_Hedef_" + tarih, 0);
        }

        public static void YeniKullanici(string ad, int kilo)
        {
            string id = Guid.NewGuid().ToString("N");
            string liste = Preferences.Get("Kullanicilar", "");

            if (liste != "")
                liste += ",";

            Preferences.Set("Kullanicilar", liste + id);
            Preferences.Set(id + "_Ad", ad);
            Preferences.Set(id + "_Hedef", kilo * 30);

            AktifId = id;
        }

        public static void SuEkle(int miktar)
        {
            string tarih = Bugun;
            int toplam = Su(tarih) + miktar;

            Preferences.Set(AktifId + "_Su_" + tarih, toplam);
            Preferences.Set(AktifId + "_Hedef_" + tarih, Hedef);
            Preferences.Set(AktifId + "_Son_" + tarih, miktar);

            TarihiKaydet(tarih);
        }

        public static int SonEklenen =>
            Preferences.Get(AktifId + "_Son_" + Bugun, 0);

        public static void GeriAl()
        {
            if (SonEklenen == 0)
                return;

            int toplam = Math.Max(0, Su(Bugun) - SonEklenen);

            Preferences.Set(AktifId + "_Su_" + Bugun, toplam);
            Preferences.Set(AktifId + "_Son_" + Bugun, 0);
        }

        public static void BugunuSifirla()
        {
            Preferences.Set(AktifId + "_Su_" + Bugun, 0);
            Preferences.Set(AktifId + "_Son_" + Bugun, 0);
            Preferences.Set(AktifId + "_Hedef_" + Bugun, Hedef);

            TarihiKaydet(Bugun);
        }

        private static void TarihiKaydet(string tarih)
        {
            string anahtar = AktifId + "_Tarihler";
            string liste = Preferences.Get(anahtar, "");

            string[] tarihler = liste.Split(',',
                StringSplitOptions.RemoveEmptyEntries);

            if (!tarihler.Contains(tarih))
            {
                if (liste != "")
                    liste += ",";

                Preferences.Set(anahtar, liste + tarih);
            }
        }

        public static void KullaniciSil(string id)
        {
            string liste = "";

            foreach (string kullaniciId in Kullanicilar)
            {
                if (kullaniciId != id)
                {
                    if (liste != "")
                        liste += ",";

                    liste += kullaniciId;
                }
            }

            Preferences.Set("Kullanicilar", liste);

            string[] tarihler = Preferences.Get(id + "_Tarihler", "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (string tarih in tarihler)
            {
                Preferences.Remove(id + "_Su_" + tarih);
                Preferences.Remove(id + "_Hedef_" + tarih);
                Preferences.Remove(id + "_Son_" + tarih);
            }

            // Önceki sürümlerdeki kayıtları da temizle
            for (int i = 0; i < 365; i++)
            {
                string tarih = DateTime.Today.AddDays(-i)
                    .ToString("yyyy-MM-dd");

                Preferences.Remove(id + "_Su_" + tarih);
                Preferences.Remove(id + "_Hedef_" + tarih);
                Preferences.Remove(id + "_Son_" + tarih);
            }

            Preferences.Remove(id + "_Tarihler");
            Preferences.Remove(id + "_Ad");
            Preferences.Remove(id + "_Hedef");

            if (AktifId == id)
                AktifId = "";
        }

        public static void EskiKaydiAktar()
        {
            if (Preferences.Get("Aktarildi", false))
                return;

            if (!Preferences.Get("KayitTamam", false))
            {
                Preferences.Set("Aktarildi", true);
                return;
            }

            string ad = Preferences.Get("KullaniciAdi", "Kullanıcı");
            int hedef = Preferences.Get("SuHedefi", 0);

            YeniKullanici(ad, 0);
            Preferences.Set(AktifId + "_Hedef", hedef);

            for (int i = 0; i < 7; i++)
            {
                string tarih = DateTime.Today.AddDays(-i)
                    .ToString("yyyy-MM-dd");

                if (Preferences.ContainsKey("Su_" + tarih))
                {
                    Preferences.Set(AktifId + "_Su_" + tarih,
                        Preferences.Get("Su_" + tarih, 0));

                    TarihiKaydet(tarih);
                }

                if (Preferences.ContainsKey("Hedef_" + tarih))
                {
                    Preferences.Set(AktifId + "_Hedef_" + tarih,
                        Preferences.Get("Hedef_" + tarih, hedef));
                }
            }

            if (Preferences.Get("GunlukTarih", "") == Bugun)
            {
                string anahtar = AktifId + "_Su_" + Bugun;

                if (!Preferences.ContainsKey(anahtar))
                {
                    Preferences.Set(anahtar,
                        Preferences.Get("IcilenSu", 0));

                    TarihiKaydet(Bugun);
                }

                Preferences.Set(AktifId + "_Son_" + Bugun,
                    Preferences.Get("SonEklenen", 0));
            }

            Preferences.Set("Aktarildi", true);
        }
    }
}
