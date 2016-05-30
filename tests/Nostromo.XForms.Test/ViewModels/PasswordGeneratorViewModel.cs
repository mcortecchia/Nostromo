using System;
using ReactiveUI;
using Nostromo.Interfaces;
using System.Windows.Input;
using System.Threading.Tasks;
using Nostromo.Navigation;

namespace Nostromo.XForms.Test
{
	public class PasswordGeneratorViewModel: SimplePageViewModel
	{
		public RandomPasswordGenerator PWDGen { get; set; }
		public PasswordSettingsViewModel Settings { get; private set; }
		public PasswordDisplayViewModel Display { get; private set; }


		int id;
		public PasswordGeneratorViewModel (int id = 1)
		{
			this.id = id;
		}

		public override void Load ()
		{
			Display = Build.ViewModel (() => new PasswordDisplayViewModel ());
			Settings = Build.ViewModel (() => new PasswordSettingsViewModel ());

			Title = "Password Generator";
			Settings.CharactersCount = 6;
			Settings.PasswordCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";


			GeneratePassword = ReactiveCommand.CreateAsyncTask (async (p) => {
				Display.Password = PWDGen.Generate (Settings.CharactersCount, Settings.PasswordCharacters);
				await Navigator.PushAsync(Display, true);
			});

			ShowSettings = ReactiveCommand.CreateAsyncTask (p => Navigator.PushModalAsync(new NavigationViewModel(Settings), true));
		}

		public ICommand GeneratePassword { get; private set; }
		public ICommand ShowSettings { get; private set; }
	}
}

