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
using System.IO;
using static Android.Resource;
using Android.Preferences;
using Android.Provider;
using Android.Database;

namespace AppTest1
{
    class Global
    {
        private static Global global = null;
        private List<ListModel> list = null;
        public static int InvalideValue = -1;
        private int position_ = InvalideValue;
        private bool startedAppBoot_ = true;


        public static Global Instance
        {

            get
            {
                if (global == null)
                    global = new Global();

                return global;

            }
        }

        private Global()
        {
            Log.Info("info", "Global constructeur");
            if (list == null)
                list = new List<ListModel>();
            // SaveList();
            LoadList();
            position_ = InvalideValue;
        }

        public List<ListModel> GetList { get { return list; } }
        public ListModel GetElement(int posElement) { if (list.Count > posElement && posElement >= 0) return list[posElement]; else return null; }
        public void SetElement(int positionElem, ListModel newElement)
        {
            if (list != null)
                if (list.Count > positionElem && positionElem >= 0)
                    list[positionElem] = newElement;

        }
        public void SetDay(int positionElem, bool[] tab)
        {
            if (list != null)
                if (list.Count > positionElem && positionElem >= 0)
                    list[positionElem].Days = tab;
        }

        public void Add(ListModel value) { if (list != null) { list.Add(value); } else { list = new List<ListModel>(); list.Add(value); } }
        public int Position { set { position_ = value; } get { return position_; } }
        public bool IsInvalidPosition { get { return position_ == InvalideValue ? true : false; } }
        public bool BootStart { get { return startedAppBoot_; } set { startedAppBoot_ = value; } }


        public void SaveList()
        {
            string path = Application.Context.FilesDir.Path;
            var filePath = Path.Combine(path, "list.txt");

            string[] saveList = new string[list.Count];
            for (int i = 0; i < saveList.Length; i++)
            {
                if (list[i].ContactsList != null)
                    list[i].ContactsList.Sort();

                saveList[i] = list[i].Titre + ":" + list[i].SwitchState + ":" + list[i].MessageText + ":" + list[i].Invert + ":" + list[i].HourStart.Hour + ":" + list[i].HourStart.Minute + ":" + list[i].HourEnd.Hour + ":" + list[i].HourEnd.Minute;
                saveList[i] = saveList[i] + ":" + list[i].IsUnknownNumberEnable;
                if (list[i].Days != null)
                {
                    for (int j = 0; j < list[i].Days.Length; j++)
                        saveList[i] = saveList[i] + ":" + list[i].Days[j].ToString();
                }
                else
                {
                    Log.Info("save", "Save complete  null");
                    for (int j = 0; j < 7; j++)
                        saveList[i] = saveList[i] + ":false";
                }

                if (list[i].ContactsList != null)
                    foreach (string v in list[i].ContactsList)
                        saveList[i] = saveList[i] + ":" + v;
            }

            System.IO.File.WriteAllLines(filePath, saveList);
            if (saveList != null && saveList.Length > 0)
                Log.Info("save", "Save complete  " + saveList[0]);
        }


        public void LoadList()
        {
            string path = Application.Context.FilesDir.Path;
            var filePath = Path.Combine(path, "list.txt");

            var text = File.ReadAllLines(filePath);
            foreach (var ligne in text) // pour chaque liste enregistrée faire...
            {
                if (list == null)
                    list = new List<ListModel>();

                string[] split = ligne.Split(':');
                Log.Info("info", ligne);

                DateTime timeStart = new DateTime(2017, 01, 01, 0, 0, 0);
                DateTime timeEnd = new DateTime(2017, 01, 01, 0, 0, 0);
                if (split.Length >= 6) // get heures
                {
                    timeStart = new DateTime(2017, 01, 01, int.Parse(split[4]), int.Parse(split[5]), 0);
                    timeEnd = new DateTime(2017, 01, 01, int.Parse(split[6]), int.Parse(split[7]), 0);
                }

                ListModel model = null;
                List<string> contact = null;
                if (split.Length >= 9) // get jours
                {
                    bool[] boolTab = new bool[7] { bool.Parse(split[9]), bool.Parse(split[10]), bool.Parse(split[11]), bool.Parse(split[12]), bool.Parse(split[13]), bool.Parse(split[14]), bool.Parse(split[15]) };
                    model = new ListModel(bool.Parse(split[1]), split[0], split[2], null, timeStart, timeEnd, boolTab, bool.Parse(split[3]), bool.Parse(split[8]));

                    //build liste de contact
                    contact = new List<string>();
                    for (int i = 16; i < split.Length; i++)
                        contact.Add(split[i]);
                }
                else
                {
                    bool[] boolTab = new bool[7] { false, false, false, false, false, false, false };
                    model = new ListModel(bool.Parse(split[1]), split[0], split[2], null, timeStart, timeEnd, boolTab, bool.Parse(split[3]), bool.Parse(split[8]));
                }

                //add liste de contact
                model.ContactsList = contact;

                // ajout de la nouvelle liste
                list.Add(model);
            }

        }

