using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GlobalSaveData
{
    //全局已解锁的CG
    public List<string> unlockedCGIds = new List<string>();
}


public static class GlobalSaveManager
{
    private const string FileName = "GlobalSave.json";


    private static string GetPath()
    {
        return Path.Combine(
            Application.persistentDataPath,
            FileName
        );
    }


    /// <summary>
    /// 读取全局存档
    /// </summary>
    public static GlobalSaveData Load()
    {
        string path = GetPath();

        GlobalSaveData data;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<GlobalSaveData>(json);
        }
        else
        {
            data = new GlobalSaveData();
        }

        //兼容旧版本或字段不存在
        if (data == null)
            data = new GlobalSaveData();

        if (data.unlockedCGIds == null)
            data.unlockedCGIds = new List<string>();

        return data;
    }


    /// <summary>
    /// 保存全局数据
    /// </summary>
    private static void Save(GlobalSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(), json);
    }


    /// <summary>
    /// 解锁CG
    /// 已经解锁过则不会重复保存
    /// </summary>
    public static void UnlockCG(string cgId)
    {
        if (string.IsNullOrEmpty(cgId))
        {
            Debug.LogWarning("无法解锁CG：cgId为空");
            return;
        }

        GlobalSaveData data = Load();

        if (data.unlockedCGIds.Contains(cgId))
            return;

        data.unlockedCGIds.Add(cgId);
        Save(data);

        Debug.Log("全局CG解锁：" + cgId);
    }


    /// <summary>
    /// 查询CG是否已经解锁
    /// </summary>
    public static bool IsCGUnlocked(string cgId)
    {
        if (string.IsNullOrEmpty(cgId))
            return false;

        GlobalSaveData data = Load();

        return data.unlockedCGIds.Contains(cgId);
    }


    /// <summary>
    /// 清空全部CG解锁
    /// 只有“清除全部游戏数据”时才调用
    /// </summary>
    public static void DeleteGlobalSave()
    {
        string path = GetPath();

        if (File.Exists(path))
            File.Delete(path);
    }
}