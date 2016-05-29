using System;
using System.Reflection;
using Nostromo.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Nostromo
{
	public class ViewInitializer {
		public Type ViewType { get; set; }
		public Func<Container, object> Factory { get; set; }
	}


	public class GenericPlatformViewActivator: IPlatformViewActivator
	{
		Container _container;
		Container iocContainer { get { return _container ?? (_container = Container.Default); } } 


		readonly Dictionary<Type, List<ViewInitializer>> _factories = new Dictionary<Type, List<ViewInitializer>>();

		public class ViewBuilder: IViewBuilder {
			public WeakReference Model { get; set; }
			public Type RequestedViewType { get; set; } = null;

			#region IViewBuilder implementation
			public IViewBuilder WithViewModel (object viewModel)
			{
				Model = new WeakReference (viewModel);
				return this;
			}
			public IViewBuilder OfType (Type screenType)
			{
				RequestedViewType = screenType;
				return this;
			}
			#endregion
		}

		public GenericPlatformViewActivator ()
		{
		}

		public virtual void RegisterView (Type viewType, Type viewModelType) {
			RegisterViewFactory (viewModelType, (container => container.Build (viewType)), viewType);
		}

		#region IPlatformViewActivator implementation

		public virtual void RegisterView<TView, TViewModel> ()
		{
			RegisterView (typeof(TView), typeof(TViewModel));
		}

		public virtual void RegisterViewFactory<TViewModel> (Func<Container, object> viewFactory, Type outType = null)
		{
			RegisterViewFactory(typeof(TViewModel), viewFactory, outType);
		}


		Dictionary<Type,HashSet<Type>> _viewsForViewModels = new Dictionary<Type, HashSet<Type>>();
		public virtual void RegisterViewFactory(Type viewModelType, Func<Container, object> viewFactory, Type viewType)
		{
			if (!_factories.ContainsKey (viewModelType)) {
				_factories [viewModelType] = new List<ViewInitializer> ();
			}

			if (!_viewsForViewModels.ContainsKey (viewType))
				_viewsForViewModels.Add (viewType, new HashSet<Type> ());

			_viewsForViewModels [viewType].Add (viewModelType);
			_factories [viewModelType].Add( new ViewInitializer {
				Factory = viewFactory, 
				ViewType = viewType
			});
		}
//
//		public virtual void RegisterAssemblyViews (System.Reflection.Assembly assembly)
//		{
//			foreach (var viewTypeDefinition in assembly.ExportedTypes
//				.Select(type => Tuple.Create(type, type.GetTypeInfo().GetCustomAttributes(typeof(ViewForAttribute), true)))
//				.SelectMany( tuple => tuple.Item2.Select( attr => Tuple.Create(tuple.Item1, attr as ViewForAttribute))))
//			{
//				RegisterView (viewTypeDefinition.Item1, viewTypeDefinition.Item2.Type);
//			}
//		}

		public virtual IViewBuilder CreateViewBuilder ()
		{
			return new ViewBuilder ();
		}

		public virtual ViewInitializer GetInitializer(Type viewModelType, Type requestedViewType = null, bool tryWithBaseType=true) {
			ViewInitializer initializer = null;
			List<ViewInitializer> initializers;
			if (_factories.TryGetValue(viewModelType, out initializers))
			{
				if (requestedViewType != null) {
					initializer = initializers.LastOrDefault (v => v.ViewType.IsOrInerithFrom (requestedViewType));
				} else {
					initializer = initializers.LastOrDefault ();
				}
			}

			if (initializer == null && tryWithBaseType) {
				var baseType = viewModelType.GetTypeInfo ().BaseType;
				if (baseType != null && baseType != typeof(object))
					initializer = GetInitializer (baseType, requestedViewType, tryWithBaseType);
			}

			return initializer;
		}


		public virtual object Activate (IViewBuilder viewBuilder)
		{
			object view = null;
			Type requestedViewType = (viewBuilder as ViewBuilder).RequestedViewType;
			object viewModel = (viewBuilder as ViewBuilder).Model?.Target;
			var initializer = GetInitializer (viewModel.GetType(), requestedViewType);

			if (initializer != null) {
				view = initializer.Factory (this.iocContainer);
				var viewForModels = _viewsForViewModels [view.GetType()];
				PopulateView (view, viewModel, viewForModels);
			} else {
				throw new Exception (
					String.Format("Initializer of type \"{1}\" not found for viewmodel: \"{0}\"", viewModel, requestedViewType)
				);
			}

			return view;
		}

		public virtual void PopulateView (object view, object viewModel, IEnumerable<Type> viewForTypes)
		{
			foreach (var viewModelProp in view.GetType().GetRuntimeProperties()
				.Where( p => p.CanWrite
					&& p.SetMethod != null
					&& (
						p.PropertyType.IsOrInerithFrom(typeof(IPageViewModel)) 
						|| viewForTypes.Any( t => p.PropertyType.IsOrInerithFrom(t))
					)
					&& viewModel.IsOrInerithFrom(p.PropertyType))) {
				viewModelProp.SetValue (view, viewModel);
			}
		}

		public virtual Type GetViewTypeForModel (Type viewModelType, Type requestedViewType = null)
		{
			var initializer = GetInitializer (viewModelType, requestedViewType);

			if (initializer != null)
				return initializer.ViewType;

			return null;
		}

		#endregion
	}
}

