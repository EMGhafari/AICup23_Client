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

    public void openFileDialog()
    {
        filePath = StandaloneFileBrowser.OpenFilePanel("choose json file", "",new[] { new ExtensionFilter("Game Log", "txt", "json") }, false);

        if (filePath != null)
        { 

            readContent(filePath[0]);
            //setContent();
        }       

    }


    private void readContent(string _filePath)
    {
        using (StreamReader reader = new StreamReader(_filePath))
        {
            fileContent = reader.ReadToEnd();

            reader.Close();
        }

        Debug.Log(fileContent);
    }

    //private void setContent()
    //{
    //    actionManager.testJSON = fileContent;
    //}
}
