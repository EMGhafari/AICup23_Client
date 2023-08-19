using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    string logContent;
    string mapContent;

    private void Awake()
    {
        if(Instance  == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public string getLog()
    {
        return logContent;
    }

    public void setLog(string log)
    {
        logContent = log;
    }

    public string getMap()
    {
        return mapContent;
    }
    public void setMap(string map) { 
        mapContent = map;   
    }


    public bool readyToPlay()
    {
        return mapContent.Length > 0 && logContent.Length > 0;
    }
}
