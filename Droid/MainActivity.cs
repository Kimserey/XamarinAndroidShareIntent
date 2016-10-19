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
	public class FilePicked : EventArgs
	{
		public string Path { get; set; }
		public bool IsCancelled { get; set; }
		public Exception Exception { get; set; }
	}

	public class ShareImplementation : IShare
	{
		public ShareImplementation()
		{

		}

		public void ShareFile()
		{
			// Creates app backup folder
			var backupDir = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Baskee", "Backups");
			Directory.CreateDirectory(backupDir);

			// Creates Share intent
			var intent = new Intent(Intent.ActionSend);
			intent.SetType("*/*");
			intent.AddFlags(ActivityFlags.NewTask);

			// Adds Subject and Text description
			var date = DateTime.UtcNow;
			var backupName = DateTime.UtcNow.ToString("yyyyMMdd_hhmmss") + ".baskee";
			intent.PutExtra(Intent.ExtraSubject, "Baskee backup - " + date.ToLongDateString());
			intent.PutExtra(Intent.ExtraText, String.Format("Baskee backup file from {0}.", date.ToLongDateString()));

			// Attaches backup file
			var source = Path.Combine(Android.App.Application.Context.FilesDir.AbsolutePath, "test.txt");
			var destination = Path.Combine(backupDir, backupName);
			File.Copy(source, destination, true);
			var file = new Java.IO.File(destination);
			intent.PutExtra(Intent.ExtraStream, Android.Net.Uri.FromFile(file));

			Android.App.Application.Context.StartActivity(intent);
		}

		TaskCompletionSource<string> completionSource;
		int id;

		// Creates a unique Id which will be used
		// to identify the picking intent.
		// We must ensure that there will be only one pick intent 
		// at a time otherwise it will yield very strange behaviours.
		int NextId()
		{
			int i = id;
			if (id == Int32.MaxValue) id = 0;
			else id++;
			return i;
		}

		public Task<string> LoadFile()
		{ 
			var next = new TaskCompletionSource<string>(NextId());

			EventHandler<FilePicked> handler = null;

			handler = (sender, e) =>
			{
				// Threadsafe way to replace "completionSource" by null
				var tcs = Interlocked.Exchange(ref completionSource, null);

				PickFileActivity.Picked -= handler;

				if (String.IsNullOrWhiteSpace(e.Path))
					tcs.SetResult(e.Path);
				else if (e.Exception != null)
					tcs.SetException(e.Exception);
			};

			PickFileActivity.Picked += handler;

			var intent = new Intent(Android.App.Application.Context, typeof(PickFileActivity));
			intent.PutExtra(PickFileActivity.ExtraId, id);
			intent.SetFlags(ActivityFlags.NewTask);
			Android.App.Application.Context.StartActivity(intent);

			// Threadsafe way to compare "completionSource" to "null" and replace "completionSource" with "next" if equal
			if (Interlocked.CompareExchange(ref completionSource, next, null) != null)
				throw new InvalidOperationException("Another task is already started.");

			return completionSource.Task;
		}
	}

	[Activity]
	public class PickFileActivity : Activity
	{
		internal const string ExtraId = "id";
		internal static event EventHandler<FilePicked> Picked;
		int id;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var bundle = savedInstanceState ?? this.Intent.Extras;
			id = bundle.GetInt(ExtraId);

			var intent = new Intent(Intent.ActionGetContent);
			intent.SetType("file/*");
			StartActivityForResult(intent, id);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (Picked != null)
				Picked(this, new FilePicked { Exception = new FileNotFoundException() });

			Finish();
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
