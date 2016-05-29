using System;
using ReactiveUI;

using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Nostromo.XForms.Test.Views
{
	[ViewFor (typeof(PasswordGeneratorViewModel))]
	public class PasswordGeneratorView: ReactiveUI.XamForms.ReactiveContentPage<PasswordGeneratorViewModel>
	{
		public Button generateButton { get; set; }
		public Button settingsButton { get; set; }


		public PasswordGeneratorView () : base ()
		{
			this.Content =
				new StackLayout {
				Children = {
					(generateButton = new Button {
						Text = "Genera PWD",
					}),
					(settingsButton = new Button {
						Text = "Settings"
					})
				}
			};
		

			this.OneWayBind (ViewModel, x => x.Title, x => x.Title);
			this.Bind (ViewModel, x => x.GeneratePassword, x => x.generateButton.Command);
			this.Bind (ViewModel, x => x.ShowSettings, x => x.settingsButton.Command);
		}
	}
}

