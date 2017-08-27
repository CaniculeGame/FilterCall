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
using Android.Provider;
using Android.Database;
using Android.Text.Format;
using Android.Telephony;

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
        Button unkownButton = null;
        EditText titre = null;
        TimePicker hourEnd = null;
        TimePicker hourStart = null;
        EditText message = null;
        ListView contactListView = null;
        bool switchTime = false;
        bool switchUnknown = false;
        Button buttonTime = null;

        List<ListModelContact> contactList = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);

            // Create your application here
            SetContentView(Resource.Layout.SettingPage);


            titre = FindViewById<EditText>(Resource.Id.TitleTextEdit);
            message = FindViewById<EditText>(Resource.Id.editTextMessage);

            switchTime = false;

            choseDate = FindViewById<Button>(Resource.Id.dayButton);
            if (choseDate != null)
            {
                if (Global.Instance.IsInvalidPosition)
                    choseDate.Text = GetString(Resource.String.once);
                else
                {
                    tabBool = Global.Instance.GetElement(Global.Instance.Position).Days;
                    ChooseDateString(tabBool);
                }

                choseDate.Click += delegate
                {
                    ChoseDialog();
                };
            }

            saveButton = FindViewById<Button>(Resource.Id.SaveButton);
            if (saveButton != null)
            {
                saveButton.Click += delegate
                {
                    if (Save())
                        ChangeActivity();
                    else
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle(Resource.String.alerte);
                        alert.SetMessage(Resource.String.alerteMessage);
                        Dialog dialog = alert.Create();
                        dialog.Show();
                    }
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

            unkownButton = FindViewById<Button>(Resource.Id.buttonUnknow);
            if (unkownButton != null)
            {
                unkownButton.Click += delegate
                {
                    switchUnknown = !switchUnknown;
                    if (switchUnknown)
                        unkownButton.Text = GetString(Resource.String.enable);
                    else
                        unkownButton.Text = GetString(Resource.String.disable);

                };
            }


            contactListView = FindViewById<ListView>(Resource.Id.contactListView);
            if (contactListView != null)
            {
                contactListView.ChoiceMode = ChoiceMode.Multiple;
                contactList = new List<ListModelContact>();



                var uri = ContactsContract.Contacts.ContentUri;
                string[] projection = {
                    ContactsContract.Contacts.InterfaceConsts.NameRawContactId,
                    ContactsContract.Contacts.InterfaceConsts.DisplayName,
                    ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber
                };

                var loader = new CursorLoader(this, uri, projection, null, null, null);
                var cursor = (ICursor)loader.LoadInBackground();


                //recherche du numero associé au nom du contact
                uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
                string[] projectionNumber = { ContactsContract.Contacts.InterfaceConsts.NameRawContactId, ContactsContract.CommonDataKinds.Phone.Number };
                var loaderNumber = new CursorLoader(this, uri, projectionNumber, null, null, null);
                var cursorNumber = (ICursor)loaderNumber.LoadInBackground();

                if (cursor.MoveToFirst())
                {
                    string num = "0";
                    do //pour chaque contact ...
                    {
                        //Log.Info("number", "recherche pour : " + cursor.GetString(cursor.GetColumnIndex(projection[1])) + "  " + cursor.GetString(cursor.GetColumnIndex(projection[0])));
                        num = "0";
                        if (cursorNumber.MoveToFirst()) //...on cherche le num asocié
                        {
                            do
                            {
                                //Log.Info("number", "numero ass : " + cursorNumber.GetString(cursorNumber.GetColumnIndex(projectionNumber[1])) +"  " + cursorNumber.GetString(cursorNumber.GetColumnIndex(projectionNumber[0])));
                                if (cursorNumber.GetString(cursorNumber.GetColumnIndex(projectionNumber[0])) == cursor.GetString(cursor.GetColumnIndex(projection[0])))
                                {
                                    num = cursorNumber.GetString(cursorNumber.GetColumnIndex(projectionNumber[1]));
                                    cursorNumber.MoveToLast();
                                }
                            }
                            while (cursorNumber.MoveToNext());
                        }

                        if (num != "0")
                        {
                            ListModelContact ct = new ListModelContact(cursor.GetString(cursor.GetColumnIndex(projection[1])),
                            num,
                            cursor.GetInt(cursor.GetColumnIndex(projection[0])));
                            contactList.Add(ct);
                        }

                    } while (cursor.MoveToNext());//on passe a un autre contact
                }

                //renseignemant des numoro desja selectionné
                foreach (var v in contactList)
                {
                    if (Global.Instance.GetElement(Global.Instance.Position) != null)
                        foreach (var ct in Global.Instance.GetElement(Global.Instance.Position).ContactsList)
                        {
                            if (v.Numero.Equals(ct))
                                v.SetSelectionne = true;
                        }
                }


                contactListView.Adapter = new ListContactAdapter(this, contactList);
                FindViewById<LinearLayout>(Resource.Id.linearLayoutScroll).LayoutParameters.Height = 110 * contactList.Count;
            }


            hourEnd = FindViewById<TimePicker>(Resource.Id.timePickerEnd);
            if (hourEnd != null)
            {
                if (DateFormat.Is24HourFormat(this))
                    hourEnd.SetIs24HourView(Java.Lang.Boolean.True);

                hourEnd.TimeChanged += delegate
                {
                    SetTimeFormat(hourStart, hourEnd, switchTime);
                };
            }


            hourStart = FindViewById<TimePicker>(Resource.Id.timePickerStart);
            if (hourStart != null)
            {
                if (DateFormat.Is24HourFormat(this))
                    hourStart.SetIs24HourView(Java.Lang.Boolean.True);

                hourStart.TimeChanged += delegate
                {
                    SetTimeFormat(hourStart, hourEnd, switchTime);
                };
            }

            buttonTime = FindViewById<Button>(Resource.Id.buttonInvertTps);
            if (buttonTime != null)
            {
                buttonTime.Click += delegate
                {
                    ChangeInvertTime();
                };
            }

            if (Global.Instance.Position != Global.InvalideValue)
            {
                if (Global.Instance.GetList.Count > Global.Instance.Position && Global.Instance.Position >= 0)
                {
                    titre.Text = Global.Instance.GetElement(Global.Instance.Position).Titre;
                    hourEnd.Hour = Global.Instance.GetElement(Global.Instance.Position).HourEnd.Hour;
                    hourEnd.Minute = Global.Instance.GetElement(Global.Instance.Position).HourEnd.Minute;
                    hourStart.Hour = Global.Instance.GetElement(Global.Instance.Position).HourStart.Hour;
                    hourStart.Minute = Global.Instance.GetElement(Global.Instance.Position).HourStart.Minute;
                    message.Text = Global.Instance.GetElement(Global.Instance.Position).MessageText;

                    switchTime = Global.Instance.GetElement(Global.Instance.Position).Invert;
                    switchUnknown = Global.Instance.GetElement(Global.Instance.Position).IsUnknownNumberEnable;
                    if (switchUnknown)
                        unkownButton.Text = GetString(Resource.String.enable);
                    else
                        unkownButton.Text = GetString(Resource.String.disable);
                    SetTimeFormat(hourStart, hourEnd, switchTime);

                }
                else
                {
                    titre.Text = "no title";
                    hourEnd.Hour = 0;
                    hourStart.Hour = 0;
                    hourEnd.Minute = 0;
                    hourStart.Minute = 0;
                    message.Text = GetString(Resource.String.Message);
                    buttonTime.Text = GetString(Resource.String.tteJournee);
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
                buttonTime.Text = GetString(Resource.String.tteJournee);
            }


            FindViewById<LinearLayout>(Resource.Id.linearLayout1).RequestFocus();

        }

        private void SetTimeFormat(TimePicker deb, TimePicker fin, bool inv)
        {
            string message = "";
            if (deb.Hour == 0 && deb.Minute == 0 && fin.Hour == 0 && fin.Minute == 0)
            {
                message = GetString(Resource.String.tteJournee);
            }
            else
            {
                if (inv)
                {
                    Log.Info("info", "invert");

                    message = GetString(Resource.String.from) + "  00:00  " + GetString(Resource.String.to) + " " + deb.Hour.ToString("00") + ":" + deb.Minute.ToString("00") + "  "
                            + GetString(Resource.String.and) + "  " + fin.Hour.ToString("00") + ":" + fin.Minute.ToString("00") + "  " + GetString(Resource.String.to) + "  23:59";

                }
                else
                {
                    Log.Info("info", "No invert");
                    if (Android.Text.Format.DateFormat.Is24HourFormat(this))
                        message = deb.Hour.ToString("00") + ":" + deb.Minute.ToString("00") + " - " + fin.Hour.ToString("00") + ":" + fin.Minute.ToString("00");
                    else
                    {
                        if (deb.Hour >= 12)
                            message = (deb.Hour - 12).ToString("00") + ":" + deb.Minute.ToString("00") + " PM - ";
                        if (deb.Hour < 12)
                            message = deb.Hour.ToString("00") + ":" + deb.Minute.ToString("00") + " Am - ";

                        if (fin.Hour >= 12)
                            message += (fin.Hour - 12).ToString("00") + ":" + fin.Minute.ToString("00") + " PM";
                        if (fin.Hour < 12)
                            message += fin.Hour.ToString("00") + ":" + fin.Minute.ToString("00") + " Am";

                    }
                }
            }
            buttonTime.Text = message;
        }

        private void ChoseDialog()
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

        private bool Save()
        {
            DateTime timeEnd = new DateTime(2017, 01, 01, hourEnd.Hour, hourEnd.Minute, 0);
            DateTime timeStart = new DateTime(2017, 01, 01, hourStart.Hour, hourStart.Minute, 0);

            if (timeEnd.CompareTo(timeStart) < 0)
                return false;

            //creation liste de contact
            List<string> contactNum = new List<string>();
            for (int i = 0; i < contactList.Count; i++)
            {
                if (contactList[i].IsSelectionne)
                {
                    Log.Info("contact", contactList[i].Numero);
                    contactNum.Add(contactList[i].Numero);
                }
            }

            ListModel newItem = null;
            if (tabBool != null)
                newItem = new ListModel(true, titre.Text, message.Text, contactNum, timeStart, timeEnd, tabBool, switchTime, switchUnknown);
            else
                newItem = new ListModel(true, titre.Text, message.Text, contactNum, timeStart, timeEnd, new bool[] { false, false, false, false, false, false, false }, switchTime, switchUnknown);


            if (Global.Instance.IsInvalidPosition)
                Global.Instance.Add(newItem);
            else
                Global.Instance.SetElement(Global.Instance.Position, newItem);

            Global.Instance.SaveList();
            return true;
        }

        private void Delete()
        {
            if (Global.Instance.Position != Global.InvalideValue && Global.Instance.Position >= 0)
                Global.Instance.GetList.RemoveAt(Global.Instance.Position);
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
                        if (i != 0)
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

        private void ChangeInvertTime()
        {
            if (buttonTime != null)
            {
                switchTime = !switchTime;
                SetTimeFormat(hourStart, hourEnd, switchTime);

            }
        }

    }
}