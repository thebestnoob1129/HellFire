using UnityEngine;
using System;
using System.IO;

namespace CFS
{
    public class SaveGameDataWriter 
    {
        public string saveDataDirectory;
        public string saveFileName;

        // CHECK FILE FIRST ( MAX 10 SLOTS )
        public bool CheckIfFileExists(string fileName)
        {
            return File.Exists(Path.Combine(saveDataDirectory, fileName));
        }
        // DELETE SAVE FILE
        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectory, saveFileName));
        }

        // CREATE NEW SAVE FILE
        public void CreateNewFile(CharacterSaveData saveData)
        {
            // MAKE A PATH TO SAVE FILE
            var savePath = Path.Combine(saveDataDirectory,saveFileName);
            saveData.fileName = saveFileName;
            try
            {
                // CREATE DIRECTORY
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("CREATING SAVE FILE AT: " + savePath);

                // SERIALIZE DATA TO FILE / JSON
                var dataToStore = JsonUtility.ToJson(saveData, true);

                using var stream = new FileStream(savePath, FileMode.Create);
                using var writer = new StreamWriter(stream);
                writer.Write(dataToStore);
            }
            catch (Exception e)
            {
                Debug.LogError("ERROR WHILE CREATING SAVE DATA FILE: " + savePath + "/n" + e);
            }

        }

        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData saveData = null;

            var loadPath = Path.Combine(saveDataDirectory,saveFileName);

            if (File.Exists(loadPath))
            {
                var dataToLoad = "";

                try
                {
                    using (var stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using var reader = new StreamReader(stream);
                        dataToLoad = reader.ReadToEnd();
                    }

                    // DESERIALIZE FROM JSON TO UNITY
                    saveData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
            
            return saveData;
        }

    }
}