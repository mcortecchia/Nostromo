using System;
using ReactiveUI;
using Nostromo.Interfaces;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Nostromo.XForms.Test
{
	public abstract class SimplePageViewModel: ReactiveObject, IPageViewModel {
		#region INotifyPropertyChanged implementation
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		#endregion
		#region IPageViewModel implementation
		public abstract void Load ();
		public virtual async Task Appearing () {}
		public virtual async Task Disappearing () {}
		string _title;
		public string Title {
			get {
				return _title;
			}
			set {
				this.RaiseAndSetIfChanged (ref _title, value);
			}
		}

		public IPageNavigator Navigator {
			get;
			set;
		}
		#endregion
		
	}
	
}
