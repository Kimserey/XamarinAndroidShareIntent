using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;

namespace ShareIntent
{
	public interface IShare
	{
		void ShareFile();
		Task<string> LoadFile();
	}

	public interface INotification
	{
		void Notify(
			string message,
			int duration = 3000,
			string actionText = null,
			Action<object> action = null);
	}

	public class App : Application
	{
		public App()
		{
			var label = new Label { HorizontalOptions = LayoutOptions.CenterAndExpand };

			var saveBackup = new Button { Text = "Backup your data" };
			saveBackup.Clicked += (sender, e) => {
				DependencyService.Get<IShare>().ShareFile();
				DependencyService
					.Get<INotification>()
					.Notify("Your data were backed up successfuly! :)");
			};

			var loadBackup = new Button { Text = "Load backup file" };
			loadBackup.Clicked += async (sender, e) =>
			{
				var text = await DependencyService.Get<IShare>().LoadFile();
				label.Text = text;
				DependencyService
					.Get<INotification>()
					.Notify(
						"Your file was successfuly loaded! :)",
						actionText: "Undo",
						action: (obj) => { label.Text = ""; });
			};

			var snack = new Button { Text = "Snack" };
			snack.Clicked += (sender, e) => { 
				DependencyService.Get<INotification>().Notify("Snack!");
			};

			var content = new ContentPage
			{
				Title = "Backup db",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						label,
						snack,
						saveBackup,
						loadBackup
					}
				}
			};

			MainPage = new NavigationPage(content);
		}
	}
}
