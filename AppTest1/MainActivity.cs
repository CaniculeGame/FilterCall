using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.Collections.Generic;
using Android.Util;
using Android.Content;

namespace AppTest1
{
    [Activity(Label = "AppTest1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        Button addButton  = null;
        Button menuButton = null;
        ListView mainListView = null;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);

            SetContentView(Resource.Layout.Main);

            Global global = Global.Instance;

            addButton = FindViewById<Button>(Resource.Id.AddButton);
            if (addButton != null)
            {
                addButton.Click += delegate
                {
                    OnButtonClicked();
                };
            }


            menuButton = FindViewById<Button>(Resource.Id.buttonMenu);
            if (menuButton != null)
            {
                menuButton.Click += delegate
                {
                    OnButtonMenuClicked();
                };
            }



            mainListView = FindViewById<ListView>(Resource.Id.MainListView);
            if (mainListView != null)
            {
                mainListView.Adapter = new ListAdapter(this, Global.Instance.GetList);
                mainListView.ItemClick += OnListItemClick;
            }


            Intent myIntent = new Intent(this, typeof(MainService));
            StartService(myIntent);
        }


        private void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Global.Instance.Position = e.Position;
            StartActivity(typeof(SettingActivity));
        }

        private void OnButtonClicked()
        {
            Global.Instance.Position = Global.InvalideValue;
            StartActivity(typeof(SettingActivity));
            Finish();
        }

        private void OnButtonMenuClicked()
        {
            Global.Instance.Position = Global.InvalideValue;
            StartActivity(typeof(Menu));
        }
    }
}
