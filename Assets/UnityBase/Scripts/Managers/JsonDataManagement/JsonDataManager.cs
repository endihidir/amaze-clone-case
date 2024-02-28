using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.Serialization;
using UnityBase.Service;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace UnityBase.Manager
{
    public class JsonDataManager : IJsonDataService, IAppPresenterDataService
    {
        private const string DirectoryName = "SaveData";

#if UNITY_EDITOR
        private static string DirectoryPath => $"{Application.dataPath}/{DirectoryName}";
#else
        private static string DirectoryPath => $"{Application.persistentDataPath}/{DirectoryName}";
#endif
        public DataFormat DataFormat { get; set; }
        public void Initialize() { }
        public void Start() { }
        
        public bool Save<T>(string key, T data)
        {
            EnsureDirectoryExists();

            var filePath = GetFilePath(key);

            try
            {
                var jsonData = SerializationUtility.SerializeValue(data, DataFormat);
                
                File.WriteAllBytes(filePath, jsonData);

#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                
                return false;
            }
        }

        public T Load<T>(string key, T defaultData = default, bool autoSaveDefaultData = true)
        {
            EnsureDirectoryExists();

            var filePath = GetFilePath(key);

            if (!File.Exists(filePath))
            {
                if (defaultData is not null && autoSaveDefaultData)
                {
                    Save(key, defaultData);
                }
                
                return defaultData;
            }

            var jsonData = File.ReadAllBytes(filePath);
                
            var data = SerializationUtility.DeserializeValue<T>(jsonData, DataFormat);
            
            return data;
        }

        public async UniTask<bool> SaveAsync<T>(string key, T data)
        {
            EnsureDirectoryExists();

            var filePath = GetFilePath(key);
            
            try
            {
                var jsonData = SerializationUtility.SerializeValue(data, DataFormat);
                
                await File.WriteAllBytesAsync(filePath, jsonData);
                
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
                
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                
                return false;
            }
        }

        public async UniTask<T> LoadAsync<T>(string key, T defaultData = default, bool autoSaveDefaultData = true)
        {
            EnsureDirectoryExists();

            var filePath = GetFilePath(key);

            if (!File.Exists(filePath))
            {
                if (defaultData is not null && autoSaveDefaultData)
                {
                    await SaveAsync(key, defaultData);
                }

                return defaultData;
            }

            try
            {
                var jsonData = await File.ReadAllBytesAsync(filePath);
                
                var data = SerializationUtility.DeserializeValue<T>(jsonData, DataFormat);
                
                return data;

            }
            catch (Exception e)
            {
                Debug.Log(e);
                
                return defaultData;
            }
        }
        
        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(DirectoryPath)) 
                Directory.CreateDirectory(DirectoryPath);
        }

        public static void ClearAllSaveLoadData()
        {
            var files = Directory.GetFiles(DirectoryPath).Select(Path.GetFileName).ToArray();

            foreach (string key in files)
            {
                var filePath = $"{DirectoryPath}/{key}";
                
                File.Delete(filePath);
            }
            
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        private string GetFilePath(string key) => Path.Combine(DirectoryPath, $"{key}.json");
        public void Dispose() { }
    }
}