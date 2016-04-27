using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace HiddenCamera.Activities
{
    [Activity(Label = "HiddenCamera", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            Button buttonCreate = FindViewById<Button>(Resource.Id.btn_create_session);
            Button buttonShow = FindViewById<Button>(Resource.Id.btn_show_session);
            buttonCreate.Click += delegate
            {
                CreateAlertDialog();
            };
            buttonShow.Click += delegate
            {
                StartActivity(typeof(ReviewSessions));
            };
        }

        private void PassDataToCameraActivity(String sessionName)
        {
            Intent intent = new Intent(this, typeof(CameraActivity));
            intent.PutExtra("sessionName", sessionName);
            StartActivity(intent);
        }


        private void CreateAlertDialog()
        {
            Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog dialog = builder.Create();
            dialog.SetMessage(GetString(Resource.String.NameSession));
            var edit = CreateEditText();
            dialog.SetView(edit);
            dialog.SetButton(GetString(Resource.String.StartSession), (s, ev) =>
            {
                String sessionName = edit.Text.ToString();
                PassDataToCameraActivity(sessionName);
            });
            dialog.SetButton2(GetString(Resource.String.ReturnToMenu), (s, ev) =>
            {
            });
            dialog.Show();
        }



        private EditText CreateEditText()
        {
            EditText editView = new EditText(this);
            var layoutParams = new LinearLayout.LayoutParams(100, 25) { LeftMargin = 20 };
            editView.LayoutParameters = layoutParams;
            return editView;
        }


    }
}

