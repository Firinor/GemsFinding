using System;
using UnityEngine;

public static class SaveLoadSystem<T>
{
    private static string key => nameof(T);
    public static T Load(T Default = default)
    {
        if (!PlayerPrefs.HasKey(key))
            return Default;
        string value = PlayerPrefs.GetString(key, string.Empty);
        if (String.IsNullOrEmpty(value))
            return Default;
        return JsonUtility.FromJson<T>(value);
    }
    
    public static void Save(T data)
    {
        string value = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, value);
    }
}