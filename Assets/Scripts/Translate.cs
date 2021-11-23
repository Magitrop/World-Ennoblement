using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translate : MonoBehaviour
{
    public static string _language;
    public bool translated;
    public string startValue;

    public void Awake()
    {
        _language = PlayerPrefs.GetString("lang");
    }

    public static string TranslateText (string text)
    {
        _language = PlayerPrefs.GetString("lang");
        string result;

        Translations.Load("/Translates/" + _language + ".json");
        result = Translations.Get(text);

        return result;
    }
}
