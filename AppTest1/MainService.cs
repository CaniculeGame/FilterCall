using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using System.Reflection.Emit;

namespace AppTest1
{

    [Service]
    public class MainService : IntentService
    {

        bool isStarted = false;

        public IBinder Binder { get; private set; }

        public override void OnCreate()
        {
            // This method is optional to implement
            base.OnCreate();

            Log.Debug("infoService", "OnCreate");
        }

        public override IBinder OnBind(Intent intent)
        {
            // This method must always be implemented
            Log.Debug("infoService", "OnBind");
            return null;
        }

        public override bool OnUnbind(Intent intent)
        {
            // This method is optional to implement
            Log.Debug("infoService", "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            // This method is optional to implement
            Log.Debug("infoService", "OnDestroy");
            Binder = null;
            base.OnDestroy();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return base.OnStartCommand(intent, flags, startId);
        }

        protected override void OnHandleIntent(Intent intent)
        {
            if (!isStarted)
            {
                const int pendingIntentId = 0;
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, new Intent(this, typeof(MainActivity)), PendingIntentFlags.OneShot);


                Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle(GetString(Resource.String.ApplicationName))
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetContentIntent(pendingIntent)
                .SetContentText(GetString(Resource.String.appLancee));

                Notification notification = builder.Build();

                NotificationManager notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
                isStarted = true;

                var global = Global.Instance;
                global.LoadPref(this);
            }



        }
    }
}

