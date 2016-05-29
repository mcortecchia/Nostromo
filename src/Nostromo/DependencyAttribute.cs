using System;
using Nostromo.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace Nostromo
{

	public class DependencyAttribute: Attribute {
		public DependencyAttribute()
		{
			AllInterfaces = true;
		}

		public DependencyAttribute(params Type[] interfaces) { 
			foreach (var intf in interfaces) {
				if (!intf.GetTypeInfo ().IsInterface) {
					throw new Exception (String.Format ("{0} is not an interface", intf));
				}
			}

			AllInterfaces = false;
			Intefaces = interfaces; 
		}

		public bool AllInterfaces { get; private set; }
		public Type[] Intefaces { get; private set; }
	}
}
