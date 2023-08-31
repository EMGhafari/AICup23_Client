using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ConsoleUI : MonoBehaviour
{
    [SerializeField] GameObject UIObjectPrefab;
    [SerializeField] RectTransform contentBox;
    public void AddLogEntry(string text)
    {
        var uiObj = Instantiate(UIObjectPrefab, contentBox);
        uiObj.GetComponentInChildren<Text>().text = text;
        GetComponent<ScrollRect>().verticalScrollbar.value = 0;
        //StartCoroutine(currectPos());
    }

    /*
    IEnumerator currectPos()
    {
        yield return null;
        GetComponent<ScrollRect>().verticalScrollbar.value = 0;
    }
    */
}
