using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Xamarin.Forms;
using NARA.Views;
using Android.Content;
using NARA.Util;
using System.Net;

namespace NARA.Droid
{
    [Activity(Label = "Remembering WWI", Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.AdjustResize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        //Icon = "@drawable/icon
        private bool _allowPortrait = true;
        private bool _ProceedToCustomRotation = false;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
            ActionBar.SetIcon(new ColorDrawable(Android.Graphics.Color.Transparent));
            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);

            if (NaraTools.Environment == Environments.Testing)
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }

            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "end", sender =>
            {
                _ProceedToCustomRotation = false;
                _allowPortrait = true;
                LockRotation(Android.Content.Res.Orientation.Undefined);
            });

            //When the page is closed this is called
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "start", sender =>
            {
                _ProceedToCustomRotation = true;
                _allowPortrait = false;
                OnConfigurationChanged(new Configuration());
            });
            MessagingCenter.Subscribe<App>((App)Xamarin.Forms.Application.Current, "makesure", sender =>
            {
                _ProceedToCustomRotation = false;
                   _allowPortrait = true;
                LockRotation(Android.Content.Res.Orientation.Undefined);
            });

        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            if (_ProceedToCustomRotation)
            {
                switch (newConfig.Orientation)
                {
                    case Android.Content.Res.Orientation.Landscape:
                        switch (Device.Idiom)
                        {
                            case TargetIdiom.Phone:
                                if (!_allowPortrait)
                                {
                                    LockRotation(Android.Content.Res.Orientation.Portrait);
                                }
                                break;
                            case TargetIdiom.Tablet:
                                LockRotation(Android.Content.Res.Orientation.Landscape);
                                break;
                        }
                        break;
                    case Android.Content.Res.Orientation.Portrait:
                        switch (Device.Idiom)
                        {
                            case TargetIdiom.Phone:
                                if (!_allowPortrait)
                                {
                                    LockRotation(Android.Content.Res.Orientation.Portrait);
                                }
                                break;
                            case TargetIdiom.Tablet:
                                if (!_allowPortrait)
                                {
                                    LockRotation(Android.Content.Res.Orientation.Landscape);
                                }
                                else
                                {
                                    LockRotation(Android.Content.Res.Orientation.Undefined);
                                }
                                break;
                        }
                        break;
                    case Android.Content.Res.Orientation.Undefined:
                        if (_allowPortrait)
                        {
                            LockRotation(Android.Content.Res.Orientation.Undefined);
                        }
                        else
                        {
                            LockRotation(Android.Content.Res.Orientation.Landscape);
                        }
                        //if (Device.Idiom == TargetIdiom.Phone && _allowPortrait)
                        //{
                        //    LockRotation(Android.Content.Res.Orientation.Landscape);
                        //}
                        //else if (Device.Idiom == TargetIdiom.Phone && !_allowPortrait)
                        //{
                        //    LockRotation(Android.Content.Res.Orientation.Portrait);
                        //}
                        break;
                }
            }
        }

        private void LockRotation(Android.Content.Res.Orientation orientation)
        {
            switch (orientation)
            {
                case Android.Content.Res.Orientation.Portrait:
                    RequestedOrientation = ScreenOrientation.Portrait;
                    break;
                case Android.Content.Res.Orientation.Landscape:
                    RequestedOrientation = ScreenOrientation.Landscape;
                    break;
                case Android.Content.Res.Orientation.Undefined:
                    RequestedOrientation = ScreenOrientation.FullSensor;
                    break;
            }
        }

        void Share(string fileName)

        {

            var intent = new Intent(Intent.ActionSend);

            intent.SetType("video/*");

            var path = "file://" + Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/" + "Foldername" + "/" + fileName;             intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.Parse(path));

            var intentChooser = Intent.CreateChooser(intent, "Share via");

            StartActivityForResult(intentChooser, 1);

        } 
    }
}

