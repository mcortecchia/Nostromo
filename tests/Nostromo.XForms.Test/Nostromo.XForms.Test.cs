using System;
using Nostromo;

using Xamarin.Forms;
using System.Reflection;
using System.Linq;

namespace Nostromo.XForms.Test
{

	public class App : Application
	{
		public App ()
		{
			NostromoApp.Setup ()
				.UseAutofacContainer ()
				.UseXamarinFormsViewActivator ()
				.RegisterViewsInAssemblyOf<App>()
				.RegisterDependenciesInAssemblyOf<App> ()
				.Init ();
			
			
			Startup ();
		}

		async void Startup() {
			var vm = Build.ViewModel<NavigationViewModel> ();
			vm.Title = "VM";
			vm.PushAsync (Build.ViewModel(() => new PasswordGeneratorViewModel()), true);

			// The root page of your application
			MainPage = Build.View<Page> (vm);
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

