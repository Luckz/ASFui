using System;
using System.Windows.Forms;

namespace ASFui {
	internal static class Program {
		[STAThread]
		private static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var asfUI = new ASFui();
			asfUI.Text += @" - v" + Application.ProductVersion;
			Application.Run(asfUI);
		}
	}
}
