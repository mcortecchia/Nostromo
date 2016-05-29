using System;
using ReactiveUI;
using Nostromo.Interfaces;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Nostromo.XForms.Test
{

	public class PasswordDisplayViewModel: SimplePageViewModel {
		string _password;
		public string Password {
			get { return _password; }
			set { this.RaiseAndSetIfChanged (ref _password, value); }
		}

		public override void Load (){ 
			Title = "Password";
		}
	}
	
}
