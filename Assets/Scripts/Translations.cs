using UnityEngine;
using System.Collections;
using MiniJSON;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Translations : MonoBehaviour
{
    private static Dictionary<string, object> data;

    public static void Load(string language)
    {
        string jsonStr = File.ReadAllText(Application.dataPath + language, Encoding.UTF8);

        data = Json.Deserialize(jsonStr) as Dictionary<string, object>;
    }

    public static string Get(string key)
    {
        return (string)data[key];
    }
}