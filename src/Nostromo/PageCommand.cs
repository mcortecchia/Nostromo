using System;
using System.Windows.Input;
using Nostromo.Interfaces;

namespace Nostromo
{

	public abstract class PageCommand: ICommand {
		public abstract bool CanExecute (object parameter);
		public abstract void Execute (object parameter);

		bool _enabled;
		public bool Enabled {
			get {
				return _enabled;
			}
			set {
				if (_enabled != value) {
					_enabled = value;
					RaiseCanExecuteChanged ();
				}
			}
		}

		public void RaiseCanExecuteChanged() {
			if (CanExecuteChanged != null) {
				CanExecuteChanged (this, new EventArgs ());
			}
		}
		public event EventHandler CanExecuteChanged;


		public static PageCommand<T> Create<T>( T page, Action<T> command, Func<T, bool> canExecute = null) 
			where T: class, IPageViewModel {
			return PageCommand<T>.Create (page, command, canExecute);
		}

		public static PageCommand<T> Create<T,TParam>( T page, Action<T, TParam> command, Func<T, TParam, bool> canExecute = null)
			where T: class, IPageViewModel  {
			return PageCommand.Create (page, command, canExecute);
		}
	}

	public class PageCommand<T> : PageCommand
		where T: class, IPageViewModel 
	{
		public static PageCommand<T> Create( T page, Action<T> command, Func<T, bool> canExecute = null) {
			return new PageCommand<T> (page, (p, param) => command (p), (p, param) => canExecute?.Invoke (p) ?? true);
		}

		public static PageCommand<T> Create<TParam>( T page, Action<T, TParam> command, Func<T, TParam, bool> canExecute = null) {
			return new PageCommand<T> (page, 
				(p, param) => command (p, (TParam)param),
				(p, param) => canExecute?.Invoke (p, (TParam)param) ?? true);
		}



		readonly Action<T, object> _command;
		readonly Func<T, object, bool> _canExecute = null;
		readonly WeakReference _page;
		private PageCommand (T page, Action<T, object> command, Func<T, object, bool> canExecute = null)
		{
			if (command == null)
				throw new NullReferenceException ("command cannot be null");
			_page = new WeakReference (page);
			_command = command;
			_canExecute = canExecute;
		}

		#region ICommand implementation



		public override bool CanExecute (object parameter)
		{
			if (!Enabled)
				return false;
			
			var page = _page.Target as T;
			if (page == null)
				return false;
			
			return _canExecute?.Invoke (page, parameter) ?? false;
		}

		public override void Execute (object parameter)
		{
			var page = _page.Target as T;
			if (page != null)
				_command (page, parameter);
		}

		#endregion
	}
}

