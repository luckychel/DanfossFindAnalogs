using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using System;
using System.Text;

namespace CompetitorTool
{
    [Activity(Label = "Competitor Tool"
            , Theme = "@style/MyCustomTheme"
            , Icon = "@drawable/Icon"
            , ConfigurationChanges = (Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize))]
    public class MailForm : Activity
    {
        private Button btnSend;
        private EditText editFIO;
        private EditText editOrganization;
        private EditText editPhone;
        private EditText editEmail;
        private EditText editComment;
        private ImageView imageHelp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.mailForm);

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

            btnSend = FindViewById<Button>(Resource.Id.btnSend);
            editFIO = FindViewById<EditText>(Resource.Id.editFIO);
            editOrganization = FindViewById<EditText>(Resource.Id.editOrganization);
            editEmail = FindViewById<EditText>(Resource.Id.editEmail);
            editPhone = FindViewById<EditText>(Resource.Id.editPhone);
            editComment = FindViewById<EditText>(Resource.Id.editComment);
            imageHelp = FindViewById<ImageView>(Resource.Id.imageView2);

            btnSend.Click += btnSend_Click;
            imageHelp.Click += ImageHelp_Click;

        }

        private void ImageHelp_Click(object sender, EventArgs e)
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
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(editFIO.Text.Trim()))
            {
                Toast.MakeText(this, "Введите ваше ФИО", ToastLength.Long).Show();
                return;
            }
            if (string.IsNullOrEmpty(editEmail.Text.Trim()) && string.IsNullOrEmpty(editPhone.Text.Trim()))
            {
                Toast.MakeText(this, "Введите Email или телефон", ToastLength.Long).Show();
                return;
            }

            var email = new Intent(Android.Content.Intent.ActionSend);

            email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { "luckychel@gmail.com" }); //pe@danfoss.ru

            email.PutExtra(Android.Content.Intent.ExtraSubject, "Расчёт для " + editFIO.Text + (!string.IsNullOrEmpty(editOrganization.Text) ? " из " + editOrganization.Text : ""));

            var tableData = Intent.GetStringExtra("FindedData");

            email.PutExtra(Android.Content.Intent.ExtraText,
                Html.FromHtml(new StringBuilder()
                    .Append("<p><b>Email:</b> " + editEmail.Text + "</p>")
                    .Append("<p><b>Телефон:</b> " + editPhone.Text + "</p>")
                    .Append("<p><b>Комментарий:</b> " + editComment.Text + "</p>")
                    .Append("<p><b>Результат подбора:</b> " + "" + "</p>")
                    .ToString())
            );

            email.SetType("message/rfc822");

            StartActivity(Intent.CreateChooser(email, "Выберите почтовый клиент"));
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {

            base.OnConfigurationChanged(newConfig);

            hideKeyBoard();
        }

        public override void OnBackPressed()
        {
            //OverridePendingTransition(Android.Resource.Animation.SlideInLeft, Android.Resource.Animation.SlideOutRight);
            base.OnBackPressed();
        }

        private void hideKeyBoard()
        {
            InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Activity.InputMethodService);
            var currentFocus = this.CurrentFocus;
            if (currentFocus != null)
            {
                inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
            }
        }
    }
}