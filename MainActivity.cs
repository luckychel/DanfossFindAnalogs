﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Java.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Object = Java.Lang.Object;

namespace DanfossFindAnalogs
{

    
    [Activity
        (
            MainLauncher = true
            , Label = "Аналоги Данфосс кодов"
            , Theme = "@style/MyCustomTheme"
            , Icon = "@drawable/Icon"
            , ConfigurationChanges = (Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)
            
        )
    ]
    public class MainActivity : Activity
    {
        
        private XDocument хdoc;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar actionBar = ActionBar;
            actionBar.Title = "";
            actionBar.SetDisplayHomeAsUpEnabled(false);
            actionBar.SetDisplayShowCustomEnabled(true);
            actionBar.SetCustomView(Resource.Layout.actionBar);
            actionBar.NavigationMode = ActionBarNavigationMode.Standard;
            Toolbar parent = (Toolbar)actionBar.CustomView.Parent;
            parent.SetContentInsetsAbsolute(0, 0);

            SetContentView(Resource.Layout.Main);

            LinearLayout ll = FindViewById<LinearLayout>(Resource.Id.LL);
            ll.SetBackgroundColor(Color.ParseColor("#f0f0f0"));

            TextView textView2 = FindViewById<TextView>(Resource.Id.textView2);
           
            хdoc = XDocument.Load(Resources.GetXml(Resource.Xml.codes));
            var codes = хdoc.Root.Elements("item").Select(x => x);
            var ids = codes.Select(x => x.Attribute("id").Value).ToList();
            var adapter = new ArrayExAdapter<string>(this, Resource.Layout.listItem, ids);
            //var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Codes, Resource.Layout.list_item);

            AutoCompleteTextView autocomplete = FindViewById<AutoCompleteTextView>(Resource.Id.autocomplete_codes);
            autocomplete.SetHintTextColor(Color.Red);
            autocomplete.Adapter = adapter;
            autocomplete.Threshold = 1;
            autocomplete.AddTextChangedListener(new MyTextWatcher(this.BaseContext));
            autocomplete.ItemClick += (sender, args) =>
            {
                var itemView = sender as TextView;

                if (itemView.Text == "Больше 50 совпадений...")
                {
                    autocomplete.SetText("", TextView.BufferType.Normal);
                    textView2.SetText("", TextView.BufferType.Normal);
                    return;
                }

                //Toast.MakeText(this, itemView.Text, ToastLength.Short).Show();

                if (хdoc == null)
                    хdoc = XDocument.Load(Resources.GetXml(Resource.Xml.codes));

                var item = хdoc.Root.Elements("item").Where(x => (string)x.Attribute("id") == itemView.Text).FirstOrDefault();

                textView2.SetText("Бренд: " + item.Attribute("brend").Value, TextView.BufferType.Normal);

                var tableLayout = FindViewById<TableLayout>(Resource.Id.results1);
                tableLayout.RemoveAllViewsInLayout();
                TableLayout.LayoutParams par = new TableLayout.LayoutParams(TableLayout.LayoutParams.WrapContent, TableLayout.LayoutParams.MatchParent);
                par.SetMargins(20, 0, 20, 0);
                tableLayout.LayoutParameters = par;

                var txtView = new TextView(this);

                if (item != null)
                {
                    var codeData = new List<string>() {
                            item.Attribute("custom").Value,
                            item.Attribute("model").Value
                        };

                    txtView.Text = "Аналог " + (codeData[0] == "Vacon" ? codeData[0] : "Данфосс");
                    txtView.Gravity = GravityFlags.Left;
                    txtView.SetPadding(25, 25, 25, 25);
                    txtView.SetTextColor(Color.Black);
                    txtView.SetTextSize(Android.Util.ComplexUnitType.Px, 70);
                    txtView.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

                    var tableRow = new TableRow(this);
                    tableRow.AddView(txtView, new TableRow.LayoutParams() { Span = 2 });
                    tableLayout.AddView(tableRow);

                    for (int i = 0; i < 1; i++)
                    {

                        tableRow = new TableRow(this);
                        tableRow.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent);

                        for (int j = 0; j < codeData.Count; j++)
                        {

                            txtView = new TextView(this);
                            txtView.Text = codeData[j];
                            txtView.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent);
                            txtView.SetBackgroundResource(Resource.Layout.cell);
                            txtView.Gravity = GravityFlags.CenterHorizontal;
                            txtView.SetPadding(25, 25, 25, 25);
                            txtView.SetTextColor(Color.Black);
                            txtView.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                            tableRow.AddView(txtView, j);
                        }

                        tableLayout.AddView(tableRow);
                    }

