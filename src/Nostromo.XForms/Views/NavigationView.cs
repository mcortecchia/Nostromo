using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Nostromo.Interfaces;
using System.Linq;
using System.Threading.Tasks;


using static System.Diagnostics.Debug;

namespace Nostromo.Views
{
	[ViewFor(typeof(NavigationViewModel))]
	public class NavigationView : Xamarin.Forms.NavigationPage
	{
		public static BindableProperty PagesProperty = 
			XFProperty.CreateOneWay<NavigationView, ObservableCollection<IPageViewModel>> (p => p.Pages);
		public static BindableProperty TransitionAnimationEnabledProperty = 
			XFProperty.CreateOneWay<NavigationView, bool> (p => p.TransitionAnimationEnabled);
		public static BindableProperty BindingTitleProperty = 
			XFProperty.CreateOneWay<NavigationView, string> (p => p.Title);
		
		protected class ViewLinkHelper {
			public IPageViewModel Model { get; private set; }
			public Page View { get; private set; }
			public WeakReference NavigationView { get; private set; }

			public ViewLinkHelper(Page view, IPageViewModel model, NavigationView nvm) {
				View = view;
				Model = model;
				NavigationView = new WeakReference(nvm);

				view.Appearing+= View_Appearing;
			}
			
			void View_Appearing (object sender, EventArgs e)
			{
				View.Appearing-= View_Appearing;
				View.Disappearing+= View_Disappearing;
				(NavigationView.Target as NavigationView)?.PageAppears(this);
			}

			void View_Disappearing (object sender, EventArgs e)
			{
				View.Appearing+= View_Appearing;
				View.Disappearing -= View_Disappearing;
				var nvm = (NavigationView.Target as NavigationView);
				nvm?.PageDisappears (this);	
			}

		}

		protected List<ViewLinkHelper> PageReferences {
			get; 
			private set;
		}


		NavigationViewModel _viewModel;
		public NavigationViewModel ViewModel {
			get { return _viewModel; }
			set {
				if (_viewModel!=null)
					_viewModel.PropertyChanged -= _viewModel_PropertyChanged;
				_viewModel = value;
				BindingContext = value;
				if (_viewModel!=null)
					_viewModel.PropertyChanged += _viewModel_PropertyChanged;
			}
		}

