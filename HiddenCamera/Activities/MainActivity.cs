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
        private const int MENU_HIDDEN_MODE = 113;
        private const int MENU_OPEN_MODE = 114;

        private Button buttonShow;
        private Button buttonCreate;

        private String sessionName;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            buttonCreate = FindViewById<Button>(Resource.Id.btn_create_session);
            buttonShow = FindViewById<Button>(Resource.Id.btn_show_session);
            this.RegisterForContextMenu(buttonCreate);
            buttonCreate.Click += delegate
            {
                CreateAlertDialog();
            };
            buttonShow.Click += delegate
            {
                StartActivity(typeof(ReviewSessions));
            };
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            menu.Add(0, MENU_HIDDEN_MODE, 0, GetString(Resource.String.menu_hidden_mode));
            menu.Add(0, MENU_OPEN_MODE, 0, GetString(Resource.String.menu_open_mode));
            base.OnCreateContextMenu(menu, v, menuInfo);
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case MENU_HIDDEN_MODE:
                    PassDataToCameraActivity(sessionName, new Intent(this, typeof(HiddenModeActivity)));
                    break;
                case MENU_OPEN_MODE:
                    PassDataToCameraActivity(sessionName,new Intent(this, typeof(CameraActivity)));
                    break;
            }
            return base.OnContextItemSelected(item);
        }

        private void PassDataToCameraActivity(String sessionName,Intent intent)
        {
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
                sessionName = edit.Text.ToString();
                OpenContextMenu(buttonCreate);
                
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

