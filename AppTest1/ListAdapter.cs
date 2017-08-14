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
            view.FindViewById<TextView>(Resource.Id.CommentaireListText).Text = item.HourStart.Hour+":"+item.HourStart.Minute + " - " + item.HourEnd.Hour+":"+item.HourEnd.Minute;
            view.FindViewById<TextView>(Resource.Id.RepetitionText).Text = "";
            view.FindViewById<Switch>(Resource.Id.switchOnOff).Checked = item.SwitchState;
            return view;
        }
    }
}