using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.SaveLoadSystem
{
    public static class SaveLoad
    {
        public static List<object> objects = new List<object>();
        public const string c_nameFile = "savedGames.txt";

        public static void Save()
        {
            //SaveLoad.savedGames.Add(Game.current);
            BinaryFormatter bf = new BinaryFormatter();
            //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located 
            FileStream file = File.Create(Application.persistentDataPath + "/" + c_nameFile); //you can call it anything you want 
            bf.Serialize(file, objects);
            file.Close();
        }

        public static void Load()
        {
            if (File.Exists(Application.persistentDataPath + "/" + c_nameFile))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + c_nameFile, FileMode.Open);
                objects = (List<object>)bf.Deserialize(file);
                file.Close();
            }
        }

        public static bool HasSaveGame()
        {
            return File.Exists(Application.persistentDataPath + "/" + c_nameFile);
        }
    }
}