                    hideKeyBoard();
                }


            };

            
 
        }


        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {

            base.OnConfigurationChanged(newConfig);

            hideKeyBoard();

            //Toast.MakeText(this, "called OnConfigurationChanged", ToastLength.Long).Show();

            //if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape)
            //{
            //    Toast.MakeText(this, "landscape", ToastLength.Long).Show();
            //}
            //else
            //{
            //    Toast.MakeText(this, "portrait", ToastLength.Long).Show();
            //}
        }


        private void hideKeyBoard()
        {
            InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            var currentFocus = this.CurrentFocus;
            if (currentFocus != null)
            {
                inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
            }
        }
    }



    public class MyTextWatcher : Java.Lang.Object, ITextWatcher
    {

        private Context myContext;
        public MyTextWatcher(Context ctx) {
            myContext = ctx;
        }

        public void AfterTextChanged(IEditable s) {
            //var ss = s.Where(x => x.ToString() != "\n").ToArray();
            //s = new Java.Lang.String(ss);
        }
        public void BeforeTextChanged(Java.Lang.ICharSequence arg0, int start, int count, int after) {

            //var ss = arg0.Where(x => x.ToString() != "\n").ToArray();
            //arg0 = new Java.Lang.String(ss);

        }
        public void OnTextChanged(Java.Lang.ICharSequence arg0, int start, int before, int count) {

            //var ss = arg0.Where(x => x.ToString() != "\n").ToArray();
            //arg0 = new Java.Lang.String(ss);

        }
    }


   

    public class JavaObjectWrapper<T> : Java.Lang.Object
    {
        public T Obj { get; set; }
    }


    public class ArrayExAdapter<Object> : ArrayAdapter
    {
        
        private List<string> _originalData;
        private List<string> _items;
        private readonly Activity _context;

        public ArrayExAdapter(Activity context, int resource, List<string> objects) : base(context, resource, objects)
        {
            _context = context;
            _items = objects;

        }

        MyFilter myfilter;

        public override Filter Filter
        {
            get
            {
                if (myfilter == null)
                {
                    myfilter = new MyFilter(this);
                }
                return myfilter;
            }
        }


        private class MyFilter : Filter
        {
            private readonly ArrayExAdapter<Object> adapter;

            public MyFilter(ArrayExAdapter<Object> adapter)
            {
                this.adapter = adapter;
            }

            protected override Filter.FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
            {
                FilterResults results = new FilterResults();
                if (adapter._originalData == null)
                    adapter._originalData = adapter._items;

                if (constraint == null) return results;
                
                var matchList = new List<Java.Lang.Object>();

                if (adapter._originalData != null && adapter._originalData.Any())
                {
                    foreach (var txt in adapter._originalData)
                    {
                        if (txt.ToString().ToUpper().Contains(constraint.ToString().ToUpper()))
                        {
                            //выходим когда кол-во совпадений больше 50
                            if (matchList.Count == 50)
                            {
                                matchList.Add("Больше 50 совпадений...");
                                break;
                            }

                            matchList.Add(txt);
                        }
                    }
                }                               

                var resultsValues = new Java.Lang.Object[matchList.Count];
                for (int i = 0; i < matchList.Count; i++)
                {
                    resultsValues[i] = matchList[i];
                }

                results.Count = matchList.Count;
                results.Values = resultsValues;

                constraint.Dispose();

                return results;

            }


            protected override void PublishResults(Java.Lang.ICharSequence constraint, Filter.FilterResults results)
            {
                if (results.Count == 0) return;

                var list = results.Values.ToArray<Java.Lang.Object>();
                if (list != null && list.Length > 0)
                {
                    adapter.Clear();
                    foreach (var t in list)
                    {
                        adapter.Add(t);
                    }
                }
                if (results.Count > 0)
                {
                    adapter.NotifyDataSetChanged();
                }
                else
                {
                    adapter.NotifyDataSetInvalidated();
                }

                constraint.Dispose();

                results.Dispose();
            }

        }
    }

   

}

