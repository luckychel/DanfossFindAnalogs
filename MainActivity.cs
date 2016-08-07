using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CompetitorTool
{


    [Activity
        (
            MainLauncher = true
            , Label = "Competitor Tool"
            , Theme = "@style/MyCustomTheme"
            , Icon = "@drawable/Icon"
            , ConfigurationChanges = (Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)
            
        )
    ]
    public class MainActivity : Activity
    {
        
        private XDocument хdoc;
        private TextView txtView;
      

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            ActionBar actionBar = ActionBar;
            actionBar.Title = "";
            actionBar.SetDisplayHomeAsUpEnabled(false);
            actionBar.SetDisplayShowCustomEnabled(true);
            actionBar.SetCustomView(Resource.Layout.actionBar);
            actionBar.NavigationMode = ActionBarNavigationMode.Standard;

            this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Sensor;

            try
            {
                Toolbar parent = (Toolbar)actionBar.CustomView.Parent;
                parent.SetContentInsetsAbsolute(0, 0);
            }
            catch (Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Color.ParseColor("#9c0303"));
            }

            LinearLayout ll = FindViewById<LinearLayout>(Resource.Id.LL);
            ll.SetBackgroundColor(Color.ParseColor("#f0f0f0"));

            TextView textView2 = FindViewById<TextView>(Resource.Id.textView2);
           
            хdoc = XDocument.Load(Resources.GetXml(Resource.Xml.codes));
            var codes = хdoc.Root.Elements("item").Select(x => x);
            var ids = codes.Select(x => x.Attribute("order").Value + " | " + x.Attribute("type").Value).ToList();
            var adapter = new ArrayExAdapter<string>(this, Resource.Layout.listItem, ids);
            //var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.Codes, Resource.Layout.list_item);

            AutoCompleteTextView autocomplete = FindViewById<AutoCompleteTextView>(Resource.Id.autocomplete_codes);
            autocomplete.SetHintTextColor(Color.Red);
            autocomplete.Adapter = adapter;
            autocomplete.Threshold = 1;
            //autocomplete.((ProgressBar)findViewById(R.id.progress_bar));
            autocomplete.AddTextChangedListener(new MyTextWatcher(this.BaseContext));
            autocomplete.ItemClick += (sender, args) =>
            {
               
                var itemView = sender as TextView;

                if (itemView.Text == limitRows.limValueText)
                {
                    autocomplete.SetText("", TextView.BufferType.Normal);
                    return;
                }

                if (хdoc == null)
                    хdoc = XDocument.Load(Resources.GetXml(Resource.Xml.codes));

                var search = itemView.Text.Split('|').Select(p => p.Trim()).ToList();

                var item = хdoc.Root.Elements("item").Where(x => (string)x.Attribute("order") == search[0] && (string)x.Attribute("type") == search[1]).FirstOrDefault();
                var brend = item.FirstAttribute.Value;

                textView2.SetText("Бренд: " + brend, TextView.BufferType.Normal);

                var tableLayout = FindViewById<TableLayout>(Resource.Id.results1);
                tableLayout.RemoveAllViewsInLayout();
                tableLayout.RemoveAllViews();

                TableLayout.LayoutParams par = new TableLayout.LayoutParams(TableLayout.LayoutParams.WrapContent, TableLayout.LayoutParams.MatchParent);
                par.SetMargins(20, 0, 20, 0);
                tableLayout.LayoutParameters = par;

                if (item != null)
                {
                    var codeData = new List<string>() {
                            item.Attribute("series").Value,
                            item.Attribute("custom").Value,
                            item.Attribute("model").Value
                        };
                    
                    var isVacon = (brend == "Vacon" ? true : false);

                    txtView = new TextView(this);
                    txtView.Text = "Аналог марки VLT";
                    txtView.Gravity = GravityFlags.Left;
                    txtView.SetPadding(25, 25, 25, 25);
                    txtView.SetTextColor(Color.Black);
                    txtView.SetTextSize(Android.Util.ComplexUnitType.Dip, 20);
                    txtView.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);
                    
                    var tableRow = new TableRow(this);
                    tableRow.AddView(txtView);
                    tableLayout.AddView(tableRow);

                    if (isVacon)
                    {
                        var items = хdoc.Root.Elements("item")
                            .Where(x => (string)x.Attribute("brend") != "Vacon" && (string)x.Attribute("custom") == item.Attribute("custom").Value)
                            .OrderBy(x => (string)x.Attribute("brend"))
                            .ToList();

                        if (items != null && items.Count > 0)
                        {
                            Toast.MakeText(this, "Найдено записей: " + items.Count.ToString(), ToastLength.Short).Show();
                            drawItems(items, tableLayout, tableRow);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < codeData.Count; i++)
                        {

                            var txt = "<b>";
                            if (i == 0)
                                txt += "Серия:";
                            else if (i == 1)
                                txt += "Заказной код:";
                            else if (i == 2)
                                txt += "Типовой код:";
                            txt += "<b> ";

                            txtView = new TextView(this);
                            txtView.TextFormatted = Html.FromHtml(txt + codeData[i]);
                            txtView.SetBackgroundResource(Resource.Layout.finded);
                            txtView.Gravity = GravityFlags.Left;
                            txtView.SetPadding(25, 25, 25, 25);
                            txtView.SetTextColor(Color.Black);
                            txtView.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                            tableRow = new TableRow(this);
                            tableRow.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent);
                            tableRow.AddView(txtView);
                            tableLayout.AddView(tableRow);
                        }

                        var items = хdoc.Root.Elements("item")
                            .Where(x => (string)x.Attribute("brend") == "Vacon" 
                                && (string)x.Attribute("custom") == item.Attribute("custom").Value
                                && (string)x.Attribute("series") != "VACON 10" && (string)x.Attribute("series") != "VACON NXL")
                            .ToList();

                        if (items != null && items.Count > 0)
                        {
                            txtView = new TextView(this);
                            txtView.Text = "Аналог марки Vacon";
                            txtView.Gravity = GravityFlags.Left;
                            txtView.SetPadding(25, 50, 25, 25);
                            txtView.SetTextColor(Color.Black);
                            txtView.SetTextSize(Android.Util.ComplexUnitType.Dip, 20);
                            txtView.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

                            tableRow = new TableRow(this);
                            tableRow.AddView(txtView);

                            tableLayout.AddView(tableRow);

                            Toast.MakeText(this, "Найдено записей: " + items.Count.ToString(), ToastLength.Short).Show();
                            drawItems(items, tableLayout, tableRow);
                        }

                    }


                    hideKeyBoard();
                }


            };


            var imageHelp = FindViewById<ImageView>(Resource.Id.imageView2);
            
            imageHelp.Click += (sender, args) =>
            {
                FragmentTransaction ft = FragmentManager.BeginTransaction();
                //Remove fragment else it will crash as it is already added to backstack
                Fragment prev = FragmentManager.FindFragmentByTag("dialog");
                if (prev != null)
                {
                    ft.Remove(prev);
                }
                ft.AddToBackStack(null);
                // Create and show the dialog.
                DialogFragment1 newFragment = DialogFragment1.NewInstance(null);
                //No title
                newFragment.SetStyle(DialogFragmentStyle.NoTitle, 0);
                //Add fragment
                newFragment.Show(ft, "dialog");
            };
        }

        private void drawItems(List<XElement> items, TableLayout tableLayout, TableRow tableRow) {

            foreach (var it in items)
            {

                var codeData = new List<string>() {

                                it.Attribute("series").Value,
                                it.Attribute("custom").Value,
                                it.Attribute("model").Value,
                                it.Attribute("brend").Value
                            };

                for (int i = 0; i < codeData.Count; i++)
                {
                    var txt = "<b>";
                    if (i == 0)
                        txt += "Серия:";
                    else if (i == 1)
                        txt += "Заказной код:";
                    else if (i == 2)
                        txt += "Типовой код:";
                    else if (i == 3)
                        txt += "Бренд:";
                    txt += "<b> ";

                    txtView = new TextView(this);
                    txtView.TextFormatted = Html.FromHtml(txt + codeData[i]);
                    txtView.SetBackgroundResource(Resource.Layout.finded);
                    txtView.Gravity = GravityFlags.Left;
                    txtView.SetPadding(25, 25, 25, 25);
                    txtView.SetTextColor(Color.Black);
                    //txtView.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                    tableRow = new TableRow(this);
                    tableRow.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent);
                    tableRow.AddView(txtView);

                    tableLayout.AddView(tableRow);
                }

                txtView = new TextView(this);
                txtView.Text = "";
                txtView.Gravity = GravityFlags.Left;
                txtView.SetPadding(25, 25, 25, 25);
                txtView.SetTextColor(Color.Black);
                txtView.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                tableRow = new TableRow(this);
                tableRow.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.WrapContent, TableRow.LayoutParams.MatchParent);
                tableRow.AddView(txtView);

                tableLayout.AddView(tableRow);
            }
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
                            //выходим когда кол-во совпадений больше заданного ограничения
                            if (matchList.Count == limitRows.limValue)
                            {
                                matchList.Add(limitRows.limValueText);
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

    public static class limitRows
    {
        public static int limValue { get { return 20; } }

        public static string limValueText { get { return "Больше " + limValue.ToString() + " совпадений..."; } }
    }


}

