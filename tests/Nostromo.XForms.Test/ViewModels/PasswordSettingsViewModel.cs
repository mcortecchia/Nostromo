using System;
using ReactiveUI;
using Nostromo.Interfaces;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Nostromo.XForms.Test
{

	public class PasswordSettingsViewModel: SimplePageViewModel {

		public ICommand Pop { get; set; }

		string _passwordCharacters;
		public string PasswordCharacters { 
			get { return _passwordCharacters; }
			set { this.RaiseAndSetIfChanged (ref _passwordCharacters, value); }
		}

		int _charactersCount;
		public int CharactersCount { 
			get { return _charactersCount; }
			set { this.RaiseAndSetIfChanged (ref _charactersCount, value); }
		}

		public override void Load (){ 
			Title = "Password Settings";
			Pop = ReactiveCommand.CreateAsyncTask ((o) => Navigator.PopModalAsync (true));
		}
	}
	
}
