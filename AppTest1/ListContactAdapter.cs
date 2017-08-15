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
    class ListContactAdapter : BaseAdapter<ListModelContact>
    {
        private List<ListModelContact> items;
        private Activity context;
        private int heightItem = 0;

        public ListContactAdapter(Activity context, List<ListModelContact> items) : base()
        {
            this.context = context;
            this.items = items;
        }


        public override long GetItemId(int position)
        {
            return position;
        }


        public override ListModelContact this[int position]
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
                view = context.LayoutInflater.Inflate(Resource.Layout.ContactItem, null);

            view.FindViewById<TextView>(Resource.Id.textNumero).Text = item.Numero.ToString();
            view.FindViewById<CheckBox>(Resource.Id.contactItemBox).Checked = item.isSelectionne;
            view.FindViewById<CheckBox>(Resource.Id.contactItemBox).Text = item.Nom;


            return view;
        }

    }
}