using UnityEngine;
using System;

public static class LocalUser
{
    private const string KEY = "LOCAL_USER_ID";

    /// <summary>
    /// 存在しなければ新規生成し、PlayerPrefs に保存。以降これを authorId として使う。
    /// </summary>
    public static string GetLocalUserId()
    {
        if (!PlayerPrefs.HasKey(KEY))
        {
            string newId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(KEY, newId);
            PlayerPrefs.Save();
        }
        return PlayerPrefs.GetString(KEY);
    }
}