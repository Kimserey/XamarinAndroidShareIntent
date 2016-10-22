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

	public class App : Application
	{
		public App()
		{
			var label = new Label { HorizontalOptions = LayoutOptions.CenterAndExpand };

			var saveBackup = new Button { Text = "Backup your data" };
			saveBackup.Clicked += (sender, e) => {
				DependencyService.Get<IShare>().ShareFile();
			};

			var loadBackup = new Button { Text = "Load backup file" };
			loadBackup.Clicked += async (sender, e) =>
			{
				var text = await DependencyService.Get<IShare>().LoadFile();
				label.Text = text;
			};

			var content = new ContentPage
			{
				Title = "Backup db",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						label,
						saveBackup,
						loadBackup
					}
				}
			};

			MainPage = new NavigationPage(content);
		}
	}
}
