using System;
using Nostromo;
using Nostromo.Interfaces;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Nostromo
{
	public abstract class PageViewModel : ViewModel, IPageViewModel
	{
		string _title;
		public string Title {
			get { return _title; }
			set { this.UpdateAndRaiseIfChanged (ref _title, value); }
		}

		#region IPageViewModel implementation
		public IPageNavigator Navigator {
			get ;
			set ;
		}
		#endregion

		public abstract void Load ();

		public virtual async Task Appearing (){ }
		public virtual async Task Disappearing (){}
	}
}

