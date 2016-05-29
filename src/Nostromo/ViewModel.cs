using System;
using Nostromo;
using Nostromo.Interfaces;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Nostromo
{
	public abstract class ViewModel: INotifyPropertyChanged {
		#region INotifyPropertyChanged implementation
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged = delegate {};
		#endregion

		protected void RaisePropertyChanged( [CallerMemberName]string propertyName ="" ) {
			PropertyChanged (this, new PropertyChangedEventArgs(propertyName));
		}

		protected void UpdateAndRaiseIfChanged<T>(ref T property, T value, [CallerMemberName]string propertyName ="" ) {
			if (!object.Equals(property, value)) {
				property = value;
				RaisePropertyChanged (propertyName);
			}
		}

	}
	
}
