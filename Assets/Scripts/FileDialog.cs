using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SFB;

public class FileDialog : MonoBehaviour
{

    public string fileContent { get; private set; }
    private string[] filePath;

    //GameObject jsonHandlerSource;
    //ActionManager actionManager;

    void Start()
    {
        //jsonHandlerSource = GameObject.Find("Main Camera");
        //actionManager = jsonHandlerSource.GetComponent<ActionManager>();
    }

    public void OpenMap()
    {
        filePath = StandaloneFileBrowser.OpenFilePanel("choose json file", "", new[] { new ExtensionFilter("Game Log", "txt", "json") }, false);

        if (filePath != null)
        {
            string f = "";
            readContent(filePath[0], ref f);
            GameManager.Instance.setMap(f);
        }
    }
   

    public void OpenLog()
    {
        filePath = StandaloneFileBrowser.OpenFilePanel("choose json file", "", new[] { new ExtensionFilter("Game Log", "txt", "json") }, false);

        if (filePath != null)
        {
            string f = "";
            readContent(filePath[0], ref f);
            GameManager.Instance.setLog(f);
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

    //private void setContent()
    //{
    //    actionManager.testJSON = fileContent;
    //}
}
