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
    [Activity(Label = "SettingActivity")]
    public class SettingActivity : Activity
    {
        Button saveButton = null;
        Button deleteButton = null;
        LinearLayout deleteLayout = null;
        Button cancelButton = null;
        EditText titre = null;
        TimePicker hourEnd = null;
        TimePicker hourStart = null;
        EditText message = null;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);

            // Create your application here
            SetContentView(Resource.Layout.SettingPage);


            titre = FindViewById<EditText>(Resource.Id.TitleTextEdit);
            hourEnd = FindViewById<TimePicker>(Resource.Id.timePickerEnd);
            hourStart = FindViewById<TimePicker>(Resource.Id.timePickerStart);
            message = FindViewById<EditText>(Resource.Id.editTextMessage);



            saveButton = FindViewById<Button>(Resource.Id.SaveButton);
            if (saveButton != null)
            {
                saveButton.Click += delegate
                {
                    Save();
                    ChangeActivity();
                };
            }

            deleteButton = FindViewById<Button>(Resource.Id.DelButton);
            if (deleteButton != null)
            {
                if (Global.Instance.Position == Global.InvalideValue)
                {
                    deleteButton.Enabled = false;
                    deleteButton.SetCompoundDrawablesWithIntrinsicBounds(null,GetDrawable(Resource.Drawable.garbageDisable),null,null);
                }
                else
                {
                    deleteButton.Enabled = true;
                    deleteButton.Click += delegate
                     {

                         Delete();
                         ChangeActivity();
                     };
                }
            }


            cancelButton = FindViewById<Button>(Resource.Id.CancelButton);
            if (cancelButton != null)
            {
                cancelButton.Click += delegate
                {
                    ChangeActivity();
                };
            }



            if (Global.Instance.Position != Global.InvalideValue)
            {
                if (Global.Instance.getList.Count > Global.Instance.Position && Global.Instance.Position >= 0)
                {
                    titre.Text = Global.Instance.getElement(Global.Instance.Position).Titre;
                    hourEnd.Hour = Global.Instance.getElement(Global.Instance.Position).HourEnd.Hour;
                    hourEnd.Minute = Global.Instance.getElement(Global.Instance.Position).HourEnd.Minute;
                    hourStart.Hour = Global.Instance.getElement(Global.Instance.Position).HourStart.Hour;
                    hourStart.Minute = Global.Instance.getElement(Global.Instance.Position).HourStart.Minute;
                    message.Text = Global.Instance.getElement(Global.Instance.Position).MessageText;
                }
                else
                {
                    titre.Text = "no title";
                    hourEnd.Hour = 0;
                    hourStart.Hour = 0;
                    hourEnd.Minute = 0;
                    hourStart.Minute = 0;
                    message.Text = GetString(Resource.String.Message);
                }
            }
            else
            {
                titre.Text = "no title";
                hourEnd.Hour = 0;
                hourStart.Hour = 0;
                hourEnd.Minute = 0;
                hourStart.Minute = 0;
                message.Text = GetString(Resource.String.Message);
            }

        }


        private void Save()
        {
            DateTime timeEnd = new DateTime(2017, 01, 01, hourEnd.Hour, hourEnd.Minute, 0);
            DateTime timeStart = new DateTime(2017, 01, 01, hourStart.Hour, hourStart.Minute, 0);

            ListModel newItem = new ListModel(true, titre.Text, message.Text, null, timeStart, timeEnd,null);
            Global.Instance.Add(newItem);
            Global.Instance.Save();
        }

        private void Delete()
        {
            if (Global.Instance.Position != Global.InvalideValue && Global.Instance.Position >= 0)
                Global.Instance.getList.RemoveAt(Global.Instance.Position);
        }


        private void ChangeActivity()
        {
            StartActivity(typeof(MainActivity));
        }
    }
}