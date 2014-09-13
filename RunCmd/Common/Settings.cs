using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;



namespace RunCmd.Common
{
    [Serializable]
    public sealed class Settings
    {
        private static volatile Settings _instance;
        private static object _syncRoot = new object();
        private string _ExePath = Utility.ExePathDefault;
        private string _SavedCommandsPath=Utility.SavedCommandsDefaultPath;

        [IgnoreDataMember]
        [XmlIgnore]
        public bool LoadStatus
        {
            get;
            private set;
        }

        [IgnoreDataMember]
        [XmlIgnore]
        public bool SaveStatus
        {
            get;
            private set;
        }

        private static string ConfigPath
        {
            get
            {
                return Utility.ConfigDefaultPath;
            }
        }



        public static Settings Instance
        {
            get
            {
                if (Settings._instance == null)
                {
                    lock (Settings._syncRoot)
                    {
                        if (Settings._instance == null)
                        {
                            Settings._instance = Settings.Load();
                        }
                    }
                }
                return Settings._instance;
            }
        }


        public string ExePath
        {
            get { return _ExePath; }
            set { _ExePath = value; }
        }


        public string SavedCommandsPath
        {
            get { return _SavedCommandsPath; }
            set { _SavedCommandsPath = value; }
        }


        private static Settings Load()
        {
            if (System.IO.File.Exists(Settings.ConfigPath))
            {
                try
                {
                    Settings settings = SerializationUtil.DeSerializeObject<Settings>(Settings.ConfigPath); //jsonSerializer.DeserializeFromReader(streamReader);
                    settings.LoadStatus = true;
                    settings.SaveStatus = true;
                    lock (Settings._syncRoot)
                    {
                        Settings._instance = settings;
                    }
                }
                catch
                {
                    throw;
                }
            }
            if (Settings._instance == null)
            {
                lock (Settings._syncRoot)
                {
                    Settings._instance = new Settings();
                }
            }
            return Settings._instance;
        }
        public void Save()
        {
            try
            {
                SerializationUtil.SerializeObject<Settings>(this, Settings.ConfigPath);
                this.SaveStatus = true;
            }
            catch
            {
                this.SaveStatus = false;
                throw;
            }
        }

        public void Reset()
        {
            try
            {
                Utility.DeleteFile(Settings.ConfigPath);
                lock (Settings._syncRoot)
                {
                    Settings._instance = null;
                }
                Load();
                Save();
                this.SaveStatus = true;
            }
            catch
            {
                this.SaveStatus = false;
                throw;
            }
        }
    }
}
