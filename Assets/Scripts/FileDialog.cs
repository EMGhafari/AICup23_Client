using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SFB;

public class FileDialog : MonoBehaviour
{
    //public string fileContent { get; private set; }
    //private string[] filePath;

    public void OpenMap()
    {
        string[] filePath = StandaloneFileBrowser.OpenFilePanel("choose json file", "", new[] { new ExtensionFilter("Map Log", "txt", "json") }, false);

        if (filePath != null)
        {
            string f = "";
            readContent(filePath[0], ref f);
            GameManager.Instance.setMap(f);
        } else
        {
            GameManager.Instance.Debug("Invalid file or path", 3);
        }
    }
   

    public void OpenLog()
    {
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
