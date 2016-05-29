using System;
using Nostromo.Interfaces;
using System.Reflection;
using System.Linq;

namespace Nostromo
{

	public static class NostroInitializationExtension {
		public static NostromoSettings RegisterDependenciesInAssemblyOf<TTypeForAssembly>(this NostromoSettings settings) {
			return settings.RegisterAssemblyDependencies (typeof(TTypeForAssembly).GetTypeInfo ().Assembly);
		}

		public static NostromoSettings UsePlatformViewActivator(this NostromoSettings settings, IPlatformViewActivator activator) {
			settings.ViewActivator = activator;
			return settings;
		}

		public static NostromoSettings UseGenericPlatformViewActivator( this NostromoSettings settings, Action<IPlatformViewActivator> withActivator = null ){
			var activator = new GenericPlatformViewActivator ();
			if (withActivator!=null)
				withActivator (activator);
			return settings.UsePlatformViewActivator(activator);
		}

		public static NostromoSettings RegisterAssemblyDependencies(this NostromoSettings settings, Assembly assembly) {
			settings.AssembliesToRegisterDependenciesFor = (settings.AssembliesToRegisterDependenciesFor ?? new Assembly[] {})
				.Concat( new[] { assembly })
				 .ToArray();
			
			return settings;
		}

		public static NostromoSettings RegisterViewsInAssemblyOf<TTypeForAssembly>(this NostromoSettings settings) {
			return settings.RegisterAssemblyViews (typeof(TTypeForAssembly).GetTypeInfo ().Assembly);
		}

		public static NostromoSettings RegisterAssemblyViews(this NostromoSettings settings, Assembly assembly) {
			settings.AssembliesToRegisterViewsFor = (settings.AssembliesToRegisterViewsFor ?? new Assembly[] {})
				.Concat( new[] { assembly })
				.ToArray();

			return settings;
		}

		public static void Init(this NostromoSettings config) {
			NostromoApp.Initialize (config);
		}

		public static IPlatformViewActivator RegisterView<TView, TViewModel>( this IPlatformViewActivator activator) { activator.RegisterView<TView,TViewModel> (); return activator; }
		public static IPlatformViewActivator RegisterViewFactory<TViewModel, TView>( this IPlatformViewActivator activator, Func<Container, TView> viewFactory ) { activator.RegisterViewFactory(typeof(TViewModel), c => viewFactory(c), typeof(TView)); return activator; }

		public static IPlatformViewActivator RegisterAllViewsInAssemblyOf<TTypeForAssembly> (this IPlatformViewActivator activator)
		{
			return RegisterAssemblyViews (activator, typeof(TTypeForAssembly).GetTypeInfo ().Assembly);	
		}
		public static IPlatformViewActivator RegisterAssemblyViews (this IPlatformViewActivator activator, System.Reflection.Assembly assembly)
		{
			foreach (var viewTypeDefinition in assembly.ExportedTypes
				.Select(type => Tuple.Create(type, type.GetTypeInfo().GetCustomAttributes(typeof(ViewForAttribute), true)))
				.SelectMany( tuple => tuple.Item2.Select( attr => Tuple.Create(tuple.Item1, attr as ViewForAttribute))))
			{
				activator.RegisterView (viewTypeDefinition.Item1, viewTypeDefinition.Item2.Type);
			}
			return activator;
		}

		public static Container RegisterAssemblyDependencies(this Container container, Assembly assembly) {
			foreach (var dependencyDefinition in assembly.ExportedTypes
				.Select(type => Tuple.Create(type, type.GetTypeInfo().GetCustomAttributes(typeof(DependencyAttribute), true)))
				.SelectMany( tuple => tuple.Item2.Select( attr => Tuple.Create(tuple.Item1, attr as DependencyAttribute))))
			{
				var attr = dependencyDefinition.Item2;
				var resolverType = dependencyDefinition.Item1;

				if (attr.AllInterfaces) {
					container.Register (resolverType, true);
				} else {
					foreach (var intf in attr.Intefaces)
						container.Register (intf, resolverType);
				}
			}

			return container;
		}
	}
	
}
