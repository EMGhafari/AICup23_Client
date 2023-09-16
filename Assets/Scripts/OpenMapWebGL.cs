using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class OpenMapWebGL : MonoBehaviour, IPointerDownHandler
{

#if UNITY_WEBGL

    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData)
    {
        UploadFile(gameObject.name, "OnFileUpload", ".txt, .json", false);
    }

    // Called from browser
    public void OnFileUpload(string url)
    {
        StartCoroutine(OutputRoutine(url));
    }

    private IEnumerator OutputRoutine(string url)
    {
        var loader = UnityWebRequest.Get(url);
        yield return loader.SendWebRequest();
        Debug.Log(loader.downloadHandler.text);
        GameManager.Instance.setMap(loader.downloadHandler.text);
    }

#else
    public void OnPointerDown(PointerEventData eventData)
    {

    }
#endif
}
