using System;
using System.Reflection;

namespace Nostromo.Interfaces
{
	public interface IPlatformViewActivator
	{
		Type GetViewTypeForModel(Type viewModelType, Type requestedViewType = null);
		void RegisterView(Type viewType, Type viewModelType);
		void RegisterViewFactory(Type viewModelType, Func<Container, object> viewFactory, Type viewType);

		IViewBuilder CreateViewBuilder ();
		object Activate(IViewBuilder viewBuilder);
	}

	public interface IViewBuilder {
		IViewBuilder WithViewModel (object viewModel);
		IViewBuilder OfType (Type screenType);
	}
}

