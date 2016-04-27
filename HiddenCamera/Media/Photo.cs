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
using Android.Graphics;
using System.IO;
using Android.Util;
using Android.Content.Res;
using HiddenCamera.Activities;


namespace HiddenCamera.Media
{
    class Photo : File
    {
        const string formatPng = ".png";
        private Bitmap picture;
        private string formatPicture;
        private string folder;
        public Bitmap Picture
        {
            get { return picture; }
            set { picture = value; }
        }

        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }

        public Photo() { }


        public Photo(Bitmap picture, string formatPicture, string sessionPath)
        {
            this.picture = picture;
            this.folder = sessionPath;
            this.formatPicture = formatPicture;
            SavePicture();
        }

        public void SavePicture()
        {
            WritePhotoToFile(folder);
        }


        private void WritePhotoToFile(string pathDirectory)
        {
            var stream = new FileStream(GetPathFile(pathDirectory, formatPng), FileMode.Create);
            picture.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();

        }


        public static ImageView CreateImageView(Context context)
        {
            ImageView imageView = new ImageView(context);
            var LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            LayoutParameters.SetMargins(0, 30, 0, 30);
            imageView.LayoutParameters = LayoutParameters;
            imageView.Visibility = ViewStates.Visible;
            imageView.Click += delegate { };
            return imageView;
        }


        public static SurfaceView CreateSurfaceView(Activity context)
        {
            SurfaceView surfaceView = new SurfaceView(context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(1, 1),
            };
            return surfaceView;
        }



        public static Bitmap GetPictureFromFile(string path)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            return (Bitmap)BitmapFactory.DecodeFile(path, options);
        }

    }
}