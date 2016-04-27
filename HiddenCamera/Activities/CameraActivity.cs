using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using System.Timers;
using Android.Views;
using Android.Util;

using Android.Hardware;
using Android.Content;
using Android.Media;
using Java.IO;
using System.Threading.Tasks;
using Java.Lang;
using System;
using Android.Graphics;
using System.IO;
using HiddenCamera.Model;
using Android.Content.Res;
using HiddenCamera.Media;

namespace HiddenCamera.Activities
{
    [Activity(Label = "CameraActivity")]
    public class CameraActivity : Activity, ISurfaceHolderCallback, Android.Hardware.Camera.IPictureCallback
    {

        public ImageView currentImageView;
        public Android.Widget.LinearLayout layout;

        private ISurfaceHolder surfaceHolder = null;
        private Android.Hardware.Camera camera;
        Timer timerUpdateVideoView, timerUpdateSurface;

        private bool previewing = false;
        private bool isVideo = false;
        private bool cameraWasClosed = false;


        Video video;

        private string sessionPath;

        int countVideo = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Camera);           
            GetSessionName();
            sessionPath = File.CreateDirectory(GetSessionName(), 0);
            layout = FindViewById<Android.Widget.LinearLayout>(Resource.Id.layoutBase);
            if (surfaceHolder == null)
                SetSurfaceHolder(false);
            SetOnLayoutClickEvent();
            SetOnLayoutLongClickEvent();
            //  SetOnTouchLayoutEvents();
 
        }



        private void SetOnLayoutLongClickEvent()
        {
            layout.LongClick += delegate
            {
                try
                {
                    isVideo = true;
                    StartVideoRecord();
                }
                catch { throw; }
            };
        }



        private void SetOnLayoutClickEvent()
        {
            layout.Click += delegate
            {
                try
                {
                    InsertImageViewInLayout();
                    if (cameraWasClosed)
                    {
                        SetSurfaceHolder(false);
                        cameraWasClosed = false;
                        StartTimer(ref timerUpdateSurface, timerUpdateSurfaceView_Elapsed);
                    }
                    else
                        camera.TakePicture(null, null, this);
                }
                catch { }
            };
        }



        private void SetOnTouchLayoutEvents()
        {
            layout.Touch += (sender, args) =>
            {
                args.Handled = false;
                if (args.Event.Action == MotionEventActions.Up)
                {
                    try
                    {
                        if (isVideo)
                        {
                            CheckOnStartedRecord();
                            isVideo = false;
                        }
                    }
                    catch { }
                }
            };
        }




        public void CheckOnStartedRecord()
        {
            if (video.startRecord)
            {
                video.StopRecord();
                VideoViewExtends videoView = Video.CreateVideoView(this, video.path);
                videoView.Id = ++countVideo;
                layout.AddView(videoView);
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
            camera = Android.Hardware.Camera.Open();
            //camera.SetDisplayOrientation(90);

        }



        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            CameraClose(true);
            previewing = false;
        }


        public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
        {
            Bitmap bitmapPicture = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            int width = 0, height = 0;
            DefineOrientation(ref width, ref height);
            bitmapPicture = Bitmap.CreateScaledBitmap(bitmapPicture, Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels / 2, false);
            currentImageView.SetImageBitmap(bitmapPicture);
            Photo picture = new Photo(bitmapPicture, ".png", sessionPath);
            SetSurfaceHolder(true);
            CameraClose(false);
        }



        public void DefineOrientation(ref int width, ref int height)
        {
            var metrics = Resources.DisplayMetrics;
            //if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
            //{
            ////    width = metrics.WidthPixels;
            ////    height = metrics.HeightPixels / 2;
            //}
            //else
            //{
            //    width = metrics.WidthPixels / 2;
            //    height = metrics.HeightPixels;
            //}
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

        public void InsertImageViewInLayout()
        {
            currentImageView = Photo.CreateImageView(this);
            layout.AddView(currentImageView);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (video!=null)
               video.Dispose();
        }






        private void timerUpdateSurfaceView_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerUpdateSurface.Stop();
            camera.TakePicture(null, null, this);
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
                video.SufaceHolder = Photo.CreateSurfaceView(this).Holder;
                StartTimer(ref timerUpdateVideoView, timerUpdateVideoView_Elapsed);
                cameraWasClosed = true;
            }
            catch { };
        }
    }
}