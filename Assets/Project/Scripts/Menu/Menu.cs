using Assets.Scripts.Economy.Data;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DataSaver
{
    public class Menu : MonoBehaviour
    {
        public void QuitGame()
        {
            Application.Quit();
        }

        public void SaveGameProgress()
        {
            using (var fileStream = new FileStream(Directory.GetCurrentDirectory() + "gameSave.bin", FileMode.OpenOrCreate, FileAccess.Write))
            using (var ms = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(ms, new DataForSaver());
                ms.CopyTo(fileStream);
            }
        }

        public void LoadGameProgress()
        {
            DataForSaver data;
            using (var fileStream = new FileStream(Directory.GetCurrentDirectory() + "gameSave.bin", FileMode.Open, FileAccess.Read))
            {
                data = (DataForSaver)(new BinaryFormatter()).Deserialize(fileStream);
            }

            PlayersData.Leonardo = data.Leonardo;
            PlayersData.Raphael = data.Raphael;
            PlayersData.Donatello = data.Donatello;
            PlayersData.Michelangelo = data.Michelangelo;
        }
    }

    [Serializable]
    public class DataForSaver
    {
        public DataForSaver()
        {
            Leonardo = PlayersData.Leonardo;
            Raphael = PlayersData.Raphael;
            Donatello = PlayersData.Donatello;
            Michelangelo = PlayersData.Michelangelo;
        }
        public PlayerDataFieldsInfo Leonardo;

        public PlayerDataFieldsInfo Raphael;

        public PlayerDataFieldsInfo Donatello;

        public PlayerDataFieldsInfo Michelangelo;
    }
}
