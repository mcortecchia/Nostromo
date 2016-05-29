using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Nostromo.Interfaces;
using System.Reflection;
using System.Linq.Expressions;

namespace Nostromo
{
	public static class ViewModelBuilder
	{
		static Dictionary<Regex, Type> s_pageRouting = new Dictionary<Regex, Type> ();
		static Dictionary<Type, Regex[]> s_inverseRouting = new Dictionary<Type, Regex[]> ();

		static internal void RegisterRoutes (Type pageType, params string[] routes)
		{
			var regexRoutes = routes.Select (route => {
				var rePath = route;
				if (!route.StartsWith ("^", StringComparison.Ordinal)) {
					rePath = rePath.Replace ("{", "(?<").Replace ("}", @">[\w_]+)");
				}

				rePath = "^" + rePath.TrimStart ('^').TrimEnd ('$') + "$"; //Mi assicuro che la regex abbia tutto il path
				return new Regex (rePath, RegexOptions.ExplicitCapture);
			}).ToList ();

			foreach (var re in regexRoutes) {
				s_pageRouting [re] = pageType;
			}			

			s_inverseRouting [pageType] = regexRoutes.ToArray ();
		}

		/// <summary>
		/// Crea la pagina in base al route (registrato) che erediti dal tipo richiesto, se specificato
		/// </summary>
		/// <param name="route">Route.</param>
		/// <param name = "outType">Il tipo della pagina che viene creata (indipendentemente dal tipo richiesto)</param>
		/// <param name="ofType">Of type.</param>

		public static object Build (string route, Type outType = null, Type ofType = null)
		{
			var pageType = TypeForRoute (route, ofType);
			if (outType == null || pageType.IsOrInerithFrom (outType)) {
				outType = pageType;
			} 

			if (pageType != null && outType.IsOrInerithFrom (pageType)) {
				return Build (outType, route, s_inverseRouting [pageType]);
			}

			return null;
		}

		/// <summary>
		/// Crea la pagina, facendone autoInject e chiamando la load
		/// </summary>
		/// <param name="viewModelFactory">viewModel type.</param>
		public static object Build (Expression<Func<object>> viewModelFactory)
		{
			try {
				var viewModel = Container.Default.Build (viewModelFactory.Compile());
				var pm = viewModel as IPageViewModel;

				if (pm != null) {
					pm.Load ();
				}

				return viewModel;
			} catch (Exception ex) {
				if (ex.InnerException != null)
					throw new Exception("error building: "+viewModelFactory.ToString(), ex.InnerException);

				throw ex;
			}
		}

		/// <summary>
		/// Crea la pagina, facendone autoInject e chiamando la load
		/// </summary>
		/// <param name="viewModelType">viewModel type.</param>
		public static object Build (Type viewModelType)
		{
			return Build (() => Container.Default.Build (viewModelType));
		}

		/// <summary>
		/// crea la pagina in base al tipo ed al route
		/// </summary>
		/// <param name="route">Route.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Build<T> (string route)
			where T: class
		{
			return (T)Build (route, ofType: typeof(T));
		}

		/// <summary>
		/// crea la pagina in base al tipo, ne fa autoinject e chiama load
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Build<T> ()
			where T: class
		{
			return (T)Build (typeof(T));
		}


		public static Type TypeForRoute (string route, Type ofType = null)
		{
			return s_pageRouting.Where (mapRoute => 
				(ofType == null || mapRoute.Value.IsOrInerithFrom (ofType))
			&& mapRoute.Key.IsMatch (route))
					.Select (mapRoute => mapRoute.Value)
					.FirstOrDefault ();
		}

		/// <summary>
		/// Crea la pagina in base al tipo ed al route
		/// </summary>
		/// <param name="viewModelType">viewModelType</param>
		/// <param name="route">Route.</param>
		/// <param name="routes">Routes.</param>
		internal static object Build (Type viewModelType, string route, params Regex[] viewModelRoutes)
		{

			var viewModel = Container.Default.Build (viewModelType);
			if (viewModel != null) {
				PopulateViewModel (viewModel, route, viewModelRoutes);
			}
			var pm = viewModel as IPageViewModel;
			if (pm != null) {
				pm.Load ();
			}

			return null;
		}

		static void PopulateViewModel (object viewModel, string route, params Regex[] viewModelRoutes)
		{
			var routeProp = viewModel.GetType ().GetRuntimeProperty ("Route");
			if (routeProp != null && routeProp.SetMethod != null && routeProp.CanWrite) {
				routeProp.SetValue (viewModel, route);
			}

			foreach (var regex in viewModelRoutes) {
				PopulateViewModelWithRegexRoute (viewModel, route, regex);
			}
		}

		static void PopulateViewModelWithRegexRoute (object viewModel, string route, Regex routeRegEx)
		{
			var match = routeRegEx.Match (route);
			var vmType = viewModel.GetType ();
			if (match.Success) {
				foreach (var memberName in routeRegEx.GetGroupNames ()) {
					if (match.Groups [memberName].Success) {
						string value = match.Groups [memberName].Value;

						PropertyInfo member = vmType.GetRuntimeProperty (memberName);
						if (member != null && member.SetMethod != null && member.CanWrite) {
							object val = Convert.ChangeType (value, member.PropertyType, System.Globalization.CultureInfo.InvariantCulture);
							member.SetValue (viewModel, val);
						}
					}
				}
			}
		}
	}
}

