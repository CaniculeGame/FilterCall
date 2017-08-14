using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.Collections.Generic;
using Android.Util;

namespace AppTest1
{
    [Activity(Label = "AppTest1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        Button addButton = null;
        ListView mainListView = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // SetContentView (Resource.Layout.Main);

            Global global = Global.Instance;

            addButton = FindViewById<Button>(Resource.Id.AddButton);
            if (addButton != null)
            {
                addButton.Click += delegate
                {
                    OnButtonClicked();
                };
            }


            mainListView = FindViewById<ListView>(Resource.Id.MainListView);
            if(mainListView != null)
            {
                mainListView.Adapter = new ListAdapter(this, Global.Instance.getList);
                mainListView.ItemClick += OnListItemClick;
            }


        }


        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Global.Instance.Position=e.Position;
            StartActivity(typeof(SettingActivity));
        }

        void OnButtonClicked()
        {
            Global.Instance.Position = Global.InvalideValue;
            StartActivity(typeof(SettingActivity));
            Finish();
        }
    }
}

