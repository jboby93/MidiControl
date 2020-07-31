﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MidiControl
{
    public class OptionsManagment
    {
        private readonly string OptionFile;
        public Options options;
        private static OptionsManagment _instance;

        public OptionsManagment()
        {
            _instance = this;

            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string ConfFolder = Path.Combine(folder, "MIDIControl");
            OptionFile = Path.Combine(ConfFolder, Path.GetFileName("options.json"));
            Directory.CreateDirectory(ConfFolder);
            Load();
        }
        public static OptionsManagment GetInstance()
        {
            return _instance;
        }

        private void Load()
        {
            try
            {
                string json = File.ReadAllText(OptionFile);
                options = JsonConvert.DeserializeObject<Options>(json);
                if (options == null)
                {
                    throw new FileNotFoundException();
                }
                if(options.MIDIInterfaces == null)
                {
                    options.MIDIInterfaces = new List<string>();
                }
            }
            catch (FileNotFoundException)
            {
                options = new Options
                {
                    Ip = "127.0.0.1:4444",
                    Password = "password",
                    Autoconnect = false,
                    AutoReconnect = false,
                    MIDIForwardInterface = "",
                    MIDIForwardEnabled = false,
                    MIDIFeedbackEnabled = false,
                    MIDIInterfaces = new List<string>()
                };
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(options);
            File.WriteAllText(OptionFile, json);
        }

        public class Options
        {
            public string Ip { get; set; }
            public string Password { get; set; }
            public bool Autoconnect { get; set; }
            public bool AutoReconnect { get; set; }
            public string MIDIForwardInterface { get; set; }
            public bool MIDIForwardEnabled { get; set; }
            public bool MIDIFeedbackEnabled { get; set; }
            public int NoteNumberStopAllSounds { get; set; }
            public int ChannelStopAllSounds { get; set; }
            public string MidiDeviceStopAllSounds { get; set; }
            public List<string> MIDIInterfaces { get; set; }
        }
    }
}
