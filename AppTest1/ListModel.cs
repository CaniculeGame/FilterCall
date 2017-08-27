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

namespace AppTest1
{
    class ListModel 
    {
        private bool switchState_ = false;
        private string titre_ = "no_title";
        private string message_ = "no_message";
        private List<string> contacts_ = null;
        private DateTime hourStart_;
        private DateTime hourEnd_;
        private string commentaire_ = "aucun commentaire";
        private bool[] days_ = new bool[7];
        private bool invert_ = false;
        private bool unknownNumber_ = false;

        public ListModel()
        {
        }


        public ListModel(bool state, string titre, string message, List<string> contacts, DateTime start, DateTime end, bool[] days, bool invert, bool unknownNumber)
        {
            switchState_ = state;
            titre_ = titre;
            message_ = message;
            contacts_ = contacts;
            hourStart_ = start;
            hourEnd_ = end;
            days_ = days;
            invert_ = invert;
            unknownNumber_ = unknownNumber;
        }


        public bool SwitchState { get { return switchState_; } set { switchState_ = value; } }
        public string Titre { get { return titre_; } set { titre_ = value; } }
        public string MessageText { get { return message_; } set { message_ = value; } }
        public List<string> ContactsList { get { return contacts_; } set { contacts_ = value; } }
        public DateTime HourEnd { get { return hourEnd_; } set { hourEnd_ = value; } }
        public DateTime HourStart { get { return hourStart_; } set { hourStart_ = value; } }
        public string Commentaire { get { return commentaire_; } set { commentaire_ = value; } }
        public bool[] Days { get { return days_;  }  set { days_ = value; } }
        public bool Invert { get { return invert_; } set { invert_ = value; } }
        public bool IsUnknownNumberEnable { get { return unknownNumber_; } set { unknownNumber_ = value; } }




    }
}