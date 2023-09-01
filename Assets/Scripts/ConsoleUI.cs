using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ConsoleUI : MonoBehaviour
{
    [SerializeField] GameObject UIObjectPrefab;
    [SerializeField] RectTransform contentBox;
    [SerializeField] float lineH;
    public void AddLogEntry(string text)
    {
        int lineCount = text.Split('\n').Length;
        var uiObj = Instantiate(UIObjectPrefab, contentBox);
        uiObj.GetComponentInChildren<Text>().text = text;
        uiObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lineCount * lineH);
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
