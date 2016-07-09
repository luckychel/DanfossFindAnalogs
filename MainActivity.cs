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
            var adapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, ids);

            //var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Codes, Resource.Layout.list_item);

            textView.Adapter = adapter;
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
}

