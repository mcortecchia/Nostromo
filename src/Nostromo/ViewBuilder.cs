using System;
using System.Collections.Generic;
using Nostromo.Interfaces;

namespace Nostromo
{
	public static class ViewBuilder
	{
		static IPlatformViewActivator _platformActivator;
		static private IPlatformViewActivator viewActivator { get { return _platformActivator ?? (_platformActivator = Container.Default.Resolve<IPlatformViewActivator> ()); } }

		static ViewBuilder() {
		}

		public static IViewBuilder GetViewBuilder(object viewModel, Type screenType = null) {
			return viewActivator.CreateViewBuilder ()
				.WithViewModel (viewModel)
				.OfType (screenType);
		}

		public static object Build(object viewModel, Type screenType = null) {
			var vb = GetViewBuilder (viewModel, screenType);
			return Activate (vb);
		}

		public static TView Build<TView> (object viewModel)
		{
			return (TView)Build (viewModel, typeof(TView));
		}

		public static object Activate(IViewBuilder viewBuilder) {
			try { 
				return viewActivator.Activate (viewBuilder);
			}
			catch (Exception ex) {
				if (ex.InnerException != null)
					throw ex.InnerException;

				throw ex;
			}
		}
	}
}

