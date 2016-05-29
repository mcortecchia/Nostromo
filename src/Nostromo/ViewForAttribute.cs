using System;
using Nostromo.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace Nostromo
{
	public class ViewForAttribute: Attribute {
		public ViewForAttribute(Type viewModelType) { Type = viewModelType; }
		public Type Type { get; private set; }
	}
}
