using System;
using System.Reflection;
using System.Linq;

namespace Nostromo
{
	public class Container
	{
		public static Interfaces.IIOCContainer iocImplementation { get; set; }

		public static Container Default { get; private set; }

		public Container ()
		{
			if (Default == null) {
				Default = this;
				Register<Container> (this);
			}
		}

		/// <summary>
		/// Registra l'istanza come servizio
		/// </summary>
		/// <param name="service">Service.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Register<T> (T service)
			where T:class
		{
			iocImplementation.RegisterInstance (service, typeof(T));
		}


		public void Register (object service, Type typeOfRegistration)
		{
			iocImplementation.RegisterInstance (service, typeOfRegistration);
		}


		public void Register (object service, bool registerAllInterfaces = true) {
			iocImplementation.RegisterInstance (service);
			if (registerAllInterfaces) {
				var t = service.GetType ();
				foreach (var if_of_t in t.GetTypeInfo().ImplementedInterfaces){ 
					Register (service, if_of_t); 
				};
			}
		}


		/// <summary>
		/// Registra il tipo singleton e (se registerAllInterfaces == true) registra tutte le interfacce associate al tipo
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Register<T> (bool registerAllInterfaces = true) 
			where T: class
		{
			Register (typeof(T), registerAllInterfaces);
		}

		/// <summary>
		/// Registra il tipo singleton e (se registerAllInterfaces == true) registra tutte le interfacce associate al tipo
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Register (Type type, bool registerAllInterfaces = true) 
		{
			iocImplementation.RegisterAsSingleton(type);

			if (registerAllInterfaces) {
				//Registro tutte le interfacce implementate dal tipo.
				foreach (var if_of_t in type.GetTypeInfo().ImplementedInterfaces){ 
					iocImplementation.Register (if_of_t, (c, p) => c.Resolve (type));
				};
			}
		}

		/// <summary>
		/// Registra l'interfaccia e viene risolta con una istanza (singleton) del tipo resolve (1 singleton per ogni istanza)
		/// </summary>
		/// <typeparam name="TRegistration">The 1st type parameter.</typeparam>
		/// <typeparam name="TImplementation">The 2nd type parameter.</typeparam>
		public void Register<TRegistration, TImplementation> ()
			where TImplementation:class, TRegistration
			where TRegistration:class
		{
			Register (typeof(TRegistration), typeof(TImplementation));
		}


		/// <summary>
		/// Registra l'interfaccia e viene risolta con una istanza (singleton) del tipo resolve (1 singleton per ogni istanza)
		/// </summary>
		public void Register (Type typeRegistration, Type typeImplementation)
		{
			iocImplementation.RegisterAsSingleton (typeRegistration, typeImplementation);
		}


		/// <summary>
		/// registra un tipo che viene risolto OGNI volta con la factory
		/// </summary>
		/// <param name="factory">Factory.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Register<T> (Func<T> factory)
			where T: class
		{
			Register (typeof(T), () => factory ());
		}

		public void Register (Type tRegistration, Func<object> factory)
		{
			iocImplementation.Register (tRegistration, (c, r) => factory ());
		}

		public T Resolve<T> ()
			where T: class
		{
			return Resolve (typeof(T)) as T;
		}

		public object Resolve (Type t)
		{
			return iocImplementation.Resolve (t);
		}

		public bool IsRegistered<T>() 
			where T: class
		{
			return iocImplementation.IsRegistered (typeof(T));
		}

		public bool IsRegistered( Type t )
		{
			return iocImplementation.IsRegistered (t);
		}

		public object Build(Type objectTypeToBuild) {
			var constructor = objectTypeToBuild
				.GetTypeInfo ()
				.DeclaredConstructors
				.FirstOrDefault (c => c.GetParameters ().Length == 0);
		
			if (constructor != null)
				return Build (() => constructor.Invoke (new object[] { }));

			return null;
		}

		public object Build(Func<object> objectFactory) {
			var obj = objectFactory ();
			iocImplementation.InjectProperties (obj);
			return obj;
		}

	
	}
}