		void _viewModel_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof (_viewModel.ModalPresentedPage)) {
				if (_viewModel.ModalPresentedPage != null) {
					var pref = createPageFor (_viewModel.ModalPresentedPage);
					Navigation.PushModalAsync (pref.View, _viewModel.TransitionAnimationEnabled);
				} else if (_viewModel.ModalPresentedPage == null) {
					Navigation.PopModalAsync (_viewModel.TransitionAnimationEnabled);
				}
			}
		}

		ObservableCollection<IPageViewModel> _pages;
		public ObservableCollection<IPageViewModel> Pages {
			get {
				return _pages;
			}
			set {
				if (_pages != value) {
					if (_pages != null) { _pages.CollectionChanged -= _pages_CollectionChanged; }
					_pages = value;
					updatePages ();
					if (_pages != null) { _pages.CollectionChanged += _pages_CollectionChanged; }
				}
			}
		}

		protected void PageAppears (ViewLinkHelper viewLinkHelper)
		{
			ViewModel.EnforcePageAppearsNotification (viewLinkHelper.Model);
		}

		protected void PageDisappears(ViewLinkHelper viewLinkHelper)
		{
			var removingFromStack = ViewModel.Pages.LastOrDefault () == viewLinkHelper.Model;
			if (removingFromStack) {
				PageReferences.Remove (viewLinkHelper);
			} 
			
			ViewModel.EnforcePageDisappearsNotification (viewLinkHelper.Model, removingFromStack);
		}

		public bool TransitionAnimationEnabled { get; set; }

		async Task popToNewRootAsync(Page newRoot, bool animated) {
			Navigation.InsertPageBefore(newRoot, Navigation.NavigationStack[0]);
			await this.PopToRootAsync (TransitionAnimationEnabled);
		}

		async void updatePages ()
		{
			if (_pages != null && _pages.Count > 0) {
				foreach (var pref in PageReferences.Where (p => !_pages.Contains (p.Model)).ToList()) {
					PageReferences.Remove (pref);
				}

				if (new[] {this.Navigation.NavigationStack.FirstOrDefault(), this.Navigation.NavigationStack.LastOrDefault(), PageReferences.LastOrDefault ()?.View }
					.All( v => v == PageReferences.FirstOrDefault ()?.View)) {
					OnPopToRoot (Pages.LastOrDefault (), TransitionAnimationEnabled);
				} else {
					if (Navigation.NavigationStack.Count > 0) {
						var pref = createPageFor (Pages.LastOrDefault ());

						PageReferences.Clear ();
						PageReferences.Add(pref);
						await popToNewRootAsync (pref.View, TransitionAnimationEnabled);
					}
					else {
						OnPushViewModel (Pages.LastOrDefault (), TransitionAnimationEnabled);
					}
				}
			}
			else {
				//At least on page should be displayed
				PageReferences.Clear ();
				var blankPage = new ContentPage {
					BackgroundColor = Color.Transparent
				};

				if (Navigation.NavigationStack.Count > 0) {
					await popToNewRootAsync (blankPage, TransitionAnimationEnabled);
				}
				else {
					await PushAsync (blankPage, TransitionAnimationEnabled);
				}
			}
		}


		public NavigationView ()
		{
			PageReferences = new List<ViewLinkHelper> ();
			PushAsync (new ContentPage {
				BackgroundColor = Color.Transparent
			});

			this.SetBinding (PagesProperty, "Pages");
			this.SetBinding (TransitionAnimationEnabledProperty, "TransitionAnimationEnabled");
			this.SetBinding (BindingTitleProperty, "Title");
		}


	
		protected void RemoveReferenceFromStack(ViewLinkHelper reference) {
			if (reference!=null) {
				PageReferences.Remove(reference);
				ViewModel.EnforcePageDisappearsNotification(reference.Model, true);
			}
		}

		bool resetting = false;
		void _pages_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			bool wasResetting = resetting;
			resetting = false;

			if (e.Action == NotifyCollectionChangedAction.Add) {
				if (wasResetting && e.NewItems.OfType<IPageViewModel> ().FirstOrDefault () == PageReferences.FirstOrDefault ()?.Model) {
					OnPopToRoot (Pages.LastOrDefault (), TransitionAnimationEnabled);
				} else if (wasResetting) {
					OnPopToNewRoot(e.NewItems.OfType<IPageViewModel> ().LastOrDefault (), TransitionAnimationEnabled);
				} else {
					OnPushViewModel (e.NewItems.OfType<IPageViewModel> ().LastOrDefault (), TransitionAnimationEnabled);
				}
			} else if (e.Action == NotifyCollectionChangedAction.Remove) {
				if (PageReferences.LastOrDefault ()?.Model == e.OldItems.OfType<IPageViewModel> ().LastOrDefault () && e.OldItems.Count == 1) {
					OnPopViewModel (e.OldItems.OfType<IPageViewModel> ().LastOrDefault (), TransitionAnimationEnabled);
				} else {
					PageReferences.RemoveAll (r => e.OldItems.Contains (r.Model));
				}
			} else if (e.Action == NotifyCollectionChangedAction.Reset) {
				resetting = true;
			}
		}


		protected virtual ViewLinkHelper createPageFor(IPageViewModel viewModel) {
			var view = ViewBuilder.Build<Page> (viewModel);
			return new ViewLinkHelper(view, viewModel, this);
		}

		protected void OnPopToNewRoot(IPageViewModel viewModel, bool animated) {
			var pref = createPageFor (viewModel);
			PageReferences.Clear ();
			PageReferences.Add (pref);
			popToNewRootAsync (pref.View, animated);
		}

		protected virtual void OnPushViewModel(IPageViewModel viewModel, bool animated) {
			if (PageReferences.Any ( p => p.Model == viewModel))
				throw new Exception(String.Format("The navigation stack already contains the viewModel: {0}", viewModel));

			var pref = createPageFor (viewModel);
			PageReferences.Add (pref);
			PushAsync (pref.View, animated);
		}
	
		protected virtual void OnPopViewModel(IPageViewModel viewModel, bool animated) {
			if (PageReferences.LastOrDefault ()?.Model != viewModel) {
				throw new Exception(String.Format("The viewModel is not the last in the stack: {0}", viewModel));
			}

			var pageReference = PageReferences.LastOrDefault ();
			PageReferences.RemoveAt (PageReferences.Count - 1);
			if (this.Navigation.NavigationStack.LastOrDefault () == pageReference.View) {
				PopAsync ();
			}
		}

		protected virtual void OnPopToRoot(IPageViewModel viewModel, bool animated) {
			var except = PageReferences.FirstOrDefault ();
			PageReferences.RemoveAll (p => p != except);
			PopToRootAsync (animated);
		}

	}
}

