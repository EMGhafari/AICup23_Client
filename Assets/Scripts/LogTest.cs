using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utilities;

public class LogTest : MonoBehaviour
{

    [SerializeField][TextArea]  string testJSON;

    // Start is called before the first frame update
    void Start()
    {
        LogUtility.Log log = LogUtility.Deserialize(testJSON);
        Debug.Log(log);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}   