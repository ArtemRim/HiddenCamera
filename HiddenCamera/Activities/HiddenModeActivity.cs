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
using Android.Graphics;

namespace HiddenCamera.Activities
{
    [Activity(Label = "HiddenActivity")]
    public class HiddenModeActivity : Activity,ISurfaceHolderCallback
    {
        private String sessionPath;
        private Button buttonStart,buttonStop;
        private LinearLayout layout;
        private Video video;
        private Timer timerUpdateVideoView, timerUpdateSurface;
        private ISurfaceHolder surfaceHolder;
        private Android.Hardware.Camera camera;
        private bool previewing = false, cameraWasClosed = false,start = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.hidden_mode);
            sessionPath = File.CreateDirectory(GetSessionName(), 0);
            buttonStart = FindViewById<Button>(Resource.Id.button1);
            layout = FindViewById<LinearLayout>(Resource.Id.layout);
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
                start = false;
            };
        }



        public void CheckOnStartedRecord()
        {
            if (video.startRecord)
            {
                video.StopRecord();
            }
            else
            {
                video.Dispose();
                video.ReleaseCamera();
            }
        }



        public string GetSessionName()
        {
            string sessionName = Intent.GetStringExtra("sessionName");
            if (sessionName == "")
                sessionName = DateTime.Now.ToString();
            return sessionName;
        }


        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {
            try
            {
                if (previewing)
                {
                    camera.StopPreview();
                    previewing = false;
                }
                if (camera != null)
                {
                    camera.SetPreviewDisplay(surfaceHolder);
                    camera.StartPreview();
                    previewing = true;
                }
            }
            catch { }
        }


        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (!start)
                camera = Android.Hardware.Camera.Open();
            //camera.SetDisplayOrientation(90);

        }



        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            CameraClose(true);
            previewing = false;
        }






        public void SetSurfaceHolder(bool isCallbacFunckDelete)
        {
            if (isCallbacFunckDelete)
                surfaceHolder.RemoveCallback(this);
            SurfaceView surfaceView = Photo.CreateSurfaceView(this);
            layout.AddView(surfaceView);
            surfaceHolder = surfaceView.Holder;
            surfaceHolder.AddCallback(this);
            surfaceHolder.SetType(SurfaceType.PushBuffers);
        }



        private void CameraClose(bool isPreviewClosing)
        {
            if (camera != null)
            {
                if (isPreviewClosing)
                    camera.StopPreview();
                camera.Release();
                camera = null;
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (video != null)
                video.Dispose();
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


        private void StartVideoRecord()
        {
            try
            {
                if (!cameraWasClosed)
                    CameraClose(false);
                video = new Video(".mp4", sessionPath);
                SurfaceView surfaceView = Photo.CreateSurfaceView(this);
                layout.AddView(surfaceView);
                video.SufaceHolder = surfaceView.Holder;
                StartTimer(ref timerUpdateVideoView, timerUpdateVideoView_Elapsed);
                cameraWasClosed = true;
                start = true;
            }
            catch { };
        }



    }
}