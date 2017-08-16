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
    class ListModelContact
    {
        private string nom_ = "";
        private string numero_ = "0";
        private int id_ = 0;
        private bool selectionne_ = false;


        public ListModelContact (string nom, string numero , int id, bool selectionne = false)
        {
            selectionne_ = selectionne;
            id_ = id;
            numero_ = numero;
            nom_ = nom;
        }

        public string Nom { get { return nom_; } set { nom_ = value; } }
        public string Numero { get { return numero_; } set { numero_ = value; } }
        public bool setSelectionne { set { selectionne_ = value; } }
        public bool isSelectionne { get { return selectionne_ == true ? true : false; } }

    }
}