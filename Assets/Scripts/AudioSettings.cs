using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Audio Settings")]
public class AudioSettings : ScriptableObject
{
    public float MasterVolume { get { return _MasterVolume; } set { _MasterVolume = value; OnSettingChange(this); } }
    [SerializeField] float _MasterVolume;
    public float MusicVolume { get { return _MusicVolume; } set { _MusicVolume = value; OnSettingChange(this); } }
    [SerializeField] float _MusicVolume;
    public float SFXVolume { get { return _SFXVolume; } set { _SFXVolume = value; OnSettingChange(this); } }
    [SerializeField] float _SFXVolume;

    public delegate void SettingChange(AudioSettings settings);
    public event SettingChange OnSettingChange;
}
