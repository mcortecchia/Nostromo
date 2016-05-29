using System;
using Nostromo;
using Nostromo.Interfaces;
using Autofac;
using Autofac.Util;

namespace Nostromo.Autofac
{
	public class AutofacContainerImplementation: IIOCContainer
	{
		class WrapDisposable : Disposable {
			Action onDispose { get; set; }
			public WrapDisposable(Action onDispose) {
				this.onDispose = onDispose;
			}
			protected override void Dispose (bool disposing)
			{
				base.Dispose (disposing);

			}
		}


		IContainer cnt;

		ContainerBuilder builderSection;

		public AutofacContainerImplementation ()
		{

		}

		public IDisposable WrapBuildup ()
		{
			if (builderSection != null) {
				throw new Exception ("Already in autofac wrap buildup");
			}

			builderSection = new ContainerBuilder ();	
			return  new WrapDisposable (() => checkBuildStatus (builderSection));
		}

		void checkBuildStatus (ContainerBuilder builder)
		{
			var finalize = builder == builderSection;
			if (finalize || builderSection == null) {
				builder = builderSection ?? builder;
				if (cnt == null) {
					cnt = builder.Build ();
				} else {
					builder.Update (cnt);
				}

				if (finalize) {
					builderSection = null;
				} else {
					if (builderSection != null) {
						builderSection = new ContainerBuilder ();
					}
				}
			}
		}

		void checkInwrapOperation ()
		{
			if (builderSection != null) {
				checkBuildStatus (builderSection);
				//Devo ricreare il builder perchè sono in mezzo ad un wrapped 
				builderSection = new ContainerBuilder();
			}
		}

		void WithBuilder (Action<ContainerBuilder> dowithbuild)
		{
			var builder = builderSection ?? new ContainerBuilder ();
			dowithbuild (builder);
			if (builder != builderSection)
				checkBuildStatus (builder);
		}

		#region IIOCContainerImplementation implementation

		public void RegisterInstance (object service, Type typeOfRegistration)
		{
			WithBuilder (builder => builder.RegisterInstance (service).As (typeOfRegistration));
		}

		public void RegisterInstance (object service)
		{
			WithBuilder (builder => builder.RegisterInstance (service).AsSelf());
		}


		public void RegisterAsSingleton (Type type)
		{
			WithBuilder (builder => builder.RegisterTypes (type).SingleInstance ());
		}

		public void RegisterAsSingleton (Type typeOfRegistration, Type typeImplementation)
		{
			WithBuilder (builder => builder.RegisterTypes (typeOfRegistration).As (typeImplementation));
		}

		public void Register (Type typeOfRegistration, Func<IIOCContainer, Type, object> factory)
		{
			WithBuilder (builder => builder.Register ((af) => factory (this, typeOfRegistration)).As (typeOfRegistration));
		}

		public object Resolve (Type t)
		{
			checkInwrapOperation ();
			return cnt.Resolve (t);
		}



		public bool IsRegistered (Type t)
		{
			checkInwrapOperation ();
			return cnt.IsRegistered (t);
		}

		public object Build (Type objectTypeToBuild)
		{
			return cnt.Resolve (objectTypeToBuild);
		}

		public void InjectProperties(object obj) {
			cnt.InjectUnsetProperties (obj);
		}

		#endregion

	}
}

