using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveScript {

    private Save save;
    protected string savePath;

    public SaveScript()
    {
        this.savePath = Application.persistentDataPath + "/save.dat";
        this.save = new Save();
        this.LoadDataFromDisk();
    }

    public void SaveDataToDisk()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, save);
        file.Close();
    }

    public void LoadDataFromDisk()
    {
        if(File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            this.save = (Save)bf.Deserialize(file);
            file.Close();
        }
    }
}

[System.Serializable]
public class Save
{
    public float[][][] weights;
}
