using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayGame()
    {
        if (GameManager.Instance.readyToPlay())
        {
            SceneManager.LoadScene(1);
        } else
        {
            GameManager.Instance.Debug("Invalid map or log",3);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
