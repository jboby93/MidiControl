using System;
#if DEBUG
using System.Diagnostics;
using System.IO;
#endif
using System.Windows.Forms;

namespace MidiControl
{
    static class Program
    {

		public static bool StartedToTray = false;

		public static readonly int obsVersion = 28;
		public static readonly string urlUpdates = "https://api.github.com/repos/Etuldan/MidiControl/releases";

		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		[STAThread]
        static void Main()
        {
#if DEBUG
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string ConfFolder = Path.Combine(folder, "MIDIControl");
            Debug.Listeners.Add(new TextWriterTraceListener(Path.Combine(ConfFolder, Path.GetFileName("debug.log"))));
            Debug.AutoFlush = true;
#endif


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			var mainForm = new MIDIControlGUI();
			var options = (new OptionsManagment()).options;

			StartedToTray = options.StartToTray;

			if(options.StartToTray) {
				Application.Run();
			} else {
				Application.Run(mainForm);
			}
        }
    }
}
