using System;
using ReactiveUI;

using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Nostromo.XForms.Test.Views
{

	[ViewFor (typeof(PasswordDisplayViewModel))]
	public class PasswordDisplayView: ReactiveUI.XamForms.ReactiveContentPage<PasswordDisplayViewModel>
	{
		public Label passwordLabel { get; set; }

		public PasswordDisplayView () : base ()
		{
			this.Content =
				new Frame {
				Content =
				(passwordLabel = new Label {
					Text = "",
					HorizontalTextAlignment = TextAlignment.Center,
					FontSize = 30,
					VerticalTextAlignment= TextAlignment.Center,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
				})
			};

			this.OneWayBind (ViewModel, x => x.Title, x => x.Title);
			this.Bind (ViewModel, x => x.Password, x => x.passwordLabel.Text);
		}
	}
}
