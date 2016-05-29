using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Nostromo.Interfaces;
using System.Reflection;
using System.Linq.Expressions;

namespace Nostromo
{
	public static class Build {
		public static OfType ViewModel<OfType>()
			where OfType: class 
		{
			return ViewModelBuilder.Build<OfType> ();
		}

		public static OfType ViewModel<OfType>(Func<OfType> factory)
			where OfType: class 
		{
			return (OfType)ViewModelBuilder.Build (() => factory());
		}
		public static object ViewModel(Type ofType) {
			return ViewModelBuilder.Build(ofType);
		}
		public static object ViewModel(string route) {
			return ViewModelBuilder.Build(route);
		}
		public static OfType ViewModel<OfType>(string route)
			where OfType: class 
		{
			return ViewModelBuilder.Build<OfType>(route);
		}
		public static object ViewModel(string route, Type ofType) {
			return ViewModelBuilder.Build(route, ofType, ofType);
		}
		public static object ViewModel(string route, Type outType, Type ofType) {
			return ViewModelBuilder.Build(route, outType, ofType);
		}

		public static object View(object viewModel, Type baseType) {
			return ViewBuilder.Build (viewModel, baseType);
		}
		public static OfType View<OfType>(object viewModel) 
			where OfType: class 
		{
			return ViewBuilder.Build<OfType>(viewModel);
		}
//		public static OfType View<OfType>(object viewModel) {
//			return (OfType)ViewBuilder.Build (viewModel);
//		}
	}
	
}
