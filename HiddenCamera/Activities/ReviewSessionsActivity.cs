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
using System.IO;
using Android.Graphics;
using HiddenCamera.Adapter;
using HiddenCamera.Model;

namespace HiddenCamera.Activities
{
    [Activity(Label = "ReviewSession")]
    public class ReviewSessions : Activity
    {

        private ListView listView;
        private SessionArrayAdapter adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ListSession);
            adapter = new SessionArrayAdapter(this, GetAllSession());
            listView = FindViewById<ListView>(Resource.Id.listView);
            listView.SetAdapter(adapter);
            listView.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                var selectedFromList = this.adapter.GetItemAtPosition(e.Position);
                PassDataToCameraActivity(selectedFromList.SessionName);
            };


        }


        private void PassDataToCameraActivity(String sessionName)
        {
            Intent intent = new Intent(this, typeof(SessionActivity));
            intent.PutExtra("sessionName", sessionName);
            StartActivity(intent);
        }





        private Session[] GetAllSession()
        {
            DirectoryInfo dir = new DirectoryInfo(GetPathToSessions());
            return GetDirectories(dir);
        }


        private Session[] GetDirectories(DirectoryInfo dir)
        {
            List<Session> sessions = new List<Session>();
            foreach (var item in dir.GetDirectories())
            {
                sessions.Add(GetOneImageFromSession(item));
            }
            return sessions.ToArray();
        }


        private Session GetOneImageFromSession(DirectoryInfo item)
        {
            Session session = new Session();
            foreach (var file in item.GetFiles())
            {
                BitmapFactory.Options options = new BitmapFactory.Options();
                options.InPreferredConfig = Bitmap.Config.Argb8888;
                session.Image = BitmapFactory.DecodeFile(file.ToString(), options);
                break;
            }
            CheckOnEmpty(session);
            session.SessionName = item.ToString().Split('/')[8];
            return session;
        }


        private void CheckOnEmpty(Session session)
        {
            if (session.Image == null)
                session.Image = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.undefined);
        }


        private string GetPathToSessions()
        {
            var directoryApp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Java.IO.File sessionPath = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/" + directoryApp);
            return sessionPath.ToString();
        }
    }
}