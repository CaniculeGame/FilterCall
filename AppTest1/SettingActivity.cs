﻿using System;
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
    [Activity(Label = "SettingActivity")]
    public class SettingActivity : Activity
    {
        Button saveButton = null;
        Button deleteButton = null;
        bool[] tabBool = null; // tableau de boolean contenant les dates d'applications
        Button cancelButton = null;
        Button choseDate = null;
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


            choseDate = FindViewById<Button>(Resource.Id.dayButton);
            if (choseDate != null)
            {
                if (Global.Instance.isInvalidPosition)
                    choseDate.Text = GetString(Resource.String.once);
                else
                {
                    tabBool = Global.Instance.getElement(Global.Instance.Position).Days;
                    ChooseDateString(tabBool);
                }

                choseDate.Click += delegate
                {
                    choseDialog();
                };
            }

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
                    deleteButton.SetCompoundDrawablesWithIntrinsicBounds(null, GetDrawable(Resource.Drawable.garbageDisable), null, null);
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

        private void choseDialog()
        {

            FragmentTransaction ft = FragmentManager.BeginTransaction();
            Fragment prev = FragmentManager.FindFragmentByTag("dialog");
            if (prev != null)
            {
                ft.Remove(prev);
            }

            ft.AddToBackStack(null);

            DialogDay newFragment = DialogDay.NewInstance(null);
            newFragment.DialogClosed += OnDialogClosed;
            //Add fragment
            newFragment.Show(ft, "dialog");
        }

        void OnDialogClosed(object sender, DialogEventArgs e)
        {
            tabBool = e.ReturnValue;
            ChooseDateString(tabBool);
        }

        private void Save()
        {
            DateTime timeEnd = new DateTime(2017, 01, 01, hourEnd.Hour, hourEnd.Minute, 0);
            DateTime timeStart = new DateTime(2017, 01, 01, hourStart.Hour, hourStart.Minute, 0);

            ListModel newItem = null;
            if (tabBool != null)
                newItem = new ListModel(true, titre.Text, message.Text, null, timeStart, timeEnd, tabBool);
            else
                newItem = new ListModel(true, titre.Text, message.Text, null, timeStart, timeEnd, new bool[] { false,false,false,false,false,false,false});

            if (Global.Instance.isInvalidPosition)
                Global.Instance.Add(newItem);
            else
                Global.Instance.setElement(Global.Instance.Position,newItem);

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
            Finish();
        }



        private void ChooseDateString(bool[] tab)
        {
            short count = 0;
            bool weekdays = true;
            string mess = "";

            if (tab != null)
            {
                for (int i = 0; i < tab.Length; i++)
                {
                    //compte le nombre de jour validé, si 0 = once, si 7 = all, si 5 = week day
                    if (tab[i] == true)
                    {
                        if(i != 0)
                            mess += ",";

                        count++;
                        switch (i)
                        {
                            case 0:
                                mess = mess + GetString(Resource.String.short_Lundi);
                                break;

                            case 1:
                                mess = mess + GetString(Resource.String.short_Mardi);
                                break;

                            case 2:
                                mess = mess + GetString(Resource.String.short_Mercredi);
                                break;

                            case 3:
                                mess = mess + GetString(Resource.String.short_Jeudi);
                                break;

                            case 4:
                                mess = mess + GetString(Resource.String.short_Vendredi);
                                break;

                            case 5:
                                mess = mess + GetString(Resource.String.short_Samedi);
                                break;

                            case 6:
                                mess = mess + GetString(Resource.String.short_Dimanche);
                                break;
                        }
                    }
                    else if (i < 5)
                        weekdays = false;

                    if (weekdays == true && tab[i] == true && i > 5)
                        weekdays = false;
                }



                if (count == tab.Length)
                    choseDate.Text = GetString(Resource.String.every_day);
                else if (count == 0)
                    choseDate.Text = GetString(Resource.String.once);
                else if (weekdays == true)
                    choseDate.Text = GetString(Resource.String.facto_day);
                else
                    choseDate.Text = mess;
            }
            else
                choseDate.Text = GetString(Resource.String.once);
        }
    }
}