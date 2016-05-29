using System;
using Nostromo;

using Xamarin.Forms;
using System.Reflection;
using System.Linq;

namespace Nostromo.XForms.Test
{
	[Dependency]
	public class RandomPasswordGenerator {
		public string Generate(int count, string characters) {
			Random rnd = new Random();
			return String.Concat (Enumerable.Range (0, count).Select (i => characters [(int)rnd.Next (characters.Length)]));
		}
	}
	
}
