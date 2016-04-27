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
using HiddenCamera.Model;

namespace HiddenCamera.Adapter
{
    class SessionArrayAdapter : ArrayAdapter<Session>
    {
        private Activity context;
        private Session[] sessions;

        public SessionArrayAdapter(Activity context, Session[] sessions)
            : base(context, Resource.Layout.row_session, sessions)
        {

            this.context = context;
            this.sessions = sessions;
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            LayoutInflater inflater = context.LayoutInflater;
            View rowView = inflater.Inflate(Resource.Layout.row_session, null, true);
            LinearLayout cell = (LinearLayout)rowView.FindViewById(Resource.Id.cell);
            TextView sessionNameTextView = (TextView)rowView.FindViewById(Resource.Id.item);
            ImageView imageView = (ImageView)rowView.FindViewById(Resource.Id.icon);
            sessionNameTextView.Text = sessions[position].SessionName;
            imageView.SetImageBitmap(sessions[position].Image);
            return rowView;
        }

        public Session GetItemAtPosition(int position)
        {
            return sessions[position];
        }
    }
}