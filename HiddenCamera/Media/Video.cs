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
using Android.Media;
using Android.Util;
using Android.Content.Res;
using HiddenCamera.Model;

namespace HiddenCamera.Media
{
    class Video : Photo
    {
        public VideoViewExtends videoView;

        private MediaRecorder recorder;
        private Android.Hardware.Camera camera;
        public string path;
        public ISurfaceHolder SufaceHolder;
        public bool startRecord;

        public Video(string format, string sessionPath)
        {
            startRecord = false;
            this.path = GetPathFile(sessionPath, format);
            InitCamera();
        }


        public void StartRecord()
        {
            SetRecorderProperties();
            SetRecorderPreview();
            recorder.Start();
            startRecord = true;
        }

        private void InitCamera()
        {
            try
            {
                camera = Android.Hardware.Camera.Open();
                camera.SetDisplayOrientation(90);
                camera.Unlock();
            }
            catch { }
        }

        public void SetRecorderPreview()
        {
            recorder.SetPreviewDisplay(SufaceHolder.Surface);
            recorder.Prepare();
        }


        private void SetRecorderProperties()
        {
            recorder = new MediaRecorder();
            recorder.SetCamera(camera);
            recorder.SetVideoSource(VideoSource.Camera);
            recorder.SetAudioSource(AudioSource.Mic);
            recorder.SetOutputFormat(OutputFormat.Default);
            recorder.SetVideoEncoder(VideoEncoder.H264);
            recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            recorder.SetOutputFile(path);
            recorder.SetOrientationHint(90);
        }


        public void StopRecord()
        {
            if (recorder != null)
            {
                recorder.Stop();
                recorder.Release();
                camera.StopPreview();
                camera.Release();
            }
        }



        public void Dispose()
        {

            if (recorder != null)
            {
                recorder.Release();
                recorder.Dispose();
                recorder = null;
            }
        }

        public void ReleaseCamera()
        {
            if (camera != null)
            {
                camera.StopPreview();
                camera.Release();
            }
        }



        public static VideoViewExtends CreateVideoView(Context context)
        {

            VideoViewExtends videoView = new VideoViewExtends(context);
            SetVideoViewParams(videoView);
            return videoView;
        }

        private static void SetVideoViewParams(VideoViewExtends videoView)
        {
            var metrics = Resources.System.DisplayMetrics;
            var LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, metrics.HeightPixels - 300);
            LayoutParameters.SetMargins(0, 30, 0, 30);
            videoView.LayoutParameters = LayoutParameters;
            //videoView.SetPadding(0, 30, 0, 30);
            videoView.StopPlayback();
        }


        public static VideoViewExtends CreateVideoView(Context context, string path)
        {
            VideoViewExtends videoView = new VideoViewExtends(context, path);
            SetVideoViewParams(videoView);
            return videoView;
        }
    }
}