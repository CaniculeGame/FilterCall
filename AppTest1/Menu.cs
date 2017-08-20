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

namespace AppTest1
{
    [Activity(Label = "Menu")]
    public class Menu : Activity
    {

        Switch bootStartButton = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu);

            bootStartButton = FindViewById<Switch>(Resource.Id.switchStartBoot);
            if (bootStartButton != null)
            {

                bootStartButton.Checked =  Global.Instance.BootStart;
                bootStartButton.Click += delegate
                {
                    OnButtonBootStartClicked();
                };
            }
        }

        private void OnButtonBootStartClicked()
        {
            Global.Instance.BootStart = !Global.Instance.BootStart;
            Global.Instance.SavePref(this);
        }
    }
}