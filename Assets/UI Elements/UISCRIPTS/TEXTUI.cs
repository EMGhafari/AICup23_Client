using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEXTUI : MonoBehaviour
{
    public UIMANAGER uimanager;
    [Header("Names")]
    public string Name0 = "ali";
    public string Name1 = "reza";
    public string Name2 = "hassan";
    public string Playerincharge_name;

    [Header("Planet&Troops")]
    public int player0_planes = 1;
    public int player1_planes = 2;
    public int player2_planes = 3;
    public int player0_troops = 5;
    public int player1_troops = 6;
    public int player2_troops = 7;

    [Header("Video Player")]
    public int totalturn = 20;
    public int currentturn;
    public string Currentspeed;
    public bool is_playing;

    [Header("Actions")]
    //{attack event}
    public bool is_attacking;
    public string defender_name;
    public int attacker_troops;
    public int defender_troops;

    //{add troops}
    public bool is_addtroops;
    public int troopsadded;
    public int Planetnum;


    [Header("Setting")]
    [SerializeField] private int master_volume;
    [SerializeField] private int music_volume;
    [SerializeField] private int sfx_volume;
    [SerializeField] private float mouse_sens_get;
    [SerializeField] private float Speed_get;



    // Start is called before the first frame update
    void Start()
    {

        //GIVE START VALUE
        uimanager.TeamNames_Dis[0].text = Name0;
        uimanager.TeamNames_Dis[1].text = Name1;
        uimanager.TeamNames_Dis[2].text = Name2;

        uimanager.Names_Dis[0].text = Name0;
        uimanager.Names_Dis[1].text = Name1;
        uimanager.Names_Dis[2].text = Name2;

        //GIVE TOTAL TURNS
        uimanager.TotalActions_Dis.text = totalturn.ToString();
        uimanager.MainSlider.maxValue = totalturn;
        uimanager.MainSlider.value = 0;
        is_attacking = false;


    }

    // Update is called once per frame
    void Update()
    {
        //UPDATING VOLUME
        master_volume = ((int)uimanager.Master_Volume.value);
        music_volume = ((int)uimanager.Music_Volume.value);
        sfx_volume = ((int)uimanager.SFX_Volume.value);
        mouse_sens_get = (uimanager.Mouse_Sens.value);
        Speed_get = (uimanager.Movment_speed.value);


        is_playing = uimanager.Play_Pause;
        Currentspeed = uimanager.gamespeed;

        // GIVE THEM PLANET AND TROOPS REALTIME
        uimanager.PlanetNumber_Dis[0].text = player0_planes.ToString();
        uimanager.PlanetNumber_Dis[1].text = player1_planes.ToString();
        uimanager.PlanetNumber_Dis[2].text = player2_planes.ToString();

        uimanager.Troops_Dis[0].text = player0_troops.ToString();
        uimanager.Troops_Dis[1].text = player1_troops.ToString();
        uimanager.Troops_Dis[2].text = player2_troops.ToString();

        //GET CURRENT TURN
        currentturn = ((int)uimanager.MainSlider.value);


        //EVENTS {ATTACKING AND ADDTROOPS}
        /*
        if (is_attacking)
        {
            uimanager.AttackManager(is_attacking, Playerincharge_name, defender_name, attacker_troops, defender_troops);

        }
        else { uimanager.AttackManager(is_attacking, Playerincharge_name, defender_name, attacker_troops, defender_troops); }

        if (is_addtroops)
        {
            uimanager.ADDManager(is_addtroops, Playerincharge_name, troopsadded, Planetnum);
        }
        else { uimanager.ADDManager(is_addtroops, Playerincharge_name, troopsadded, Planetnum); }
        */
    }
}
