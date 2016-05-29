using System;
using ReactiveUI;

using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace Nostromo.XForms.Test.Views
{

	[ViewFor (typeof(PasswordSettingsViewModel))]
	public class PasswordSettingsView: ReactiveUI.XamForms.ReactiveContentPage<PasswordSettingsViewModel>
	{
		public Entry countEntry { get; set; }
		public Editor charactersEdit { get; set; }
		public ToolbarItem tbItem { get; set; }

		public PasswordSettingsView () : base ()
		{
			this.Content =
				new StackLayout {
				Children = {
					new Label {
						Text = "Password Length:"
					},
					(countEntry = new Entry {
						Text = "",
					}),
					new Label {
						Text = "Password Characters:"
					},
					(charactersEdit = new Editor {
						VerticalOptions = LayoutOptions.FillAndExpand
					})
				}
			};

			tbItem = new ToolbarItem {
				Text = "Close"
			};

			this.OneWayBind (ViewModel, x => x.Title, x => x.Title);
			this.Bind (ViewModel, x => x.CharactersCount, x => x.countEntry.Text);
			this.Bind (ViewModel, x => x.PasswordCharacters, x => x.charactersEdit.Text);
			this.Bind (ViewModel, x => x.Pop, x => x.tbItem.Command);
			this.ToolbarItems.Add (tbItem);
		}
	}
	
}
