using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Nostromo.Interfaces;
using System.Threading.Tasks;

namespace Nostromo
{
	public class NavigationViewModel: PageContainerViewModel
	{
		IPageViewModel _modalPresentedViewModel;
		public IPageViewModel ModalPresentedPage {
			get {
				return _modalPresentedViewModel;
			}
			set {
				this.UpdateAndRaiseIfChanged (ref _modalPresentedViewModel, value);
			}
		}


		public NavigationViewModel ()
		{
			Pages.CollectionChanged += Pages_CollectionChanged;
		}

		public NavigationViewModel (IPageViewModel root):this()
		{
			selfHandlingAppear = true;
			setNewRoot (root, false);
			lastAppearing ();
			selfHandlingAppear = false;
		}

		public override void Load() {
			
		}

		HashSet<int> appearedViews = new HashSet<int>();
		public async void EnforcePageAppearsNotification(IPageViewModel page) {
			if (!appearedViews.Contains (page.GetHashCode ())) {
				appearedViews.Add (page.GetHashCode ());
				await page.Appearing ();
			}
		}

		public async void EnforcePageDisappearsNotification(IPageViewModel page, bool removingFromStack) {

			if (removingFromStack) {
				if (Pages.Contains (page)) {
					Pages.Remove (page);
				}
			}

			if (appearedViews.Contains (page.GetHashCode ())) {
				appearedViews.Remove (page.GetHashCode ());
				await page.Disappearing ();
			}

			if (ModalPresentedPage == page) {
				ModalPresentedPage = null;
			}
		}

		bool selfHandlingAppear { get; set; }


		void updateBackButtonCanExecute ()
		{
			SetBackEnabled (((Pages?.Count??0) > 0));
		}

	 	void Pages_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			updateBackButtonCanExecute ();
		}

		async Task lastAppearing() {
			var lp = Pages.LastOrDefault ();
			if (lp != null) {
				if (!appearedViews.Contains (lp.GetHashCode ())) {
					appearedViews.Add (lp.GetHashCode ());
					await lp.Appearing ();
				}
			}
		}

		async Task lastDisappearing() {
			var lp = Pages.LastOrDefault ();
			if (lp != null) {
				if (appearedViews.Contains(lp.GetHashCode())) {
					appearedViews.Remove (lp.GetHashCode ());
					await lp.Disappearing ();
				}
			}
		}


		public async Task PopToRootAsync(bool animated) {
			if (Pages.LastOrDefault () != Pages.FirstOrDefault ()) {
				selfHandlingAppear = true;
				await lastDisappearing ();
				setNewRoot (Pages.FirstOrDefault (), animated);
				await lastAppearing();
				selfHandlingAppear = false;
			}
		}

		public async Task SetNewRootAsync(IPageViewModel newRoot, bool animated) {
			selfHandlingAppear = true;
			bool wasVisible = newRoot == Pages.LastOrDefault ();
			if (!wasVisible) {
				await (lastDisappearing ());
			} 
			setNewRoot (newRoot, animated);

			if (!wasVisible) {
				await (lastAppearing ());
			}
			selfHandlingAppear = false;
		}

		protected async virtual void setNewRoot(IPageViewModel newRoot, bool animated) {
			TransitionAnimationEnabled = animated;
			newRoot.Navigator = this;
			if (Pages.Count > 1 || Pages.FirstOrDefault () != newRoot) {
				Pages.Clear ();
				Pages.Add (newRoot);
			}
		}

		public async Task PopAsync(bool animated) {
			if (CanNavigateBack ()) {
				selfHandlingAppear = true;
				TransitionAnimationEnabled = animated;
				await lastDisappearing ();
				Pages.RemoveAt (Pages.Count - 1);
				await lastAppearing ();
				selfHandlingAppear = false;
			}
		}

		public async Task PushAsync(IPageViewModel model, bool animated) {
			TransitionAnimationEnabled = animated;
			model.Navigator = this;
			selfHandlingAppear = true;
			await lastDisappearing ();
			Pages.Add (model);
			await lastAppearing ();
			selfHandlingAppear = false;
		}

		public async Task PushModalAsync(IPageViewModel model, bool animated) {
			TransitionAnimationEnabled = animated;
			model.Navigator = this;

			if (ModalPresentedPage != null) {
				throw new Exception ("Only one modal view at once");
			}
				
			selfHandlingAppear = true;
			await lastDisappearing();
			ModalPresentedPage = model;
			if (model != null) {
				await model.Appearing ();
			}
			selfHandlingAppear = false;
		}

		public async Task PopModalAsync (bool animated)
		{
			TransitionAnimationEnabled = animated;
			if (ModalPresentedPage != null) {
				selfHandlingAppear = true;
				await ModalPresentedPage.Disappearing ();
				ModalPresentedPage = null;
				await lastAppearing ();
				selfHandlingAppear = false;
			} else {
				if (Navigator != null) {
					await Navigator.PopModalAsync (animated);
				}
			}
		}

		#region implemented abstract members of NavigatorViewModel

		#region implemented abstract members of PageContainerViewModel

		protected override bool CanNavigateBack ()
		{
			return Pages.Count > 0;
		}

		#endregion
		public async override Task NavigateToAsync (IPageViewModel viewmodel, Nostromo.Interfaces.NavigatorSettings settings)
		{
			if (settings.IsModal) {
				await PushModalAsync (viewmodel, settings.IsModal);
			} else {
				await PushAsync (viewmodel, settings.UsePlatformAnimation);
			}
		}

		public override async Task BackAsync ( Nostromo.Interfaces.NavigatorSettings settings)
		{	
			if (settings.IsModal) {
				await PopModalAsync (settings.UsePlatformAnimation);
			}
			else {
				var navigationSettings = settings as NavigationSettings;

				if (navigationSettings != null && navigationSettings.PopToRoot) {
					if (navigationSettings.NewRoot != null) {
						await SetNewRootAsync (navigationSettings.NewRoot, settings.UsePlatformAnimation);
					} else {
						await PopToRootAsync (settings.UsePlatformAnimation);
					}
				} else { 
					await PopAsync (settings.UsePlatformAnimation);
				}
			}
		}
		#endregion
	}
}

