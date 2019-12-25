using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace babytrigger
{
	static class Program
	{
		public static bool StartMinimized = false;

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			foreach (string arg in args)
			{
				if ((System.String.Compare(arg.ToUpperInvariant(), "--MINIMIZED", System.StringComparison.Ordinal) == 0) ||
					(System.String.Compare(arg.ToUpperInvariant(), "-M", System.StringComparison.Ordinal) == 0))
					StartMinimized = true;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
