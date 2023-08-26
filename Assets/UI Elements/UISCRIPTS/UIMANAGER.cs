using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMANAGER : MonoBehaviour
{
    [Header("HINTS")]
    public Transform show;
    public Transform hide;
    public GameObject masker;
    public GameObject playerui;
    public GameObject actionui;
    public GameObject leaderboardui;
    public GameObject videoui;

    [Header("PLAYER")]
    public TextMeshProUGUI[] TeamNames_Dis;
    public Image[] PlayerFlags;
   

    [Header("Leaderboard")]
    public TextMeshProUGUI[] Names_Dis;
    public TextMeshProUGUI[] PlanetNumber_Dis;
    public TextMeshProUGUI[] Troops_Dis;

    [Header("Video Player")]
    public TextMeshProUGUI TotalActions_Dis;
    public TextMeshProUGUI CurrentAction_Dis;
    public Slider MainSlider;
    public bool Play_Pause;
    public string gamespeed;


    [Header("Setting")]
    public CameraSettings cameraSettings;
    public AudioSettings audioSettings;
    [Space(10)]
    public GameObject setting;

    public Slider Master_Volume;
    public TextMeshProUGUI Master_Volume_text;

    public Slider Music_Volume;
    public TextMeshProUGUI Music_Volume_text;

    public Slider SFX_Volume;
    public TextMeshProUGUI SFX_Volume_text;

    public Slider Mouse_Sens;
    public TextMeshProUGUI Mouse_Sens_text;

    public Slider Movment_speed;
    public TextMeshProUGUI Movment_speed_text;

    public bool Autozoom;


    [Header("Actions")]
    [SerializeField] private GameObject Attack_eventUI;
    [SerializeField] private GameObject AddTroops_eventUI;

    public GameObject attack_display;
    public GameObject addtroops_display;
    public TextMeshProUGUI Attacker_name;
    public TextMeshProUGUI Defender_name;
    public TextMeshProUGUI Attacker_Troops;
    public TextMeshProUGUI Defender_Troops;

    //{add troops}
    public TextMeshProUGUI addtoopsint;
    public TextMeshProUGUI playeradding;
    public TextMeshProUGUI Planetadding;


    ActionManager actionManager;
    PlayerStyles styles;
    void Start()
    {
        masker.SetActive(false);
        CurrentAction_Dis.text = "0";
        Play_Pause = true;
        Master_Volume.value = audioSettings.MasterVolume;
        Music_Volume.value = audioSettings.MusicVolume;
        SFX_Volume.value = audioSettings.SFXVolume;
        Mouse_Sens.value = cameraSettings.Sensitiviy;
        Movment_speed.value = cameraSettings.MovementSpeed;
        Autozoom = cameraSettings.AutoFocus;
    }

    public void Initialize(PlayerStyles styles, ActionManager manager, int actionCount , int playerCount = 3)
    {
        MainSlider.maxValue = actionCount;
        TotalActions_Dis.text = actionCount.ToString();
        actionManager = manager;
        this.styles = styles;
        for(int i = 0; i < playerCount; i++)
        {
            PlayerFlags[i].color = styles.GetStyle(i).color;
            PlayerFlags[i].transform.parent.gameObject.SetActive(true);
            TeamNames_Dis[i].text = "Player " + i;
            Names_Dis[i].gameObject.SetActive(true);
            Names_Dis[i].text = "Player " + i;
            PlanetNumber_Dis[i].text = 0.ToString();
            Troops_Dis[i].text = 0.ToString();
        }
    }

    public void autozoom_f()
    {
        Autozoom = !Autozoom;
        cameraSettings.AutoFocus = Autozoom;
    }

    //{HINTS MANAGER}
    public void hint1()
    {
        playerui.transform.SetParent(hide);
        leaderboardui.transform.SetParent(hide);
        actionui.transform.SetParent(hide);
        videoui.transform.SetParent(show);
        masker.SetActive(true);
    }
    public void hint2()
    {
        playerui.transform.SetParent(show);
        leaderboardui.transform.SetParent(hide);
        actionui.transform.SetParent(hide);
        videoui.transform.SetParent(hide);
        masker.SetActive(true);
    }
    public void hint3()
    {
        playerui.transform.SetParent(hide);
        leaderboardui.transform.SetParent(show);
        actionui.transform.SetParent(show);
        videoui.transform.SetParent(hide);
        masker.SetActive(true);
    }


    //{ATTACK MANAGER}
    public void AttackManager(bool isattack, int incharge, int defender, int attacktroops, int defendtroops)
    {
        RESETACTIONManager();

        //attacker color
        Attacker_name.text = "Player " + incharge;
        Attacker_Troops.text = attacktroops.ToString();
       
        Attacker_name.color = styles.GetStyle(incharge).color;
        Attacker_Troops.color = styles.GetStyle(incharge).color;
       

        //defender color
        Defender_name.text = "Player " + defender;
        Defender_Troops.text = defendtroops.ToString();
       
        Defender_name.color = styles.GetStyle(defender).color;
        Defender_Troops.color = styles.GetStyle(defender).color;


        if (isattack)
        {
            attack_display.SetActive(true);
            Attack_eventUI.SetActive(true);
        }
        else
        {
            attack_display.SetActive(false);
            Attack_eventUI.SetActive(false);
        }
    }


    //{ADD TROOPS MANAGER}
    public void ADDManager(bool isadding, int incharge, int troops, int planetnumber)
    {
        RESETACTIONManager();

        playeradding.text = "Player " + incharge;
        addtoopsint.text = troops.ToString();
        Planetadding.text = planetnumber.ToString();
        
        playeradding.color = styles.GetStyle(incharge).color;
        addtoopsint.color = styles.GetStyle(incharge).color;
        Planetadding.color = styles.GetStyle(incharge).color;

        if (isadding)
        {
            addtroops_display.SetActive(true);
            AddTroops_eventUI.SetActive(true);
        }
        else
        {
            addtroops_display.SetActive(false);
            AddTroops_eventUI.SetActive(false);
        }
    }

    void RESETACTIONManager()
    {
        addtroops_display.SetActive(false);
        AddTroops_eventUI.SetActive(false);
        attack_display.SetActive(false);
        Attack_eventUI.SetActive(false);
    }

    //////////////////////////////////
    //Video PLAYER
    //{Speed Changer}
    public void Dropdownchanged(int value)
    {
        actionManager.SetGameSpeed((ActionManager.GameSpeed)value);
    }

    //{Slider Show Vlaue}
    public void Sliderchange(float value)
    {
        CurrentAction_Dis.text = value.ToString();
        actionManager.SetPlayheadPos((int)value);
    }

    public void UpdateSliderValue(int value)
    {
        MainSlider.SetValueWithoutNotify(value);
    }


    //Setting
    public void settingicon()
    {
        Transform parent = setting.transform.parent;
        int childCount = parent.childCount;
        setting.transform.SetSiblingIndex(childCount - 1);
    }
    public void master_slider(float value)
    {
        Master_Volume_text.text = value.ToString();
    }
    public void music_slider(float value)
    {
        Music_Volume_text.text = value.ToString();
    }
    public void sfx_slider(float value)
    {
        SFX_Volume_text.text = value.ToString();
    }


    public void mouse_sens_slider(float value)
    {
        Mouse_Sens_text.text = value.ToString("0.0");
        cameraSettings.Sensitiviy = value;
    }
    public void movment_speed_slider(float value)
    {
        Movment_speed_text.text = value.ToString("0.0");
        cameraSettings.MovementSpeed = value;
    }

    //Buttons
    public void PlayandPause()
    {
        Play_Pause = !Play_Pause;
        switch(Play_Pause)
        {
            case true:
                actionManager.Pause();
                break;
            case false:
                actionManager.Play();
                break;
        }
    }
    public void Forward()
    {
        GetComponent<UIMANAGER>().MainSlider.value++;
    }
    public void Backward()
    {
        GetComponent<UIMANAGER>().MainSlider.value--;
    }
}