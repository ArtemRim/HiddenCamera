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
using HiddenCamera.Model;
using HiddenCamera.Media;

namespace HiddenCamera.Activities
{
    [Activity(Label = "SessionActivity")]
    public class SessionActivity : Activity
    {


        private LinearLayout layout;


        private int countVideos = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Session);
            string sessionName = Intent.GetStringExtra("sessionName");
            layout = FindViewById<LinearLayout>(Resource.Id.layout);
            GetMediViewsForLayout(sessionName);
        }




        private void GetMediViewsForLayout(string sessionName)
        {
            DirectoryInfo directoryFiles = new DirectoryInfo(GetPathToFilesOfSession(sessionName));
            foreach (var file in directoryFiles.GetFiles())
            {
                if (DefineFormatFile(file))
                    LoadPictureToImageView(file.ToString());
                else
                    LoadVideoToVideoView(file.ToString());
            }
        }


        private bool DefineFormatFile(FileInfo file)
        {

            if (file.Extension == ".png")
                return true;
            else
                return false;
        }


        public void LoadPictureToImageView(string path)
        {
            ImageView imageView = Photo.CreateImageView(this);
            imageView.SetImageBitmap(Photo.GetPictureFromFile(path));
            layout.AddView(imageView);
        }



        public void LoadVideoToVideoView(string path)
        {
            VideoViewExtends videoView = Video.CreateVideoView(this);
            videoView.SetVideoPath(path);
            videoView.Id = ++countVideos;
            videoView.InitPicture();
            videoView.SetEventOnTouch();
            layout.AddView(videoView);
        }



        private string GetPathToFilesOfSession(string sessionName)
        {
            var directoryApp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Java.IO.File sessionPath = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/" + directoryApp + "/" + sessionName);
            return sessionPath.ToString();
        }
    }
}