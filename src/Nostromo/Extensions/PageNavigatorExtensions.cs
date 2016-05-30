using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Nostromo.Interfaces
{
		
	public static class PageNavigatorExtensions {
		
		public static async Task PushModalAsync (this IPageNavigator navigator, IPageViewModel vm, bool animated)
		{
			await navigator.NavigateToAsync (vm, new NavigatorSettings {
				UsePlatformAnimation = animated,
				IsModal = true
			});
		}

		public static async Task PopModalAsync (this IPageNavigator navigator, bool animated)
		{
			await navigator.BackAsync (new NavigatorSettings {
				UsePlatformAnimation = animated,
				IsModal = true
			});
		}

	}
}
