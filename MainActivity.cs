using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Content.Res;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using Java.Util;
using Java.Lang;
using System.Collections;
using Object = Java.Lang.Object;


namespace DanfossFindAnalogs
{
    

    [Activity(MainLauncher = true)]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            AutoCompleteTextView textView = FindViewById<AutoCompleteTextView>(Resource.Id.autocomplete_codes);

            var xdoc = XDocument.Load(Resources.GetXml(Resource.Xml.codes));
            var codes = xdoc.Root.Elements("item").Select(x => x);
            var ids = codes.Select(x => x.Attribute("id").Value).ToList();

            //var adapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, ids);
            var adapter2 = new ArrayExAdapter<string>(this, Resource.Layout.list_item, ids);
            //var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Codes, Resource.Layout.list_item);

            textView.Adapter = adapter2;
            textView.Threshold = 1;

            textView.ItemClick += (sender, args) =>
            {
                var itemView = sender as TextView;
                
                Toast.MakeText(this, itemView.Text, ToastLength.Short).Show();

                var tableLayout = FindViewById<TableLayout>(Resource.Id.results);
                tableLayout.RemoveAllViewsInLayout();
                tableLayout.LayoutParameters = new TableRow.LayoutParams(TableLayout.LayoutParams.MatchParent, TableLayout.LayoutParams.MatchParent);

                var title = new TextView(this);

                var хdoc2 = XDocument.Load(Resources.GetXml(Resource.Xml.codes));
                var item = хdoc2.Root.Elements("item").Where(x => (string)x.Attribute("id") == itemView.Text).FirstOrDefault();

                if (item != null)
                {
                    var codeData = new List<string>() {
                        item.Attribute("brend").Value,
                        item.Attribute("custom").Value,
                        item.Attribute("model").Value
                    };

                    title.Text = "Аналог " + (codeData[0] == "Vacon" ? codeData[0] : "Данфосс");
                    title.SetBackgroundResource(Resource.Layout.cell);
                    title.Gravity = GravityFlags.Center;
                    title.SetPadding(25, 25, 25, 25);
                    title.SetTextColor(Color.White);
                    title.SetBackgroundColor(Color.Red);
                    title.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

                    var tableRow = new TableRow(this);
                    tableRow.AddView(title, new TableRow.LayoutParams() { Span = 3 });
                    tableLayout.AddView(tableRow);

                    for (int i = 0; i < 1; i++)
                    {

                        tableRow = new TableRow(this);
                        tableRow.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent,
                                TableRow.LayoutParams.MatchParent);

                        for (int j = 0; j < codeData.Count; j++)
                        {

                            TextView b = new TextView(this);
                            b.Text = codeData[j];
                            b.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent);
                            b.SetBackgroundResource(Resource.Layout.cell);
                            b.Gravity = GravityFlags.CenterHorizontal;
                            b.SetPadding(25, 25, 25, 25);
                            b.SetTextColor(Color.Black);
                            b.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                            tableRow.AddView(b, j);
                        }

                        tableLayout.AddView(tableRow);
                    }
                }


            };

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

