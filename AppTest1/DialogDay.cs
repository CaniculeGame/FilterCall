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
    class DialogDay : DialogFragment
    {

        bool[] isDaychecked = new bool[7] { false, false, false, false, false, false, false };

        public event EventHandler<DialogEventArgs> DialogClosed;

        public static DialogDay NewInstance(Bundle bundle)
        {
            DialogDay fragment = new DialogDay();
            fragment.Arguments = bundle;
            return fragment;
        }


        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);

            string[] days = new string[7] {GetString(Resource.String.Lundi),
                GetString(Resource.String.Mardi),
                GetString(Resource.String.Mercredi),
                GetString(Resource.String.Jeudi),
                GetString(Resource.String.Vendredi),
                GetString(Resource.String.Samedi),
                GetString(Resource.String.Dimanche)
            };


            builder.SetTitle(Resource.String.dialogTitle);

            builder.SetMultiChoiceItems(days, isDaychecked, (s, e) =>
            {
                isDaychecked[e.Which] = e.IsChecked;
            });



            builder.SetNeutralButton(Resource.String.valider, (sender, args) =>
                                              {
                                             /*     if (!Global.Instance.isInvalidPosition)
                                                  {
                                                      Global.Instance.SetDay(Global.Instance.Position,isDaychecked);
                                                      Global.Instance.Save();
                                                  }*/
                                              });

            return builder.Create();
        }



        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            if (DialogClosed != null)
            {
                Log.Info("info", "dismiss");
                DialogClosed(this, new DialogEventArgs { ReturnValue = isDaychecked });
            }

        }
    }
}