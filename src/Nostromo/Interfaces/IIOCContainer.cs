using System;
using System.ComponentModel;

namespace Nostromo.Interfaces
{
	public interface IIOCContainer
	{
		/// <summary>
		/// Set up a session in which you can register all your service in an optimized way.
		/// </summary>
		/// <returns>A disposable used to finalize the buildup of the internal container</returns>
		IDisposable WrapBuildup();

		/// <summary>
		/// Register the instance as a service for the type
		/// </summary>
		/// <param name="service">Service.</param>
		/// <param name = "typeOfRegistration">The registered type</param>
		void RegisterInstance (object service, Type typeOfRegistration);

		/// <summary>
		/// Register the instance as a service for the concrete type definition
		/// </summary>
		void RegisterInstance (object service);

		/// <summary>
		/// Register the type as a single instance service
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		void RegisterAsSingleton (Type type) ;

		/// <summary>
		/// Register the typeImplementation as a single instance for the typeOfRegistration
		/// </summary>
		void RegisterAsSingleton (Type typeOfRegistration, Type typeImplementation);

		/// <summary>
		/// Register the factory as the implementation for the typeRegistration
		/// </summary>
		/// <param name = "typeOfRegistration"></param>
		/// <param name="factory">Factory.</param>
		void Register (Type typeOfRegistration, Func<IIOCContainer, Type, object> factory);

		/// <summary>
		/// Tries to resolve the type
		/// </summary>
		/// <param name="type">T.</param>
		object Resolve (Type type);

		void InjectProperties (object obj);

		/// <summary>
		/// Determines whether the specified type is registered .
		/// </summary>
		/// <returns><c>true</c> if the type is registered; otherwise, <c>false</c>.</returns>
		/// <param name="type">T.</param>
		bool IsRegistered (Type type);
	}
}

