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

namespace AppTest1
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted },
    Categories = new[] { Android.Content.Intent.CategoryDefault })]
    class Start : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //Log.Info("info", "blou bloup bloup");

            if (intent.Action == Intent.ActionBootCompleted)
            {
                Intent myIntent = new Intent(context, typeof(MainService));
                context.StartService(myIntent);

                //Intent myIntent = new Intent(context, typeof(MainActivity));
                //context.StartActivity(myIntent);     
            }
        }

    }
}