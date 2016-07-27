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

namespace DanfossFindAnalogs
{

    public class DelayAutoCompleteTextView : AutoCompleteTextView 
    {
        public Context _Context {get; set;}
        private Android.Util.IAttributeSet _Attrs { get; set; }

        public DelayAutoCompleteTextView(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
            _Context = context;
            _Attrs = attrs;
        }

        private static int messageTextChanged = 100;
        private static int defaultAutocompleteDelay = 750;

        private int _autoCompleteDelay = defaultAutocompleteDelay;
        private ProgressBar _loadingIndicator;

        private readonly Handler mHandler = new Handler(delegate (Message msg) {
            
            //base.PerformFiltering((Java.Lang.ICharSequence)msg.Obj, msg.Arg1);

        });

        public void setLoadingIndicator(ProgressBar progressBar)
        {
            _loadingIndicator = progressBar;
        }

        public void setAutoCompleteDelay(int autoCompleteDelay)
        {
            _autoCompleteDelay = autoCompleteDelay;
        }
        
        protected override void PerformFiltering(Java.Lang.ICharSequence text, int keyCode)
        {
            if (_loadingIndicator != null)
            {
                _loadingIndicator.Visibility = ViewStates.Visible;
            }
            
            mHandler.RemoveMessages(messageTextChanged);
            mHandler.SendMessageDelayed(mHandler.ObtainMessage(messageTextChanged, (Java.Lang.Object)text), _autoCompleteDelay);
        }

        public override void OnFilterComplete(int count)
        {
            if (_loadingIndicator != null)
            {
                _loadingIndicator.Visibility = ViewStates.Gone;
            }

            base.OnFilterComplete(count);
        }
    }
}
