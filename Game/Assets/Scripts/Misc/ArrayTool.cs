using System;
using System.Linq;

namespace AssemblyCSharp
{
	public class ArrayTool
	{
		public static double[] Binarize(double[] arr) {
			double[] result = new double[arr.Length];

			int id = 0;
			double max = 0;
			for (int i = 0; i < arr.Length; i++) {
				double d = arr [i];
				if (max < d) {
					max = d;
					id = i;
				}
			}
			result [id] = 1;
			return result;
		}

		public static string ToString(double[] arr) {
			return "[" + string.Join (", ", arr.Select (p => p.ToString ("0.##")).ToArray ()) + "]";
		}
	}
}
