using System;
using Nostromo.Interfaces;
using System.Reflection;

namespace Nostromo
{
	public static class NostromoXamarinFormsExtensions
	{
		public static NostromoSettings UseXamarinFormsViewActivator( this NostromoSettings settings, Action<IPlatformViewActivator> withActivator = null ){
			var activator = new XamarinFormsPlatformViewActivator ();
			if (withActivator!=null)
				withActivator (activator);
			return settings.UsePlatformViewActivator(activator);
		}
	

	}

}

