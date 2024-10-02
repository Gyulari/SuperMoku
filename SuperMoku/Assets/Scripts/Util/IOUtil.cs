using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class IOUtil
{
    public static void ExportDataByJson<T>(T data, string path)
    {
        string fPath = Path.Combine(Application.dataPath, path);
        string jsonString = JsonConvert.SerializeObject(data);
        File.WriteAllText(fPath, jsonString);
    }

    public static T ImportDataByJson<T>(string path)
    {
        string fPath = Path.Combine(Application.dataPath, path);
        string jsonString = File.ReadAllText(fPath);
        T data = JsonConvert.DeserializeObject<T>(jsonString);

        return data;
    }

    public static void ExportDataListByJson<T>(List<T> list, string path)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };

        string fPath = Path.Combine(Application.dataPath, path);
        string jsonString = JsonConvert.SerializeObject(list, settings);
        File.WriteAllText(fPath, jsonString);
    }

    public static List<T> ImportDataListByJson<T>(string path)
    {
        List<T> data = new List<T>();

        string fPath = Path.Combine(Application.dataPath, path);
        string jsonString = File.ReadAllText(fPath);
        data = JsonConvert.DeserializeObject<List<T>>(jsonString);

        return data;
    }
}
