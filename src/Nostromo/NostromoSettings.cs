using System;
using Nostromo.Interfaces;
using System.Reflection;

namespace Nostromo
{
	public class NostromoSettings {
		public IIOCContainer IOCImplementation { get; set ;}
		public IPlatformViewActivator ViewActivator { get; set; }
		internal Assembly[] AssembliesToRegisterDependenciesFor {
			get;
			set;
		}

		internal Assembly[] AssembliesToRegisterViewsFor {
			get;
			set;
		}
	}
	
}
