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
using HiddenCamera.Media;
using System.Timers;
using HiddenCamera.Model;

namespace HiddenCamera.Activities
{
    [Activity(Label = "HiddenActivity")]
    public class HiddenModeActivity : Activity
    {
        private String sessionPath;
        private Button buttonStart;
        private Button buttonStop;
        private Video video;
        private Timer timerUpdateVideoView, timerUpdateSurface;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.hidden_mode);
            sessionPath = File.CreateDirectory(GetSessionName(), 0);
            buttonStart = FindViewById<Button>(Resource.Id.button1);
            SetSurfaceHolder(false);
            buttonStart.Click += delegate
            {
                try
                {
                    StartVideoRecord();
                }
                catch { throw new Exception("on click event"); }
            };
            buttonStop = FindViewById<Button>(Resource.Id.button2);
            buttonStop.Click += delegate
            {
                CheckOnStartedRecord();
            };
        }


        public void SetSurfaceHolder(bool isCallbacFunckDelete)
        {
            SurfaceView surfaceView = Photo.CreateSurfaceView(this);
            layout.AddView(surfaceView);
            surfaceHolder = surfaceView.Holder;
            surfaceHolder.AddCallback(this);
            surfaceHolder.SetType(SurfaceType.PushBuffers);
        }

        public void CheckOnStartedRecord()
        {
            video.StopRecord();         
        } 

        private void StartVideoRecord()
        {
            try
            {
                video = new Video(".mp4", sessionPath);
                SurfaceView surfaceview = Photo.CreateSurfaceView(this);
                video.SufaceHolder = surfaceview.Holder;
                StartTimer(ref timerUpdateVideoView, timerUpdateVideoView_Elapsed);
            }
            catch { throw new Exception("on start"); };
        }



        public void StartTimer(ref Timer timer, ElapsedEventHandler Event)
        {
            timer = new Timer();
            timer.Interval = 300;
            timer.Elapsed += new ElapsedEventHandler(Event);
            timer.Start();
        }


        private void timerUpdateVideoView_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerUpdateVideoView.Stop();
            video.StartRecord();
        }

        public string GetSessionName()
        {
            string sessionName = Intent.GetStringExtra("sessionName");
            if (sessionName == "")
                sessionName = DateTime.Now.ToString();
            return sessionName;
        }
    }
}