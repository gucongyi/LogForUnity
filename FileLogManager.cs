using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SEUNITY;

public class FileLogManager : UnitySingleton<FileLogManager>
{
    private log4net.ILog LogManager = null;
    private const int CountSaveFiles=15;
    private string fileFolderName = string.Empty;
    private List<FileInfo> listFiles=new List<FileInfo>();

    void Awake()
    {
        if (string.IsNullOrEmpty(fileFolderName))
            fileFolderName = Path.Combine(Application.persistentDataPath,"LogFile");
        log4net.GlobalContext.Properties["Global:ApplicationLogPath"] = fileFolderName;
        Debug.Log("Global:ApplicationLogPath:" + log4net.GlobalContext.Properties["Global:ApplicationLogPath"]);
        log4net.GlobalContext.Properties["LogFileName"] = "BattleGirl" + System.DateTime.Now.Year + "y" +
                                                      System.DateTime.Now.Month + "mo" +
                                                      System.DateTime.Now.Day + "d" +
                                                      System.DateTime.Now.Hour + "h" +
                                                      System.DateTime.Now.Minute + "mi" +
                                                      System.DateTime.Now.Second+"s";
        Debug.Log("LogFileName:" + log4net.GlobalContext.Properties["LogFileName"]);
        var fileName = Path.Combine(Application.streamingAssetsPath, "log4net.config");
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(fileName));
        LogInfo<FileLogManager>("Game Start==========================================");
        DeleteExpireFile();
    }

    //保存CountSaveFiles个文件，超出把时间最早的删除
    public void DeleteExpireFile()
    {
        try
        {
            DirectoryInfo TheFolder = new DirectoryInfo(fileFolderName);
            Debug.Assert(TheFolder.Exists);
            if (TheFolder.Exists)
            {
                FileInfo[] files = TheFolder.GetFiles();
                if (files.Length > 0)
                {
                    for (int idx = 0; idx < files.Length; idx++)
                    {
                        listFiles.Add(files[idx]);
                    }
                    listFiles.Sort((fileA, fileB) =>
                    {
                        if (fileA.CreationTime < fileB.CreationTime)
                        {
                            return -1;
                        }
                        return 1;
                    });
                    if (listFiles.Count > CountSaveFiles)
                    {
                        int willDeleteFileCount = listFiles.Count - CountSaveFiles;
                        for (int idx = 0; idx < willDeleteFileCount; idx++)
                        {
                            File.Delete(listFiles[idx].FullName);
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
        }
        
    }

    public void LogDebug<T>(object message)
    {
        LogManager = log4net.LogManager.GetLogger(typeof(T));
        Debug.Assert(LogManager!=null);
        if (LogManager!=null)
        {
            LogManager.Info(message);
        }
    }
    public void LogInfo<T>(object message)
    {
        LogManager = log4net.LogManager.GetLogger(typeof(T));
        Debug.Assert(LogManager != null);
        if (LogManager != null)
        {
            LogManager.Info(message);
        }
    }
    public void LogError<T>(object message)
    {
        LogManager = log4net.LogManager.GetLogger(typeof(T));
        Debug.Assert(LogManager != null);
        if (LogManager != null)
        {
            LogManager.Error(message);
        }
    }
    public void LogWarn<T>(object message)
    {
        LogManager = log4net.LogManager.GetLogger(typeof(T));
        Debug.Assert(LogManager != null);
        if (LogManager != null)
        {
            LogManager.Warn(message);
        }
    }

}
