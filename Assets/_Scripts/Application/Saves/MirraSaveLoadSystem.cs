using System;
using UnityEngine;

public static class MirraSaveLoadSystem<T>
{
    /*public static T Load(string key, T Default = default)
    {
        if (!MirraSDK.Data.HasKey(key))
            return Default;
        string value = MirraSDK.Data.GetString(key, string.Empty);
        if (String.IsNullOrEmpty(value))
            return Default;
        return JsonUtility.FromJson<T>(value);
    }
    
    public static void Save(string key, T data)
    {
        string value = JsonUtility.ToJson(data);
        MirraSDK.Data.SetString(key, value);
        MirraSDK.Data.Save();
    }*/
}