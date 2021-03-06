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
    public class HiddenModeActivity : Activity, ISurfaceHolderCallback, Android.Hardware.Camera.IPictureCallback
    {
        private String sessionPath;
        private Button buttonStart;
        private LinearLayout layout;
        private RadioButton radio_video,radio_photo;
        private Spinner spinner;
        private Video video;
        private Timer timerTakePicture;
        private ISurfaceHolder surfaceHolder;
        private Android.Hardware.Camera camera;
        private bool previewing = false, cameraWasClosed = false, isRecording=false,isTakingPictures = false;
        private const int DIALOG_TIME = 11;
        private Int64 interval = 0;
        private const Int32 seconds = 60;
        private const Int32 millisec = 1000;
        int myHour = 14;
        int myMinute = 35;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.hidden_mode);
            sessionPath = File.CreateDirectory(GetSessionName(), 0);
            buttonStart = FindViewById<Button>(Resource.Id.button1);
            radio_video = FindViewById<RadioButton>(Resource.Id.radioButton_video);
            radio_photo = FindViewById<RadioButton>(Resource.Id.radioButton_photo);
            layout = FindViewById<LinearLayout>(Resource.Id.layout);

            SetSurfaceHolder(false);


            buttonStart.Click += DefineAction;
            radio_photo.Click += RadioButtonPhotoChange;
            radio_video.Click += RadioButtonVideoChange;

            InitSpinner();
        }

        private void InitSpinner()
        {
            ArrayAdapter adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.intervals_array, Resource.Layout.spinner_item);
            spinner = (Spinner)FindViewById(Resource.Id.spinner1);
            adapter.SetDropDownViewResource(Resource.Layout.spinner_dropdown_item);
            spinner.Adapter = adapter;
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            spinner.Visibility = ViewStates.Invisible;
        }




        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            String item =  spinner.GetItemAtPosition(e.Position).ToString();
            interval = Int64.Parse(item.Split(' ')[0]) * millisec;
            if (item.Contains("���"))
                interval *= seconds;
            Toast.MakeText(this, String.Format("�������� ����� �������� {0} ", item), ToastLength.Short).Show();
        }
   
        
        private void RadioButtonPhotoChange(object sender, EventArgs args)
        {
            if (radio_photo.Checked)
            {
                radio_video.Checked = false;
                spinner.Visibility = ViewStates.Visible;
            }               
        }

        private void RadioButtonVideoChange(object sender, EventArgs args)
        {
            if (radio_video.Checked)
            {
                radio_photo.Checked = false;           
                spinner.Visibility = ViewStates.Invisible;
            }
        }






        public string GetSessionName()
        {
            string sessionName = Intent.GetStringExtra("sessionName");
            if (sessionName == "")
                sessionName = DateTime.Now.ToString();
            return sessionName;
        }


        private void DefineAction(object sender,EventArgs args)
        {
            StartVideoRecordOnRadioVideoChecked();
            TakePictureOnRadioPhotoChecked();
        }

        private void StartVideoRecordOnRadioVideoChecked()
        {
            if (radio_video.Checked)
            {
                if (!isRecording)
                    StartVideoRecord();
                else
                    StopVideoRecord();
            }
        }

        private void StartVideoRecord()
        {
            try
            {
                if (!cameraWasClosed)
                    CameraClose(false);
                video = new Video(sessionPath);
                video.SufaceHolder = surfaceHolder;
                video.StartRecord();
                cameraWasClosed = true;
                isRecording = true;
                buttonStart.Text = GetString(Resource.String.stop_record);
            }
            catch { };
        }



        private void StopVideoRecord()
        {
            if (video.startRecord)
            {
                video.StopRecord();
                isRecording = false;
                buttonStart.Text = GetString(Resource.String.start_record);
            }
        }

        private void TakePictureOnRadioPhotoChecked()
        {
            if (radio_photo.Checked)
            {
                if (!isTakingPictures)
                    StartMakingPictures();
                else
                    StopTakingPictures();
            }
        }


        private void StartMakingPictures()
        {
            StartTimer(ref timerTakePicture, timerTakePicture_Elapsed);
            isTakingPictures = true;
            buttonStart.Text = GetString(Resource.String.stop_record);
        }



        private void StopTakingPictures()
        {
            timerTakePicture.Stop();
            CameraClose(false);
            isTakingPictures = false;
            buttonStart.Text = GetString(Resource.String.start_record);
        }

        public void StartTimer(ref Timer timer, ElapsedEventHandler Event)
        {
            timer = new Timer();
            timer.Interval = interval;
            timer.Elapsed += new ElapsedEventHandler(Event);
            timer.Start();
        }


        private void timerTakePicture_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {           
            TakePicture();
        }

        private void TakePicture()
        {
            camera.StartPreview();
            camera.TakePicture(null, null, this);
        }

        public void OnPictureTaken(byte[] data, Android.Hardware.Camera camera)
        {
            Bitmap bitmapPicture = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            bitmapPicture = Bitmap.CreateScaledBitmap(bitmapPicture, Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels / 2, false);
            Photo picture = new Photo(bitmapPicture, ".png", sessionPath);     
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
            if ((!isRecording)&&(!isTakingPictures))
                camera = Android.Hardware.Camera.Open();
        }



        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
           // CameraClose(true);
           // previewing = false;
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


    }
}