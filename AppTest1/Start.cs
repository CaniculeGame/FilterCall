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

            if (intent.Action == Intent.ActionBootCompleted)
            {
                Global.Instance.LoadPref(context);
                if (Global.Instance.BootStart)
                {
                    Intent myIntent = new Intent(context, typeof(MainService));
                    context.StartService(myIntent);
                }
            }
        }

    }
}