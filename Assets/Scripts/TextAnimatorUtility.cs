using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Animator))]
public class TextAnimatorUtility : MonoBehaviour
{
    [SerializeField] Text textUI;
    [SerializeField] string triggerName;

    private void Start()
    {
        GetComponent<Animator>().SetTrigger(triggerName);
    }

    public void OnUpdateText(string text)
    {
        if (textUI != null)
        textUI.text = text;
    }
}