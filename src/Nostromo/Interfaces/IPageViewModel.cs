using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Nostromo.Interfaces
{
	public interface IPageViewModel : INotifyPropertyChanged
	{
		void Load();
		Task Appearing();
		Task Disappearing();
		string Title { get; set; }
		IPageNavigator Navigator { get; set; }
	}
}
