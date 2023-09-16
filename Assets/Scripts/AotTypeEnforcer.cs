using Newtonsoft.Json.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class AotTypeEnforcer : MonoBehaviour
{
#if UNITY_WEBGL
    public void Awake()
    {
        AotHelper.EnsureList<int>();
        AotHelper.EnsureList<string>();
        AotHelper.EnsureList<List<int>>();
        AotHelper.EnsureDictionary<string, LogUtility.Turn>();
        AotHelper.EnsureList<LogUtility.Turn>();
        AotHelper.EnsureList<LogUtility.Attack>();
        AotHelper.EnsureList<LogUtility.Fortify>();
    }
#endif
}
