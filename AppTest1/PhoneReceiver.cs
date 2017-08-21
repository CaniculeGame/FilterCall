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
using Android.Provider;
using Android.Telephony;
using Android.Util;
using Android.Media;
using System.Threading.Tasks;

namespace AppTest1
{
    [BroadcastReceiver]
    [Android.App.IntentFilter(new[] { SMS_RECEIVER })]
    class PhoneReceiver : BroadcastReceiver
    {
        private const string SMS_RECEIVER = "android.provider.Telephony.SMS_RECEIVED";
        private static int MAX_SMS_MESSAGE_LENGTH = 160;
        private bool desactiveSon = true; //permet de savoir si le tel est en silencieux, s'il est en silencieux, il ne faut pas  le remettre en normal

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == SMS_RECEIVER)
            {
                SmsMessage[] msgs = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
                Log.Info("info", "number = " + msgs[0].OriginatingAddress);
                desactiveSon = true;

                if (true) //TODO : chercher si dans liste noire
                {
                    AudioManager aM = (AudioManager)context.GetSystemService(Context.AudioService);
                    if (aM.RingerMode != RingerMode.Silent)
                        aM.RingerMode = RingerMode.Silent;
                    else
                        desactiveSon = false;

                    string reponse = "Une reponse"; //TODO : recupere le message dans la bonne liste noir
                    SmsManager sms = SmsManager.Default;
                    if (reponse.Length > MAX_SMS_MESSAGE_LENGTH)
                    {
                        var messagelist = sms.DivideMessage(reponse);
                        sms.SendMultipartTextMessage(msgs[0].OriginatingAddress, null, messagelist, null, null);
                    }
                    else
                    {
                        sms.SendTextMessage(msgs[0].OriginatingAddress, null, reponse, null, null);
                    }


                    Sleep(aM,context);
                }

            }

        }

        public async void Sleep(AudioManager aM, Context context)
        {
            if (desactiveSon)
            {
                await Task.Delay(300);
                aM.RingerMode = RingerMode.Normal;
            }
        }
    }
}