using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SFB;
using UnityEngine.Networking;
using System.Runtime.InteropServices;


public class FileDialog : MonoBehaviour
{
    //public string fileContent { get; private set; }
    //private string[] filePath;

    static bool loaded = false;
    
    public void Start()
    {
        if (!loaded)
        {
            CheckExternalPath();
            loaded = true;
        }
    }

    void CheckExternalPath()
    {
#if !UNITY_WEBGL
        string mapPath = Application.dataPath + "/Maps";
        string logPath = Application.dataPath + "/Logs";

        if (Directory.Exists(mapPath))
        {
            var allpaths = Directory.GetFiles(mapPath);
            if (allpaths != null && allpaths.Length > 0) mapPath = allpaths[0];
            else mapPath = null;
        } else
        {
            mapPath = null;
        }

        if (Directory.Exists(logPath))
        {
            var allpaths = Directory.GetFiles(logPath);
            if (allpaths != null && allpaths.Length > 0) logPath = allpaths[0];
            else logPath = null;
        } else
        {
            logPath = null;
        }

        if (mapPath != null)
        {
            string map = "";
            readContent(mapPath, ref map);
            GameManager.Instance.setMap(map);
        } else
        {
            GameManager.Instance.Debug("Maps folder path doesn't contain a file or is invalid" , 3);
        }

        if (logPath != null)
        {
            string log = "";
            readContent(logPath, ref log);
            GameManager.Instance.setLog(log);
        }
        else
        {
            GameManager.Instance.Debug("Logs folder path doesn't contain a file or is invalid", 3);
        }
#endif
    }

    public void OpenMap()
    {
#if !UNITY_WEBGL
        string[] filePath = StandaloneFileBrowser.OpenFilePanel("choose json file", "", new[] { new ExtensionFilter("Map Log", "txt", "json") }, false);

        if (filePath != null)
        {
            string f = "";
            readContent(filePath[0], ref f);
            GameManager.Instance.setMap(f);
        }
        else
        {
            GameManager.Instance.Debug("Invalid file or path", 3);
        }
#endif
    }


    public void OpenLog()
    {
#if !UNITY_WEBGL
        string[] filePath = StandaloneFileBrowser.OpenFilePanel("choose json file", "", new[] { new ExtensionFilter("Game Log", "txt", "json") }, false);

        if (filePath != null)
        {
            string f = "";
            readContent(filePath[0], ref f);
            GameManager.Instance.setLog(f);
        }
        else
        {
            GameManager.Instance.Debug("Invalid file or path", 3);
        }
#endif
    }

    private void readContent(string _filePath, ref string content)
    {
        using (StreamReader reader = new StreamReader(_filePath))
        {
            content = reader.ReadToEnd();

            reader.Close();
        }

        Debug.Log(content);
    }
}
