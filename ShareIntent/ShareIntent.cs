using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShareIntent
{
	public interface IShare
	{
		void ShareFile();
	}

	public class App : Application
	{
		public App()
		{
			var button = new Button { Text = "Backup your data" };
			button.Clicked += (sender, e) => {
				DependencyService.Get<IShare>().ShareFile();
			};

			var content = new ContentPage
			{
				Title = "Backup db",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						button
					}
				}
			};

			MainPage = new NavigationPage(content);
		}
	}
}
