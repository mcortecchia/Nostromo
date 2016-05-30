using System;
using System.Threading.Tasks;
using Nostromo.Interfaces;

namespace Nostromo.Navigation
{

	public class NavigationSettings : NavigatorSettings
	{
		public bool PopToRoot { get; set; }
		public IPageViewModel NewRoot { get; set; }
	}

	public static class PageNavigationExtensions
	{
		public static async Task PushAsync (this IPageNavigator navigator, IPageViewModel vm, bool animated)
		{
			await navigator.NavigateToAsync (vm, new NavigationSettings {
				UsePlatformAnimation = animated
			});
		}

		public static async Task PopAsync (this IPageNavigator navigator, bool animated)
		{
			await navigator.BackAsync (new NavigationSettings {
				UsePlatformAnimation = animated
			});
		}

		public static async Task PopToRootAsync (this IPageNavigator navigator, bool animated)
		{
			await navigator.BackAsync (new NavigationSettings {
				UsePlatformAnimation = animated,
				PopToRoot = true,
			});
		}

		public static async Task SetNewRootAsync (this IPageNavigator navigator, IPageViewModel newRoot, bool animated)
		{
			await navigator.BackAsync (new NavigationSettings {
				NewRoot = newRoot,
				UsePlatformAnimation = animated,
				PopToRoot = true,
			});
		}
	}
}

