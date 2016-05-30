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

	public interface IPageNavigator
	{
		Task NavigateToAsync (IPageViewModel viewmodel, NavigatorSettings settings);
		Task BackAsync (NavigatorSettings settings);
	}

	public interface IPageNavigatorViewModel:IPageNavigator, IPageViewModel
	{
		ICommand BackCommand { get; }
	}	
}
