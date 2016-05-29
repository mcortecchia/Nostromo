using System;
using Xamarin.Forms;
using System.Linq.Expressions;
using System.Reflection;

namespace Nostromo
{
	public static class XFProperty
	{
		public static BindableProperty CreateOneWay<TView, TProp>( Expression<Func<TView,TProp>> prop, TProp defaultValue = default(TProp) )
			where TView: BindableObject
		{
			return Create(prop, defaultValue, BindingMode.OneWay);
		}
			
		
		public static BindableProperty Create<TView, TProp>( Expression<Func<TView,TProp>> prop, TProp defaultValue = default(TProp), BindingMode mode = BindingMode.TwoWay )
			where TView: BindableObject
		{
			return BindableProperty.Create<TView, TProp>(
				prop,
				defaultValue,
				mode,
				null, 
				(b,o,v) => SetPropertyValue((b as TView), prop, v));
			
		}

		public static void SetPropertyValue<T, TProp>(this T target, Expression<Func<T, TProp>> memberLamda, object value)
		{
			var memberSelectorExpression = memberLamda.Body as MemberExpression;
			if (memberSelectorExpression != null)
			{
				var property = memberSelectorExpression.Member as PropertyInfo;
				if (property != null)
				{
					property.SetValue(target, value, null);
				}
			}
		}
	}
}

