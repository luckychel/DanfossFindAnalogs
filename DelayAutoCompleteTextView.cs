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
        public static Context _Context {get; set;}
        private static Android.Util.IAttributeSet _Attrs { get; set; }

        public DelayAutoCompleteTextView(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
            _Context = context;
            _Attrs = attrs;
        }

        private static int MESSAGE_TEXT_CHANGED = 100;
        private static int DEFAULT_AUTOCOMPLETE_DELAY = 750;

        private int mAutoCompleteDelay = DEFAULT_AUTOCOMPLETE_DELAY;
        private ProgressBar mLoadingIndicator;

        public class mHandler : Handler
        {
            public override void HandleMessage(Message msg)
            {
                //DelayAutoCompleteTextView.base.PerformFiltering((Java.Lang.ICharSequence)msg.Obj, msg.Arg1);
            }
        }

        public void setLoadingIndicator(ProgressBar progressBar)
        {
            mLoadingIndicator = progressBar;
        }

        public void setAutoCompleteDelay(int autoCompleteDelay)
        {
            mAutoCompleteDelay = autoCompleteDelay;
        }

        protected override void PerformFiltering(Java.Lang.ICharSequence text, int keyCode)
        {
            if (mLoadingIndicator != null)
            {
                mLoadingIndicator.Visibility = ViewStates.Visible;
            }

            //mHandler.removeMessages(MESSAGE_TEXT_CHANGED);
            //mHandler.sendMessageDelayed(mHandler.obtainMessage(MESSAGE_TEXT_CHANGED, text), mAutoCompleteDelay);
        }

        public override void OnFilterComplete(int count)
        {
            if (mLoadingIndicator != null)
            {
                mLoadingIndicator.Visibility = ViewStates.Gone;
            }

            base.OnFilterComplete(count);
        }
    }
}
