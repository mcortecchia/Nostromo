using System;
using System.Reflection;

namespace Nostromo
{
	public static class SystemUtilityExtensions
	{
		public static bool IsOrInerithFrom(this object objOrType, Type type) {
			if (objOrType == null || type == null)
				return false;

			var objType = objOrType as Type;
			if (objType == null) {
				objType = objOrType.GetType();
			}

			return type.IsAssignableFrom(objType);
		}

		/// <summary>
		/// Determines if the specified type (fromType) is assignable TO the current Type
		/// </summary>
		/// <returns><c>true</c> if is assignable from the specified thisType fromType; otherwise, <c>false</c>.</returns>
		/// <param name="thisType">This type.</param>
		/// <param name="fromType">From type.</param>
		public static bool IsAssignableFrom(this Type thisType, Type fromType) {
			return thisType.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo());
		}
	}
}

