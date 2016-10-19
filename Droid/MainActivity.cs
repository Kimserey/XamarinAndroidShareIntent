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
using System.IO;

[assembly: Dependency(typeof(ShareImplementation))]
namespace ShareIntent.Droid
{
	public class ShareImplementation : IShare
	{
		public void ShareFile()
		{
			// Creates app tmp folder
			var backupDir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, ".baskee_backup");
			Directory.CreateDirectory(backupDir);

			// Creates Share intent
			var intent = new Intent(Intent.ActionSend);
			intent.SetType("*/*");
			intent.AddFlags(ActivityFlags.NewTask);

			// Adds Subject and Text description
			var date = DateTime.UtcNow;
			var backupName = "baskee_" + DateTime.UtcNow.ToString("yyyyMMdd__hhmmss") + ".baskee";
			intent.PutExtra(Intent.ExtraSubject, "Baskee backup - " + date.ToLongDateString());
			intent.PutExtra(Intent.ExtraText, String.Format("Baskee backup file from {0}.", date.ToLongDateString()));

			// Attaches backup file
			var source = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "test.txt");
			var destination = Path.Combine(backupDir, backupName);
			File.Copy(source, destination, true);
			var file = new Java.IO.File(destination);
			intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(file));

			Android.App.Application.Context.StartActivity(intent);
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
