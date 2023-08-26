using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    public bool AutoFocus { get { return _AutoFocus; } set { _AutoFocus = value; OnSettingChange(this); } }
    [SerializeField] bool _AutoFocus;
    public float Sensitiviy { get { return _Sensitivity; } set { _Sensitivity = value; OnSettingChange(this); } }
    [SerializeField] float _Sensitivity;
    public float MovementSpeed { get { return _MovementSpeed; } set { _MovementSpeed = value; OnSettingChange(this); } }
    [SerializeField] float _MovementSpeed;

    public delegate void SettingChange(CameraSettings settings);
    public event SettingChange OnSettingChange;
}