        public void LoadPref(Context mContext)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            startedAppBoot_ = prefs.GetBoolean("bootStart", true);
        }

        public void SavePref(Context mContext)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutBoolean("bootStart", startedAppBoot_);
            editor.Apply();
        }



        public bool SearchPhoneNumber(string number, out short idlst, Context context)
        {
            bool trouver = true;
            idlst = 0;
            foreach (var lst in Global.Instance.GetList)
            {
                bool continuerTraitement;


                if (lst.ContactsList == null)
                    return false;

                if (lst.SwitchState == false)
                    continue;

                if (lst.Days[(int)DateTime.Now.DayOfWeek] == false)
                    continue;

                if (lst.Invert)
                {
                    continuerTraitement = false; // init de la valeur car la on vas chercher a savoir si on est dans les bornes

                    //on a inversé les plages horraies, on ne veux pas avoir les appel dans la tranche designée
                    //on test si on est dans la tranche horraire
                    if (lst.HourStart.Hour >= DateTime.Now.Hour && lst.HourEnd.Hour <= DateTime.Now.Hour)
                    {
                        if (lst.HourStart.Hour == DateTime.Now.Hour)
                            if (lst.HourStart.Minute >= DateTime.Now.Minute)
                                continuerTraitement = true;

                        if (lst.HourEnd.Hour == DateTime.Now.Hour)
                            if (lst.HourEnd.Minute <= DateTime.Now.Minute)
                                continuerTraitement = true;
                    }
                }
                else
                {
                    continuerTraitement = true; // init de la valeur cxar la on vas chercher a savoir si on est pas dans les bornes

                    //on test si on est dans la tranche horraire
                    if (lst.HourStart.Hour < DateTime.Now.Hour || lst.HourEnd.Hour > DateTime.Now.Hour)
                        continuerTraitement = false;
                    // on test si quand on est a la mm heure on a les bonnes minutes
                    else if ((lst.HourStart.Hour == DateTime.Now.Hour && lst.HourStart.Minute < DateTime.Now.Minute) &&
                        (lst.HourEnd.Hour == DateTime.Now.Hour && lst.HourEnd.Minute < DateTime.Now.Minute))
                        continuerTraitement = false;
                }

                if (!continuerTraitement)
                    continue;

                //si on recal les num qui sont pas dans le repertoire
                if (lst.IsUnknownNumberEnable)
                {
                    //on cherche son existance, si pas trouvé on retourne qu'on a trouvé afin d'envoyer la reponse auto et stoper le calcul
                    // si on trouve le num alors on continue le traitemetn
                    var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
                    string[] projectionNumber = { ContactsContract.Contacts.InterfaceConsts.NameRawContactId, ContactsContract.CommonDataKinds.Phone.Number };
                    var loaderNumber = new CursorLoader(context, uri, projectionNumber, null, null, null);
                    var cursorNumber = (ICursor)loaderNumber.LoadInBackground();

                    if (cursorNumber.MoveToFirst()) //...on cherche le num asocié
                    {
                        bool numTrouver = false;
                        do
                        {
                            //num trouvé dans contact, on met fin a la recherche
                            if (cursorNumber.GetString(cursorNumber.GetColumnIndex(projectionNumber[1])).Equals(number))
                            {
                                cursorNumber.MoveToLast();
                                numTrouver = true;
                            }
                        }
                        while (cursorNumber.MoveToNext());

                        //on a pas trouvé. On quitte. En effet on a fait ce test car on doit blocker les num inconnues dans la liste de contact
                        if (!numTrouver)
                            return true;
                    }
                }


                int debut = 0;
                int fin = lst.ContactsList.Count - 1;
                int mil = fin / 2;
                while (!trouver && fin > debut)
                {
                    //Log.Info("boucle", lst.ContactsList[mil] +"  "+ lst.ContactsList[mil].CompareTo(number));
                    int res = lst.ContactsList[mil].CompareTo(number);
                    if (res == 0)
                    {
                        trouver = true;
                    }
                    else if (res < 0)
                    {
                        debut = mil + 1;
                    }
                    else
                    {
                        fin = mil - 1;
                    }
                    mil = (fin + debut) / 2;
                    //Log.Info("boucle", "max = " + fin + "  min = " + debut);
                }

                if (fin == debut)
                {
                    if (lst.ContactsList[fin].CompareTo(number) == 0)
                        trouver = true;
                }
                //Log.Info("boucle", trouver.ToString());
                if (trouver)
                    continue;
                else
                    idlst++;
            }

            return trouver;
        }


    }
}
