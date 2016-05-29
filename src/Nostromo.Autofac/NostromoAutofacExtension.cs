using System;
using Nostromo;
using Nostromo.Interfaces;
using Autofac;
using Autofac.Util;
using Nostromo.Autofac;

namespace Nostromo
{
	public static class NostromoAutofacExtension {
		public static NostromoSettings UseAutofacContainer(this NostromoSettings settings) {
			settings.IOCImplementation = new AutofacContainerImplementation ();
			return settings;
		}
	}
	
}
