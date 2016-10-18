using System;
using Xamarin.Forms;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Threading;
using ShareIntent.Droid;

[assembly: Dependency(typeof(ShareImplementation))]
namespace ShareIntent.Droid
{
	public class ShareImplementation : IShare
	{
		public void ShareFile(string path)
		{
			var sendIntent = new Intent(Intent.ActionSend);
			sendIntent.PutExtra(Intent.ExtraText, path);
			sendIntent.SetType("text/plain");
			sendIntent.AddFlags(ActivityFlags.NewTask);
			Android.App.Application.Context.StartActivity(sendIntent);
		}
	}

	[Activity(Label = "ShareIntent.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());
		}
	}
}
