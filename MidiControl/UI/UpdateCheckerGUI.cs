using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

#if DEBUG
using System.Diagnostics;
#endif

using Newtonsoft.Json;

namespace MidiControl {
	public partial class UpdateCheckerGUI : Form {
		private readonly Size normalSize = new Size(406, 345);
		private readonly Size progressSize = new Size(406, 105);

		private bool updateFound = false;

		public UpdateCheckerGUI() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MIDIControlGUI));
			InitializeComponent();
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

			this.Size = this.progressSize;
			this.FormBorderStyle = FormBorderStyle.Fixed3D;

			this.Text = "Updates - Current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion;

			grpBox.Text = "Checking for updates...";

			btnDownload.Enabled = false;

			ThemeSupport.ThemeOtherWindow((new OptionsManagment()).options.Theme, this);
		}

		private void UpdateCheckerGUI_Load(object sender, EventArgs e) {
			this.Size = this.progressSize;
			this.Show();

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

						DisplayResults("Unable to check for updates", "An error occurred while checking for updates:\r\n\r\n" + ex.Message, "Updates", 1);

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
							DisplayResults("Error parsing received data", "An error occurred while checking for updates:\r\n\r\n" + ex.Message, "Updates", 1);
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
					var releaseVersion = ParseVersionString(data[0].TagName.Replace("v", ""));

					// check major version; update if newer, otherwise match and check deeper
					if(releaseVersion[0] > currentVersion[0]) {
						updateFound = true;
					} else if(releaseVersion[0] == currentVersion[0]) {
						// minor
						if(releaseVersion[1] > currentVersion[1]) {
							updateFound = true;
						} else if(releaseVersion[1] == currentVersion[1]) {
							if(releaseVersion[2] > currentVersion[2]) {
								updateFound = true;
							} else if(releaseVersion[2] == currentVersion[2]) {
								if(releaseVersion[3] > currentVersion[3]) {
									updateFound = true;
								}
							}
						}
					}

					if(updateFound) {
						//grpBox.Text = "An update is available!";
						var releaseNotes = data[0].Name + "\r\n" + data[0].PublishedAt + "\r\n\r\n" + data[0].Body;
						//txtReleaseNotes.Text = data[0].Name + "\r\n" + data[0].PublishedAt + "\r\n\r\n" + data[0].Body;
						var title = "Updates - Current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion;
						//this.Text = "Updates - Current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion;
						//btnDownload.Enabled = true;
						DisplayResults("An update is available!", releaseNotes, title, 0);
					} else {
						//grpBox.Text = "No updates are available at this time.";
						//txtReleaseNotes.Text = "You are currently on the latest version :)";

						//this.Text = "Up to date - Current version: " + Application.ProductVersion + ", OBS " + Program.obsVersion;
						DisplayResults("No updates are available at this time.", "You are already on the latest version :)", "Updates", 1);
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
			this.FormBorderStyle = FormBorderStyle.Sizable;
			progressBar1.Visible = false;

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
	}


}
