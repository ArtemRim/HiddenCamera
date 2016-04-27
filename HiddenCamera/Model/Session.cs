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
namespace HiddenCamera.Model
{
    class Session
    {

        private string sessionName;
        private Bitmap image;

        public Bitmap Image
        {
            get { return image; }
            set { image = value; }
        }


        public string SessionName
        {
            get { return sessionName; }
            set { sessionName = value; }
        }
        public Session()
        {
        }

        public Session(string sessionName, Bitmap image)
        {
            this.sessionName = sessionName;
            this.image = image;
        }
    }
}