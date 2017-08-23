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
using Android.Net.Sip;

namespace AppTest1
{
    [BroadcastReceiver]
    [Android.App.IntentFilter(new[] { SMS_RECEIVER, CALL_RECEIVER })]
    class PhoneReceiver : BroadcastReceiver
    {

        private const string SMS_RECEIVER = "android.provider.Telephony.SMS_RECEIVED";
        private const string CALL_RECEIVER = "android.intent.action.PHONE_STATE";
        private static int MAX_SMS_MESSAGE_LENGTH = 160;
        private bool desactiveSon = true; //permet de savoir si le tel est en silencieux, s'il est en silencieux, il ne faut pas  le remettre en normal

        public override void OnReceive(Context context, Intent intent)
        {
            short idList = 0;
            if (intent.Action == CALL_RECEIVER)
            {
                if (intent.GetStringExtra(TelephonyManager.ExtraState).Equals(TelephonyManager.ExtraStateRinging))
                {
                    string number = intent.GetStringExtra(TelephonyManager.ExtraIncomingNumber);
                    if (Global.Instance.SearchPhoneNumber(number, out idList))
                    {
                        execute(context, number, idList);

                        TelephonyManager mng = (TelephonyManager)(context.GetSystemService(Context.TelephonyService));
                        IntPtr iTelephonyPtr = JNIEnv.GetMethodID(mng.Class.Handle, "getITelephony", "()Lcom/android/internal/telephony/ITelephony;");

                        IntPtr telephony = JNIEnv.CallObjectMethod(mng.Handle, iTelephonyPtr);
                        IntPtr iTelephonyClass = JNIEnv.GetObjectClass(telephony);
                        IntPtr iTelephonyEndCall = JNIEnv.GetMethodID(iTelephonyClass, "endCall", "()Z");
                        JNIEnv.CallBooleanMethod(telephony, iTelephonyEndCall);
                        JNIEnv.DeleteLocalRef(telephony);
                        JNIEnv.DeleteLocalRef(iTelephonyClass);

                    }
                }
            }
            else if (intent.Action == SMS_RECEIVER)
            {
                SmsMessage[] msgs = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
                Log.Info("info", "number = " + msgs[0].OriginatingAddress);
                if (Global.Instance.SearchPhoneNumber(msgs[0].OriginatingAddress, out idList))
                {
                    execute(context, msgs[0].OriginatingAddress, idList);
                }
            }
        }

        private void execute(Context context, string numberDest, int idLstRep = 0)
        {
            desactiveSon = true;

            AudioManager aM = (AudioManager)context.GetSystemService(Context.AudioService);
            if (aM.RingerMode != RingerMode.Silent)
                aM.RingerMode = RingerMode.Silent;
            else
                desactiveSon = false;

            string reponse = Global.Instance.GetElement(idLstRep).MessageText;
            SmsManager sms = SmsManager.Default;
            if (reponse.Length > MAX_SMS_MESSAGE_LENGTH)
            {
                var messagelist = sms.DivideMessage(reponse);
                sms.SendMultipartTextMessage(numberDest, null, messagelist, null, null);
            }
            else
            {
                sms.SendTextMessage(numberDest, null, reponse, null, null);
            }

            Sleep(aM, context);
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