using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MidiControl {
	public partial class UpdateCheckerGUI : Form {
		public UpdateCheckerGUI() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MIDIControlGUI));
			InitializeComponent();
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));


		}
	}
}
