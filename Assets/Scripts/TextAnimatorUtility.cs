using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class TextAnimatorUtility : MonoBehaviour
{
    [SerializeField] Text textUI;
    public void OnUpdateText(string text)
    {
        if (textUI != null)
        textUI.text = text;
    }
}