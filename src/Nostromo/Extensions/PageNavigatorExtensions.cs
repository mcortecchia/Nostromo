using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Nostromo.Interfaces
{
		
	public static class PageNavigatorExtensions {

		public static async Task PushAsync (this IPageNavigator navigator, IPageViewModel vm, bool animated) {
			await navigator.NavigateToAsync (vm, new NavigatorSettings { 
				UsePlatformAnimation = animated
			});
		}


		public static async Task PushModalAsync (this IPageNavigator navigator, IPageViewModel vm, bool animated) {
			await navigator.NavigateToAsync (vm, new NavigatorSettings { 
				UsePlatformAnimation = animated,
				IsModal = true
			});
		}

		public static async Task PopModalAsync (this IPageNavigator navigator, bool animated) {
			await navigator.BackAsync (new NavigatorSettings { 
				UsePlatformAnimation = animated,
				IsModal = true
			});
		}

		public static async Task PopAsync (this IPageNavigator navigator, bool animated) {
			await navigator.BackAsync (new NavigatorSettings { 
				UsePlatformAnimation = animated
			});
		}

		public static async Task PopToRootAsync (this IPageNavigator navigator, bool animated) {
			await navigator.BackAsync (new NavigationSettings { 
				UsePlatformAnimation = animated,
				PopToRoot = true,
			});
		}

		public static async Task SetNewRootAsync (this IPageNavigator navigator, IPageViewModel newRoot, bool animated) {
			await navigator.BackAsync (new NavigationSettings { 
				NewRoot = newRoot,
				UsePlatformAnimation = animated,
				PopToRoot = true,
			});
		}
	}
}
