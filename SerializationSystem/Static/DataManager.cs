using System.IO;

using Newtonsoft.Json;

using UnityEngine;

namespace Logic.SerializationSystem.Static
{
    public static class DataManager
    {
        private static readonly string Datapath = Application.dataPath + "/Saves/";
        private static readonly string FileExtension = ".json";
        
        public static T LoadData<T>(string filepath, string customFolder = "") where T : new()
        {
            if (customFolder != "" && !Directory.Exists(Datapath + customFolder + "/")) 
                Directory.CreateDirectory(Datapath + customFolder + "/");
            
            string dataStream;
            var savefile = Datapath + customFolder + "/" + filepath + FileExtension;

            if (!Directory.Exists(Datapath)) Directory.CreateDirectory(Datapath);

            if (!File.Exists(savefile))
            {
                SaveData(filepath, new T(), customFolder);

                return new T();
            }

            dataStream = File.ReadAllText(savefile);
            var ret = JsonConvert.DeserializeObject<T>(dataStream);

            return ret;
        }
        public static void SaveData<T>(string filepath, T data, string customFolder = "") where T : new()
        {
            if (customFolder != "" && !Directory.Exists(Datapath + customFolder + "/")) 
                Directory.CreateDirectory(Datapath + customFolder + "/");

            var savefile = Datapath + customFolder + "/" + filepath + FileExtension;

            var dataStream = File.CreateText(savefile);
            dataStream.Write(JsonConvert.SerializeObject(data, Formatting.Indented));
            dataStream.Dispose();
            dataStream.Close();
        }
    }
}