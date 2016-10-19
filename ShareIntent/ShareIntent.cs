using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShareIntent
{
	public interface IShare
	{
		void ShareFile();
		void LoadFile();
	}

	public class App : Application
	{
		public App()
		{
			var saveBackup = new Button { Text = "Backup your data" };
			saveBackup.Clicked += (sender, e) => {
				DependencyService.Get<IShare>().ShareFile();
			};

			var loadBackup = new Button { Text = "Load backup file" };
			loadBackup.Clicked += (sender, e) =>
			{
				DependencyService.Get<IShare>().LoadFile();
			};

			var content = new ContentPage
			{
				Title = "Backup db",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						saveBackup,
						loadBackup
					}
				}
			};

			MainPage = new NavigationPage(content);
		}
	}
}
