using System;
using ASFui.Properties;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows.Forms;

namespace ASFui
{
    internal static class Util
    {
		private static readonly NetTcpBinding Binding = new NetTcpBinding { SendTimeout = new TimeSpan(0, 30, 0), Security = { Mode = SecurityMode.None } };

        public static bool CheckBinary()
        {
            return File.Exists(Settings.Default.ASFBinary) || !Settings.Default.IsLocal;
        }

        public static string SendCommand(string command)
        {

			string address = GetEndpointAddress();
			string msg="";
			using (WebClient webclient = new WebClient()) {
                webclient.Encoding = System.Text.Encoding.UTF8;
                webclient.Headers[HttpRequestHeader.ContentType] = "application/json";

                if (!address.Contains("#"))
					msg= webclient.UploadString(address + System.Net.WebUtility.UrlEncode(command), String.Empty);
				else {
					var regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape("#"));
					msg= webclient.UploadString(regex.Replace(address, System.Net.WebUtility.UrlEncode(command), 1), String.Empty);
				}	
			}
			dynamic tmp= JsonConvert.DeserializeObject(msg);
			return tmp.Result;

		}

        public static string GenerateCommand(string command, string user, string args = "")
        {
            return command + " " + user + " " + args;
        }

        public static string MultiToOne(string[] text)
        {
            string command = null;
            text = text.Where(x => !string.IsNullOrEmpty(x) && !Environment.NewLine.Equals(x)).ToArray();
            command += string.Join(",", text);

            return command;
        }

        public static string GetEndpointAddress()
        {
            if (!Settings.Default.IsLocal) return Settings.Default.RemoteURL;
            var json =
                JObject.Parse(
                    File.ReadAllText(Path.GetDirectoryName(Settings.Default.ASFBinary) + @"/config/ASF.json"));

			var hostname = "127.0.0.1";
			var port = "1242";
			//newversion:
			if (null != json["IPCHostname"])
				hostname = json["IPCHostname"].ToString();
			if (null != json["IPCPort"])
				port = json["IPCPort"].ToString();
			if (null == json["IPCPassword"] || string.IsNullOrEmpty(json["IPCPassword"].ToString()))
				return "http://" + hostname + ":" + port + "/Api/Command/";
			else
				return "http://" + hostname + ":" + port + "/Api/Command/#?password=" + json["IPCPassword"].ToString();
        }

        public static bool CheckIfAsfIsRunning()
        {
            return Process.GetProcessesByName("ASF").Length > 0 ||
                   Process.GetProcessesByName("ArchiSteamFarm").Length > 0 ||
                   Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Settings.Default.ASFBinary)).Length > 0;
        }

        public static void CheckVersion()
        {
            
        }
		
		public static void UpgradeSettings()
		{
            if (!Settings.Default.UpdateSettings) return;
            Settings.Default.Upgrade();
            Settings.Default.UpdateSettings = false;
            Settings.Default.Save();
            Logging.Info("ASFui updated to " + new Version(Application.ProductVersion));
        }

        public static bool IsOnScreen(Rectangle rec, double minPercentOnScreen = 0.2)
        {
            var pixelsVisible = Screen.AllScreens.Select(scrn => Rectangle.Intersect(rec, scrn.WorkingArea))
                .Where(r => r.Width != 0 & r.Height != 0)
                .Aggregate<Rectangle, double>(0, (current, r) => current + (r.Width*r.Height));

            return pixelsVisible >= (rec.Width * rec.Height) * minPercentOnScreen;
        }

        public static bool CheckUrl(string url)
        {

            return true;
        }
    }
}