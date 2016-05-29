using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Nostromo.Interfaces
{
	public class NavigatorSettings {
		public bool IsModal { get; set; }
		public bool UsePlatformAnimation { get; set; }
	}

	public class NavigationSettings: NavigatorSettings {
		public bool PopToRoot { get; set; }
		public IPageViewModel NewRoot { get; set; }
	}

	public interface IPageNavigator
	{
		ICommand BackCommand { get; }
		Task NavigateToAsync (IPageViewModel viewmodel, NavigatorSettings settings);
		Task BackAsync (NavigatorSettings settings);
	}

	public interface IPageNavigatorViewModel:IPageNavigator, IPageViewModel
	{
	}



		
}
