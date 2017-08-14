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

namespace AppTest1
{
    class Global
    {
        private static Global global = null;
        private List<ListModel> list = null;
        public static int InvalideValue = -1;
        private int position_ = InvalideValue;

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

            Load();
            position_ = InvalideValue;
        }

        public List<ListModel> getList { get { return list; }}
        public ListModel getElement(int posElement) { if (list.Count > posElement && posElement >= 0) return list[posElement]; else return null; }
        public void setElement(int positionElem, ListModel newElement)
        {
            if (list != null)
                if (list.Count > positionElem && positionElem >= 0)
                    list[positionElem] = newElement;

        }
        public void SetDay(int positionElem,bool[] tab)
        {
            if (list != null)
                if (list.Count > positionElem && positionElem >= 0)
                    list[positionElem].Days = tab;
        }

        public void Add(ListModel value) { if (list != null) { list.Add(value); } else { list = new List<ListModel>(); list.Add(value); } }
        public int Position { set { position_ = value; }  get { return position_; } }
        public bool isInvalidPosition { get { return position_ == InvalideValue ? true : false; } }

        public void Save()
        {
            string path = Application.Context.FilesDir.Path;
            var filePath = Path.Combine(path, "list.txt");

            string[] saveList = new string[list.Count];
            for (int i = 0; i < saveList.Length; i++)
            {
                saveList[i] = list[i].Titre + ":" + list[i].SwitchState + ":" + list[i].MessageText + ":" + list[i].HourStart.Hour +":"+ list[i].HourStart.Minute + ":" + list[i].HourEnd.Hour + ":" + list[i].HourEnd.Minute;
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
            }

            System.IO.File.WriteAllLines(filePath, saveList);
            Log.Info("save", "Save complete  " + saveList[0]);

        }


        public void Load()
        {
            string path = Application.Context.FilesDir.Path;
            var filePath = Path.Combine(path, "list.txt");

            var text = File.ReadAllLines(filePath);
            foreach (var ligne in text)
            {
                if (list == null)
                    list = new List<ListModel>();

                string[] split = ligne.Split(':');
                Log.Info("info", ligne);

                DateTime timeStart = new DateTime(2017, 01, 01, 0, 0, 0);
                DateTime timeEnd = new DateTime(2017, 01, 01, 0, 0, 0);
                if (split.Length >= 6)
                {
                    timeStart = new DateTime(2017, 01, 01, int.Parse(split[3]), int.Parse(split[4]), 0);
                    timeEnd = new DateTime(2017, 01, 01, int.Parse(split[5]), int.Parse(split[6]), 0);
                }

                if (split.Length >= 7)
                {
                    bool[] boolTab = new bool[7] { bool.Parse(split[7]), bool.Parse(split[8]), bool.Parse(split[9]), bool.Parse(split[10]), bool.Parse(split[11]), bool.Parse(split[12]), bool.Parse(split[13]) };
                    ListModel model = new ListModel(bool.Parse(split[1]), split[0], split[2], null, timeStart, timeEnd, boolTab);
                    list.Add(model);
                }
                else
                {
                    bool[] boolTab = new bool[7] { false, false, false, false, false, false, false };
                    ListModel model = new ListModel(bool.Parse(split[1]), split[0], split[2], null, timeStart, timeEnd,boolTab);
                    list.Add(model);
                }
            }
        }



    }
}
