using System;
using Nostromo.Interfaces;

namespace Nostromo
{


	public static class NostromoApp
	{
		public static Container Container {
			get;
			private set;
		}

		public static void Initialize (NostromoSettings config)
		{
			Container.iocImplementation = config.IOCImplementation;
			if (Container.iocImplementation == null) {
				throw new Exception ("the IOCImplementation isn't specified");
			}

			Container = (new Container ());
			if (config.ViewActivator != null) {
				Container.Register<IPlatformViewActivator> (config.ViewActivator);
			}

			foreach (var assembly in config.AssembliesToRegisterDependenciesFor??new System.Reflection.Assembly[]{}) {
				Container.RegisterAssemblyDependencies (assembly);
			}

			foreach (var assembly in config.AssembliesToRegisterViewsFor??new System.Reflection.Assembly[]{}) {
				if (config.ViewActivator == null) {
					throw new Exception ("the IPlatformViewActivator isn't specified");
				}

				config.ViewActivator.RegisterAssemblyViews (assembly);
			}
		}

		public static NostromoSettings Setup() {
			return new global::Nostromo.NostromoSettings ();
		}
	}
}

