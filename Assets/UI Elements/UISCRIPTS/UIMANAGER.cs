using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
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
    public TextMeshProUGUI TurnNumber;

    [Header("Video Player")]
    public GameObject PlayButton;
    public GameObject PauseButton;
    public TextMeshProUGUI TotalActions_Dis;
    public TextMeshProUGUI CurrentAction_Dis;
    public Slider MainSlider;
    public bool Play_Pause
    {
        get
        {
            return _Play_Pause;
        }
        set
        {
            _Play_Pause = value;
            PlayButton.SetActive(!value);
            PauseButton.SetActive(value);
        }
    }
    bool _Play_Pause;
    public string gamespeed;


    [Header("Setting")]
    public CameraSettings cameraSettings;
    public AudioSettings audioSettings;
    public AudioMixer masterMixer;
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

    public GameObject autozoom_On;
    public GameObject autozoom_Off;

    public bool Autozoom
    {
        get
        {
            return _Autozoom;
        } set
        {
            _Autozoom = value;
            autozoom_On.SetActive(value);
            autozoom_Off.SetActive(!value);
        }
    }
    bool _Autozoom;

    [Header("Actions")]
    [SerializeField] private GameObject Attack_eventUI;
    [SerializeField] private GameObject AddTroops_eventUI;
    [SerializeField] private GameObject Fortify_eventUI;
    [SerializeField] private GameObject Fort_eventUI;

    public GameObject attack_display;
    public GameObject addtroops_display;
    public GameObject fortify_display;
    public GameObject fort_display;

    public TextMeshProUGUI Attacker_name;
    public TextMeshProUGUI Defender_name;
    public TextMeshProUGUI Attacker_Troops;
    public TextMeshProUGUI Defender_Troops;
    [Header("Add")]
    //{add troops}
    public TextMeshProUGUI addtoopsint;
    public TextMeshProUGUI playeradding;
    public TextMeshProUGUI Planetadding;
    [Header("Fortify")]
    public TextMeshProUGUI playerFortifying;
    public TextMeshProUGUI FortifyFrom;
    public TextMeshProUGUI FortifyTo;
    public TextMeshProUGUI FortifyAmount;
    [Header("Fort")]
    public TextMeshProUGUI fortAmountInt;
    public TextMeshProUGUI playeraddingFort;
    public TextMeshProUGUI PlanetaddingFort;


    [Space(50)]
    [Header("Game End")]
    public GameObject gameEndPanel;
    public Text gameEndMainText;
    public Text gameEndSubText;

    ActionManager actionManager;
    PlayerStyles styles;
    void Start()
    {
        masker.SetActive(false);
        CurrentAction_Dis.text = "0";
        Play_Pause = false;
        LoadSettings();
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

    void LoadSettings()
    {
        LoadAudioSettings(audioSettings);
        Autozoom = cameraSettings.AutoFocus;
        Master_Volume.SetValueWithoutNotify(audioSettings.MasterVolume);
        Master_Volume_text.text = audioSettings.MasterVolume.ToString();
        Music_Volume.SetValueWithoutNotify (audioSettings.MusicVolume);
        Music_Volume_text.text = audioSettings.MusicVolume.ToString();
        SFX_Volume.SetValueWithoutNotify(audioSettings.SFXVolume);
        SFX_Volume_text.text = audioSettings.SFXVolume.ToString();
        Mouse_Sens.SetValueWithoutNotify(cameraSettings.Sensitiviy);
        Mouse_Sens_text.text = cameraSettings.Sensitiviy.ToString("0.0");
        Movment_speed.SetValueWithoutNotify(cameraSettings.MovementSpeed);
        Movment_speed_text.text = cameraSettings.MovementSpeed.ToString("0.0");
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

    public void FORTIFYManager(bool isFortifying, int incharge, int troops, int from, int to)
    {
        RESETACTIONManager();

        playerFortifying.text = "Player " + incharge;
        FortifyAmount.text = troops.ToString();
        FortifyFrom.text = from.ToString();
        FortifyTo.text = to.ToString();


        playerFortifying.color = styles.GetStyle(incharge).color;
        FortifyAmount.color = styles.GetStyle(incharge).color;
        FortifyFrom.color = styles.GetStyle(incharge).color;
        FortifyTo.color = styles.GetStyle(incharge).color;

        if (isFortifying)
        {
            fortify_display.SetActive(true);
            Fortify_eventUI.SetActive(true);
        }
        else
        {
            fortify_display.SetActive(false);
            Fortify_eventUI.SetActive(false);
        }
    }

    public void FORTManager(bool isForting, int incharge, int troops, int planetnumber)
    {
        RESETACTIONManager();

        playeraddingFort.text = "Player " + incharge;
        fortAmountInt.text = troops.ToString();
        PlanetaddingFort.text = planetnumber.ToString();

        playeraddingFort.color = styles.GetStyle(incharge).color;
        fortAmountInt.color = styles.GetStyle(incharge).color;
        PlanetaddingFort.color = styles.GetStyle(incharge).color;

        if (isForting)
        {
            fort_display.SetActive(true);
            Fort_eventUI.SetActive(true);
        }
        else
        {
            fort_display.SetActive(false);
            Fort_eventUI.SetActive(false);
        }
    }

    void RESETACTIONManager()
    {
        addtroops_display.SetActive(false);
        AddTroops_eventUI.SetActive(false);
        attack_display.SetActive(false);
        Attack_eventUI.SetActive(false);
        fortify_display.SetActive(false);
        Fortify_eventUI.SetActive(false);
        fort_display.SetActive(false);
        Fort_eventUI.SetActive(false);
    }



    public void UpdateScoreBoard(int turn, int[] planets,int[] troops)
    {
        TurnNumber.text = turn.ToString();
        for (int i = 0; i < PlanetNumber_Dis.Length; i++)
        {
            PlanetNumber_Dis[i].text = planets[i].ToString();
            Troops_Dis[i].text = troops[i].ToString();
        }
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
        CurrentAction_Dis.text = value.ToString();
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
        audioSettings.MasterVolume = value;
    }
    public void music_slider(float value)
    {
        Music_Volume_text.text = value.ToString();
        audioSettings.MusicVolume = value;
    }
    public void sfx_slider(float value)
    {
        SFX_Volume_text.text = value.ToString();
        audioSettings.SFXVolume = value;
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



    public void ShowGameEndScreen(string main, string sub)
    {
        gameEndPanel.SetActive(true);
        gameEndMainText.text = main;
        gameEndSubText.text = sub;
    }


    //Buttons
    public void PlayandPause()
    {
        Play_Pause = !Play_Pause;
        switch(Play_Pause)
        {
            case true:
                actionManager.Play();
                break;
            case false:
                actionManager.Pause();
                break;
        }
    }
    public void Forward()
    {
        MainSlider.value++;
    }
    public void Backward()
    {
        MainSlider.value--;
    }

    private void OnEnable()
    {
        audioSettings.OnSettingChange += LoadAudioSettings;
    }

    private void OnDisable()
    {
        audioSettings.OnSettingChange -= LoadAudioSettings;
    }

    public void LoadAudioSettings(AudioSettings settings)
    {
        masterMixer.SetFloat("MasterVolume", Mathf.Lerp(-40,10, settings.MasterVolume / 100));
        masterMixer.SetFloat("MusicVolume",  Mathf.Lerp(-40, 10, settings.MusicVolume / 100));
        masterMixer.SetFloat("SFXVolume",    Mathf.Lerp(-40, 10, settings.SFXVolume / 100));
    }


    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayandPause();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Backward();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Forward();
        }
    }
}