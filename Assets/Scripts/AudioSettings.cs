using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Audio Settings")]
public class AudioSettings : ScriptableObject
{
    public float MasterVolume;
    public float MusicVolume;
    public float SFXVolume;
}
