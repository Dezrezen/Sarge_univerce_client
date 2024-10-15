using System.Runtime.Serialization.Formatters.Binary;
using Common.Model;
using UnityEngine;

namespace Save
{
    public static class SaveLoad
    {
        private const string SaveFolderName = "Save";
        private const string PlayerDataFileName = "playerData.srg";
        private const string BuildingsDataFileName = "buildingsData.srg";

        public static void SavePlayerData(PlayerDataModel dataModel)
        {
            SaveData<PlayerDataModel>(dataModel, PlayerDataFileName);
        }

        public static PlayerDataModel LoadPlayerData()
        {
            return LoadData<PlayerDataModel>(PlayerDataFileName);
        }
        
        private static void SaveData<T>(T data, string fileName)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(fileName, json);
        }

        private static T LoadData<T>(string fileName) where T : class, new()
        {
            var data = new T();
            if (!PlayerPrefs.HasKey(fileName))
                return data;

            var json = PlayerPrefs.GetString(fileName);
            data = JsonUtility.FromJson<T>(json);
            return data;
        }
    }
}