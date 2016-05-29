using System;
using Nostromo.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Nostromo
{
	public abstract class PageContainerViewModel : PageViewModel, IPageNavigatorViewModel
	{
		public System.Windows.Input.ICommand BackCommand {
			get;
			private set;
		}

		protected void SetBackEnabled(bool enabled) {
			(BackCommand as PageCommand).Enabled = enabled;
		}

		#region IPageNavigatorViewModel implementation
		public abstract Task NavigateToAsync (IPageViewModel viewmodel, NavigatorSettings settings);
		public abstract Task BackAsync(NavigatorSettings settings);
		#endregion


		ObservableCollection<IPageViewModel> _pages;
		public virtual ObservableCollection<IPageViewModel> Pages {
			get { return _pages; }
			private set { UpdateAndRaiseIfChanged(ref _pages, value); }
		}

		bool _transitionAnimationEnabled = false;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Nostromo.NavigatorViewModel"/> transition animation enabled.
		/// </summary>
		/// <value><c>true</c> if transition animation enabled; otherwise, <c>false</c>.</value>
		public bool TransitionAnimationEnabled {
			get { return _transitionAnimationEnabled; }
			set { this.UpdateAndRaiseIfChanged (ref _transitionAnimationEnabled, value); }
		}
			
		protected abstract bool CanNavigateBack ();

		protected PageContainerViewModel ()
		{
			BackCommand = PageCommand.Create (this,
				p => BackAsync (new NavigatorSettings { UsePlatformAnimation = true }),
				p => p.CanNavigateBack ()
			);

			Pages = new ObservableCollection<IPageViewModel> ();
		}
	}
}

