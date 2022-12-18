using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

#if DEBUG
using System.Diagnostics;
#endif

using Newtonsoft.Json;

namespace MidiControl {
	public partial class UpdateCheckerGUI : Form {
		private readonly Size normalSize = new Size(406, 345);
		private readonly Size progressSize = new Size(406, 105);

		private bool updateFound = false;

		private GithubReleasesAPIResponse.Root available = null;
		private string githubLink = "https://github.com/Etuldan/MidiControl/releases";

		public bool AppShouldClose { get; private set; }
		public string UpdateExe { get; private set; }

		public UpdateCheckerGUI() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MIDIControlGUI));
			InitializeComponent();
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

			this.Size = this.progressSize;
			this.FormBorderStyle = FormBorderStyle.Fixed3D;

			this.Text = "Updates - Current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion;

			grpBox.Text = "Checking for updates...";

			btnDownload.Enabled = false;
			lnkGithubLink.Visible = false;

			ThemeSupport.ThemeOtherWindow((new OptionsManagment()).options.Theme, this);
		}

		private void UpdateCheckerGUI_Load(object sender, EventArgs e) {
			this.Size = this.progressSize;
			this.Show();

			var title = "Updates - Current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion;

			// (https://stackoverflow.com/questions/19028374/accessing-ui-controls-in-task-run-with-async-await-on-winforms/25903258#25903258)
			Task.Run(() => {
				// check for updates here
#if DEBUG
				Debug.WriteLine("Checking for updates... url = " + Program.urlUpdates);
				Debug.WriteLine("current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion);
#endif

				using(var http = new WebClient()) {
					//http.Proxy = System.Net.WebRequest.DefaultWebProxy;
					//http.Credentials = System.Net.CredentialCache.DefaultCredentials;
					http.UseDefaultCredentials = true;
					//http.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
					http.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

					string json;

					try {
						http.Timeout = 5000; // 5 second timeout instead of the default
						json = http.DownloadString(Program.urlUpdates);
					} catch(System.Net.WebException ex) {
						//this.Size = this.normalSize;
						//this.FormBorderStyle = FormBorderStyle.Sizable;
						//progressBar1.Visible = false;

						//grpBox.Text = "Unable to check for updates";
						//txtReleaseNotes.Text = "An error occurred while checking for updates:\n\n" + ex.Message;

						DisplayResults("Unable to check for updates", "An error occurred while checking for updates:\r\n\r\n" + ex.Message, title, 1);

						http.Dispose();

						return;
					}

					//this.Size = this.normalSize;
					//this.FormBorderStyle = FormBorderStyle.Sizable;
					//progressBar1.Visible = false;

					// attempt to parse the result
#if DEBUG
					Debug.WriteLine("json = \n" + json);
#endif
					List<GithubReleasesAPIResponse.Root> data = new List<GithubReleasesAPIResponse.Root>();
					try {
						data = JsonConvert.DeserializeObject<List<GithubReleasesAPIResponse.Root>>(json);
					} catch(Exception ex) {
						//grpBox.Text = "Error parsing received data";
						//txtReleaseNotes.Text = "An error occurred while checking for updates:\n\n" + ex.Message;
						DisplayResults("Error parsing received data", "An error occurred while checking for updates:\r\n\r\n" + ex.Message, title, 1);
						return;
					}

					// each element in data is a .Root object, denoting a single release
					// need to look at:
					// .Root.TagName - the release tag containing the version number to compare against
					// .Root.Url - the URL pointing to that release (for opening in a browser)
					// .Root.Name - release's name
					// .Root.PublishedAt - the date/time the release was posted
					// .Root.Body - the release notes

					// this should all be enclosed in a loop probably, but for now, just check the first one

					var currentVersion = ParseVersionString(Application.ProductVersion);
					//var releaseVersion = ParseVersionString(data[0].TagName.Replace("v", ""));

					int mostRecent = -1;
					int[] releaseVersion = currentVersion;

					// check major version; update if newer, otherwise match and check deeper
					for(int i = 0; i < data.Count; i++) {
						int[] rv = ParseVersionString(data[i].TagName.Replace("v", ""));

						if(versionIsNewer(rv, releaseVersion)) {
							mostRecent = i;
							releaseVersion = rv;
							updateFound = true;
						}
					}

#if DEBUG
					Debug.WriteLine("- most recent version available is: " + String.Join(".", releaseVersion));
#endif

					if(mostRecent > -1)
						available = data[mostRecent];

					if(updateFound) {
						//grpBox.Text = "An update is available!";
						var releaseNotes = data[mostRecent].Name + "\r\n" + data[mostRecent].PublishedAt + "\r\n\r\n" + data[mostRecent].Body;
						//txtReleaseNotes.Text = data[0].Name + "\r\n" + data[0].PublishedAt + "\r\n\r\n" + data[0].Body;
						//this.Text = "Updates - Current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion;
						//btnDownload.Enabled = true;
						DisplayResults("An update is available!", releaseNotes, title, 0);
					} else {
						//grpBox.Text = "No updates are available at this time.";
						//txtReleaseNotes.Text = "You are currently on the latest version :)";

						//this.Text = "Up to date - Current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion;
						DisplayResults("No updates are available at this time.", "You are already on the latest version :)", title, 1);
					}
				}
			});
		}

		private void DisplayResults(string caption, string body, string title, int layout) {
			if(InvokeRequired) {
				Invoke((Action<string, string, string, int>) DisplayResults, caption, body, title, layout);
				return;
			}

			this.Text = title;

			this.Size = this.normalSize;
			int dh = this.normalSize.Height - this.progressSize.Height;
			this.Top -= (int)(dh / 2);

			this.FormBorderStyle = FormBorderStyle.Sizable;
			progressBar1.Visible = false;
			lnkGithubLink.Visible = true;

			grpBox.Text = caption;
			txtReleaseNotes.Text = body;

			// 0 - show download button
			// 1 - Close button only
			switch(layout) {
				case 0:
					btnDownload.Visible = true;
					btnDownload.Enabled = true;
					break;
				case 1:
					btnDownload.Visible = false;
					btnDownload.Enabled = false;
					btnCancel.Text = "Close";
					break;
			}
		}

		private void btnDownload_Click(object sender, EventArgs e) {
			if(available != null) {
				btnDownload.Enabled = btnCancel.Enabled = false;

				// need to fetch the Setup.exe asset and download it
				using(var http = new WebClient()) {
					http.UseDefaultCredentials = true;
					http.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

					// find the asset containing "Setup" and "exe"
					int asset = -1;
					for(int i = 0; i < available.Assets.Count; i++) {
						if(available.Assets[i].Name.ToLower().Contains("setup") && available.Assets[i].Name.ToLower().Contains("exe")) {
							asset = i;
						}
					}

					if(asset == -1) {
						// couldn't find an exe?
						// just open the release page in a browser for a usable experience
						System.Diagnostics.Process.Start(available.Url);

						btnDownload.Enabled = btnCancel.Enabled = true;
						return;
					}

					var dest = UpdateCheckerGUI.getDownloadsFolder() + "\\" + available.Assets[asset].Name;

					try {
						http.Timeout = 5000; // 5 second timeout instead of the default
						http.DownloadFile(available.Assets[asset].BrowserDownloadUrl, dest);
					} catch(System.Net.WebException ex) {
						System.Diagnostics.Process.Start(available.Url);
						btnDownload.Enabled = btnCancel.Enabled = true;
						return;
					}

					// file downloaded
					if(MessageBox.Show("Update downloaded successfully to " + dest + ".  Install now?", "Update downloaded", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
						this.AppShouldClose = true;
						this.UpdateExe = dest;
					}
				}

				btnDownload.Enabled = btnCancel.Enabled = true;

				if(this.AppShouldClose) {
					DialogResult = DialogResult.Yes;
					Close();
				}
			}
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			if(this.updateFound) {
				// update available but declined; return Cancel
				DialogResult = DialogResult.Cancel;
			} else {
				// no update available, return No
				DialogResult = DialogResult.No;
			}

			Close();
		}

		// subclass of System.Net.WebClient that allows for changing timeout value
		// https://stackoverflow.com/questions/1789627/how-to-change-the-timeout-on-a-net-webclient-object
		private class WebClient : System.Net.WebClient {
			public int Timeout { get; set; }

			protected override System.Net.WebRequest GetWebRequest(Uri uri) {
				System.Net.WebRequest lWebRequest = base.GetWebRequest(uri);
				lWebRequest.Timeout = Timeout;
				((System.Net.HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
				return lWebRequest;
			}
		}

		private int[] ParseVersionString(string ver) {
			var v = ver.Split('.');

			return new int[] { int.Parse(v[0]), int.Parse(v[1]), int.Parse(v[2]), int.Parse(v[3]) };
		}

		private bool versionIsNewer(int[] compare, int[] current) {
			if(compare[0] > current[0]) {
				return true;
			} else if(compare[0] == current[0]) {
				// minor
				if(compare[1] > current[1]) {
					return true;
				} else if(compare[1] == current[1]) {
					if(compare[2] > current[2]) {
						return true;
					} else if(compare[2] == current[2]) {
						if(compare[3] > current[3]) {
							return true;
						}
					}
				}
			}

			return false;
		}

		//https://stackoverflow.com/questions/7672774/how-do-i-determine-the-windows-download-folder-path
		private class KnownFolder {
			public static readonly Guid Downloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
		}

		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);

		private static string getDownloadsFolder() {
			string downloads;
			SHGetKnownFolderPath(KnownFolder.Downloads, 0, IntPtr.Zero, out downloads);

			return downloads;
		}

		private void lnkGithubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start(githubLink);
		}
	}
}
