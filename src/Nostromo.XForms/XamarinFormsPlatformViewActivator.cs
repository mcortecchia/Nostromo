using System;
using Nostromo.Interfaces;
using System.Reflection;

namespace Nostromo
{

	public class XamarinFormsPlatformViewActivator: GenericPlatformViewActivator {
		public XamarinFormsPlatformViewActivator() {
			//Registra tutte le views di questo assembly
			this.RegisterAssemblyViews (typeof(XamarinFormsPlatformViewActivator).GetTypeInfo ().Assembly);
		}
	}
}
