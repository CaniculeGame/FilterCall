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
using Android.Text.Format;
using Java.Text;

namespace AppTest1
{
    class ListAdapter : BaseAdapter<ListModel>
    {
        List<ListModel> items;
        Activity context;

        public ListAdapter(Activity context, List<ListModel> items) : base()
        {
            this.context = context;
            this.items = items;
        }


        public override long GetItemId(int position)
        {
            return position;
        }


        public override ListModel this[int position]
        {
            get { return items[position]; }
        }


        public override int Count
        {
            get { return items.Count; }
        }



        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.RowModel, null);

            view.FindViewById<TextView>(Resource.Id.TitreListText).Text = item.Titre;
            view.FindViewById<TextView>(Resource.Id.RepetitionText).Text = ChooseDateString(item.Days);


            if (item.HourStart.Hour == 0 && item.HourStart.Minute == 0 && item.HourEnd.Hour == 0 && item.HourEnd.Minute == 0)
                view.FindViewById<TextView>(Resource.Id.CommentaireListText).Text = context.GetString(Resource.String.tteJournee);
            else if (Android.Text.Format.DateFormat.Is24HourFormat(parent.Context))
                view.FindViewById<TextView>(Resource.Id.CommentaireListText).Text = item.HourStart.Hour + ":" + item.HourStart.Minute + " - " + item.HourEnd.Hour + ":" + item.HourEnd.Minute;
            else
            {
                string str = "";
                if (item.HourStart.Hour >= 12)
                    str = (item.HourStart.Hour - 12) + ":" + item.HourStart.Minute + " PM - ";
                if (item.HourStart.Hour < 12)
                    str = (item.HourStart.Hour) + ":" + item.HourStart.Minute + " Am - ";

                if (item.HourEnd.Hour >= 12)
                    str += (item.HourEnd.Hour - 12) + ":" + item.HourEnd.Minute + " PM";
                if (item.HourEnd.Hour < 12)
                    str += (item.HourEnd.Hour) + ":" + item.HourEnd.Minute + " Am";

                view.FindViewById<TextView>(Resource.Id.CommentaireListText).Text = str;
            }



            Switch sw = view.FindViewById<Switch>(Resource.Id.switchOnOff);
            sw.Checked = item.SwitchState;
            sw.Click += delegate 
            {
                item.SwitchState = !item.SwitchState;
                Global.Instance.Save();
            };


            return view;
        }



        private string ChooseDateString(bool[] tab)
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
                                mess = mess + context.Resources.GetString(Resource.String.short_Lundi);
                                break;

                            case 1:
                                mess = mess + context.Resources.GetString(Resource.String.short_Mardi);
                                break;

                            case 2:
                                mess = mess + context.Resources.GetString(Resource.String.short_Mercredi);
                                break;

                            case 3:
                                mess = mess + context.Resources.GetString(Resource.String.short_Jeudi);
                                break;

                            case 4:
                                mess = mess + context.Resources.GetString(Resource.String.short_Vendredi);
                                break;

                            case 5:
                                mess = mess + context.Resources.GetString(Resource.String.short_Samedi);
                                break;

                            case 6:
                                mess = mess + context.Resources.GetString(Resource.String.short_Dimanche);
                                break;
                        }
                    }
                    else if (i < 5)
                        weekdays = false;

                    if (weekdays == true && tab[i] == true && i > 5)
                        weekdays = false;
                }



                if (count == tab.Length)
                    mess = context.Resources.GetString(Resource.String.every_day);
                else if (count == 0)
                    mess = context.Resources.GetString(Resource.String.once);
                else if (weekdays == true)
                    mess = context.Resources.GetString(Resource.String.facto_day);
            }
            else
                mess = context.Resources.GetString(Resource.String.once);

            return mess;
        }
    }
}