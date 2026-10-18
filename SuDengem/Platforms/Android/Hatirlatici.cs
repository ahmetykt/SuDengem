
using Android.App;
using Android.Content;
using Android.OS;

namespace SuDengem
{
    public static class Hatirlatici
    {
        public static void Ayarla(int dakika)
        {
            Context context = Android.App.Application.Context;

            AlarmManager alarm = (AlarmManager)
                context.GetSystemService(Context.AlarmService)!;

            Intent intent = new Intent(context, typeof(SuBildirimReceiver));

            PendingIntent bekleyen = PendingIntent.GetBroadcast(
                context, 1, intent,
                PendingIntentFlags.UpdateCurrent |
                PendingIntentFlags.Immutable)!;

            alarm.Cancel(bekleyen);

            if (dakika == 0)
                return;

            long aralik = dakika * 60L * 1000L;
            long ilkZaman = SystemClock.ElapsedRealtime() + aralik;

            alarm.SetInexactRepeating(
                AlarmType.ElapsedRealtimeWakeup,
                ilkZaman, aralik, bekleyen);
        }

        public static void TestBildirimi()
        {
            SuBildirimReceiver.BildirimGonder(
                Android.App.Application.Context);
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class SuBildirimReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context != null)
                BildirimGonder(context);
        }

        public static void BildirimGonder(Context context)
        {
            const string kanalId = "su_hatirlatma";

            NotificationManager yonetici = (NotificationManager)
                context.GetSystemService(Context.NotificationService)!;

            NotificationChannel kanal = new NotificationChannel(
                kanalId,
                "Su hatırlatıcıları",
                NotificationImportance.Default);

            yonetici.CreateNotificationChannel(kanal);

            Notification bildirim =
                new Notification.Builder(context, kanalId)
                    .SetContentTitle("SuDengem")
                    .SetContentText("Biraz su içmeye ne dersin?")
                    .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                    .SetAutoCancel(true)
                    .Build();

            yonetici.Notify(1001, bildirim);
        }
    }
}
