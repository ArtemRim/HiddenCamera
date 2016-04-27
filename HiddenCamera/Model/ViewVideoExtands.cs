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
using System.Timers;
using System.IO;

namespace HiddenCamera.Model
{
    class VideoViewExtends : VideoView
    {
        private Context context;
        private VideoViewState state;
        private Timer timer;
        private int seconds = 0;
        private const int countSeconds = 2;
        public bool isShotVideo = false;

        public VideoViewState State
        {
            get { return state; }
            set { state = value; }
        }


        public VideoViewExtends(Context context)
            : base(context)
        {
            this.context = context;
            this.state = new VideoViewState();

        }


        public VideoViewExtends(Context context, string path)
            : base(context)
        {
            this.context = context;
            this.state = new VideoViewState();
            this.SetVideoPath(path);
            this.InitPicture();
            this.SetEventOnTouch();
        }

        public override void SetVideoPath(string path)
        {
            base.SetVideoPath(path);
            FileInfo file = new FileInfo(path);
            UInt64 size = (UInt64)file.Length;
            if (size < 15000)
                this.isShotVideo = true;
        }

        public void InitPicture()
        {

            if (!this.isShotVideo)
            {
                StartShowVideo(100);
                this.Pause();
            }
            else
                StartShowVideo(0);

        }

        public void StartShowVideo(int Position)
        {
            this.SeekTo(Position);
            this.Start();
        }

        public void SetEventOnTouch()
        {
            this.Touch += (sender, args) =>
            {
                if (args.Event.Action == MotionEventActions.Down)
                    StartTimer();
                if (args.Event.Action == MotionEventActions.Up)
                {
                    if (seconds >= countSeconds)
                        OnStop();
                    else
                        this.SetNewState();
                    StopTimer();
                }
            };
        }


        private void StartTimer()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        private void StopTimer()
        {
            timer.Stop();
            seconds = 0;
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            seconds++;
            if (seconds > countSeconds)
                timer.Stop();
        }


        public void SetNewState()
        {
            switch (this.state)
            {
                case VideoViewState.Playing:
                    OnPause();
                    break;
                case VideoViewState.Stop:
                    OnStart();
                    break;
                case VideoViewState.Resume:
                    OnResume();
                    break;
            }
        }


        public void OnStart()
        {
            this.Start();
            this.state = VideoViewState.Playing;
        }

        public void OnPause()
        {
            this.Pause();
            this.state = VideoViewState.Resume;
        }

        public void OnStop()
        {
            this.SeekTo(0);
            this.Start();
            this.state = VideoViewState.Stop;
        }

        public void OnResume()
        {
            this.SeekTo(this.CurrentPosition);
            this.Start();
            this.state = VideoViewState.Playing;
        }


    }
}