using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShareIntent
{
	public interface IShare
	{
		void ShareFile(string path);
	}

	public class App : Application
	{
		public App()
		{
			var button = new Button { Text = "Share" };
			button.Clicked += (sender, e) => {
				DependencyService.Get<IShare>().ShareFile("somefile.txt");
			};

			var content = new ContentPage
			{
				Title = "Share intent test",
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
