using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class ActionManager : MonoBehaviour , IActionPerformer
{
    [SerializeField][TextArea] string testJSON;
    [SerializeField] MapMaker mapMaker;

    int[] playerFinalScores;
    List<Actions.Action> actionStack;
    int stackIndex = 0;

    List<Planet> planets = new List<Planet>();

    [SerializeField] float gameSpeed = 1;
    [SerializeField] float coolDownTime = 5;

    [Header("Visuals")]
    [SerializeField] ParticleSystem troopsAttackParticle;
    LineRenderer actionLine;    
    [SerializeField] PlayerStyles playerStyles;


    [Header("UI references")]
    [SerializeField] ConsoleUI consoleUI;
    [SerializeField] UIMANAGER mainUI;

    // Start is called before the first frame update
    void Start()
    {
        actionLine = GetComponent<LineRenderer>();
        LoadActions(mapMaker.CreateMap());
        mainUI.Initialize(playerStyles, this, actionStack.Count , 3);
        StartCoroutine(StartFirstPlayback());
    }

    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach(Actions.Action action in actionStack)
            {
                Debug.Log(Actions.Utilities.ArrayToString(action.owners));
            }
        }
    }
    */

    void LoadActions(int planetCount)
    {
        int turnID = 0;
        actionStack = new List<Actions.Action>();

        string logJson = GameManager.Instance == null ? testJSON : GameManager.Instance.getLog();
        LogUtility.Log loadedLog = LogUtility.Deserialize(logJson);


        int[] owners = new int[planetCount];
        int[] counts = new int[planetCount];
        int[] forts = new int[planetCount];
        for(int i = 0; i < planetCount; i++) { owners[i] = -1; }

        foreach(int[] move in loadedLog.initialize)
        {
            Actions.Add action = new Actions.Add(move[1], 1, turnID++, owners, counts, forts, move[0]);
            actionStack.Add(action);
            Actions.Utilities.UpdateMapInfo(owners, counts, action);
        }
        foreach (KeyValuePair<string,LogUtility.Turn> turn in loadedLog.turns)
        {
            //actionStack.Add(new Actions.Update(turn.Value.nodes_owner, turn.Value.troop_count, turnID));
            turn.Value.nodes_owner.CopyTo(owners,0);
            turn.Value.troop_count.CopyTo(counts,0);
            foreach (int[] add in turn.Value.add_troop)
            {
                int? owner = owners[add[0]] == -1 ? turnID % 3 : null;
                Actions.Add action = new Actions.Add(add[0], add[1], turnID, owners, counts, forts, owner);
                actionStack.Add(action);
                Actions.Utilities.UpdateMapInfo(owners, counts, action);
            }
            foreach (LogUtility.Attack attack in turn.Value.attack)
            {
                Actions.Attack action = new Actions.Attack(attack, turnID, owners, counts, forts);
                actionStack.Add(action);
                Actions.Utilities.UpdateMapInfo(owners, counts, action);
                forts[action.target] = action.new_fort_troop;
            }
            if (turn.Value.fortify.number_of_troops != 0)
            {
                Actions.Fortify action = new Actions.Fortify(turn.Value.fortify, turnID, owners, counts, forts);
                actionStack.Add(action);
                Actions.Utilities.UpdateMapInfo(owners, counts, action);
            }

            
            int fortPlanet = -1;
            for (int i = 0; i < turn.Value.fort.Length; i++)
            {
                if (turn.Value.fort[i] != forts[i])
                {
                    if (forts[i] == 0)
                    {
                        fortPlanet = i;
                    }
                }
            }
            turn.Value.fort.CopyTo(forts, 0);
            if (fortPlanet >= 0)
            {
                Actions.Fort action = new Actions.Fort(fortPlanet, turn.Value.fort[fortPlanet], owners, counts, forts, turnID);
                actionStack.Add(action);
            }
           
            turnID++;
        }
        playerFinalScores = loadedLog.score;
    }

    private void OnEnable()
    {
        Planet.OnPlanetCreated += RegisterPlanet;
    }
    void OnDisable()
    {
        Planet.OnPlanetCreated -= RegisterPlanet;
    }

    int currentTurn = 0;
    Coroutine loopCoroutine = null;
    IEnumerator StartFirstPlayback()
    {
        Time.timeScale = 10;
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1;
    }

    IEnumerator RunAction(Actions.Action action)
    {
        currentTurn = action.turnId;
        UpdateMapData(action.owners, action.counts, action.forts);
        UpdateScoreboard(stackIndex);
        action.Perform(this);
        consoleUI.AddLogEntry(action.ToString());
        mainUI.UpdateSliderValue(stackIndex);
        yield return new WaitForSeconds ((1 / gameSpeed) * action.durationMultiplyer * 2);
        stackIndex++;
        
        if(actionStack != null && stackIndex < actionStack.Count)
        {
            loopCoroutine = StartCoroutine (RunAction(actionStack[stackIndex]));
        } else if (stackIndex == actionStack.Count)
        {
            if (playerFinalScores == null || playerFinalScores.Length == 0) yield break;
            mainUI.ShowGameEndScreen(
                "Player " + Array.IndexOf(playerFinalScores, playerFinalScores.Max()) + " Wins! (" + playerFinalScores.Max() + ")",
                GetPlayerScoresMultiline());
        }
    }

    string GetPlayerScoresMultiline()
    {
        string res = "";
        for (int i = 0; i < playerFinalScores.Length; i++)
        {
            res += "Player " + i + ": " + playerFinalScores[i] + "\n";
        }
        return res;
    }


    void UpdateMapData(int[] owners, int[] counts, int[] forts)
    {
        for (int i = 0; i < owners.Length; i++)
        {
            planets[i].SetOwner(owners[i]);
            planets[i].TroopCount = counts[i];
            planets[i].FortCount = forts[i];
        }
    }

    public void RegisterPlanet(Planet planet)
    {
        planets.Add(planet);
    }

    public void PerformUpdate(int[] owners, int[] count)
    {
        foreach (Planet planet in planets)
        {
            planet.OnDeselect();
        }
        ResetActionLine();
    }

    List<GameObject> InstantiatedObjects = new List<GameObject>();
    public void PerformAttack(LogUtility.Attack info)
    {
        foreach (Planet planet in planets)
        {
            planet.OnDeselect();
        }

        bool success = planets[info.attacker].GetOwner() == info.new_target_owner;
        int damageToAttacker = planets[info.attacker].TroopCount - info.new_troop_count_attacker;
        int damageToTarget = success? planets[info.target].TroopCount : planets[info.target].TroopCount - info.new_troop_count_target;
        planets[info.attacker].SpawnText("-" + damageToAttacker);
        planets[info.target].SpawnText("-" + damageToTarget);

        mainUI.AttackManager(true, planets[info.attacker].GetOwner(), planets[info.target].GetOwner(), damageToAttacker, planets[info.target].TroopCount);

        planets[info.target].SetOwner(info.new_target_owner);
        planets[info.attacker].TroopCount = info.new_troop_count_attacker;
        planets[info.target].TroopCount = info.new_troop_count_target;

        if(success) planets[info.target].SpawnText("+" + info.new_troop_count_target);

        Vector3 attachPos = planets[info.attacker].gameObject.transform.position;
        Vector3 targetPos = planets[info.target].gameObject.transform.position;
        Vector3 attackDir = (targetPos - attachPos).normalized;       

        planets[info.attacker].OnSelect();
        planets[info.target].OnSelect();
        Camera.main.GetComponent<CameraController>().SetTarget(attachPos, targetPos);
        ParticleSystem troops = Instantiate(troopsAttackParticle, attachPos + attackDir , Quaternion.LookRotation(attackDir));
        InstantiatedObjects.Add(troops.gameObject);
        SetActionLine(new Vector3[] {attachPos, targetPos}, planets[info.attacker].GetPlanetColor(), planets[info.target].GetPlanetColor());
    }

    public void PerformAdd(int node, int amount, int? owner = null)
    {
        foreach(Planet planet in planets)
        {
            planet.OnDeselect();
        }
        if (owner != null) planets[node].SetOwner(owner??-1);
        planets[node].OnSelect();
        planets[node].SpawnText("+" + amount);
        planets[node].TroopCount += amount;
        mainUI.ADDManager(true, planets[node].GetOwner(), amount, node);

        ResetActionLine();
    }

    public void PerformFortify(LogUtility.Fortify info)
    {
        foreach (Planet planet in planets)
        {
            planet.OnDeselect();
        }
        foreach (int i in info.path)
        {
            planets[i].OnSelect();
        }
        planets[info.path[0]].TroopCount -= info.number_of_troops;
        planets[info.path.Length - 1].TroopCount += info.number_of_troops;
        mainUI.FORTIFYManager(true, planets[info.path[0]].GetOwner(), info.number_of_troops, info.path[0], info.path[info.path.Length - 1]);
        ResetActionLine();
    }

    public void PerformFort(int node, int amount)
    {
        foreach (Planet planet in planets)
        {
            planet.OnDeselect();
        }
        planets[node].FortCount = amount;
        mainUI.FORTManager(true, planets[node].GetOwner(), amount, node);
        ResetActionLine();
    }


    public void SetPlayheadPos(int stackIndex)
    {
        if (actionStack == null || stackIndex >= actionStack.Count) return;
        
        if(loopCoroutine != null) StopCoroutine(loopCoroutine);
        ClearInstantiatedObjects();


        this.stackIndex = stackIndex;
        loopCoroutine = StartCoroutine(RunAction(actionStack[stackIndex]));
        if(Time.timeScale > 0) mainUI.Play_Pause = true;
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Play()
    {
        if(loopCoroutine == null) { loopCoroutine = StartCoroutine(RunAction(actionStack[stackIndex])); }
        Time.timeScale = 1;
    }

    public void ForwardButton()
    {
        SetPlayheadPos(stackIndex + 1);
    }

    public void BackwardButton()
    {
        SetPlayheadPos(stackIndex + 1);
    }

    void SetActionLine(Vector3[] positions, Color color1 = default, Color color2 = default)
    {
        actionLine.positionCount = positions.Length;
        actionLine.SetPositions(positions);
        actionLine.startColor = color1;
        actionLine.endColor = color2;
    }
    void ResetActionLine()
    {
        actionLine.positionCount = 0;
    }
    void ClearInstantiatedObjects()
    {
        foreach (GameObject obj in InstantiatedObjects)
        {
            if(obj != null) Destroy(obj);
        }
        InstantiatedObjects.Clear();
    }

    public enum GameSpeed
    {
        quarter,
        half,
        normal,
        twice,
        quadruple,
        octuple
    }
    public void SetGameSpeed(GameSpeed speed)
    {
        switch(speed)
        {
            case GameSpeed.quarter:
                gameSpeed = 0.25f;
                break;
            case GameSpeed.half:
                gameSpeed = 0.5f;
                break;
            case GameSpeed.normal:
                gameSpeed = 1f;
                break;
            case GameSpeed.twice:
                gameSpeed = 2;
                break;
            case GameSpeed.quadruple:
                gameSpeed = 4;
                break;
            case GameSpeed.octuple:
                gameSpeed = 8;
                break;
        }
    }
    public void UpdateScoreboard(int index)
    {
        int[] playerPlanets = new int[4];
        int[] playerTroops = new int[4];
        for (int i = 0; i < actionStack[index].owners.Length; i++)
        {
            if (actionStack[index].owners[i] < 0) continue;
            playerPlanets[actionStack[index].owners[i]]++;
            playerTroops[actionStack[index].owners[i]] += actionStack[index].counts[i];
        }

        mainUI.UpdateScoreBoard(currentTurn, playerPlanets, playerTroops);
    }

    public struct TurnInfo
    {
        public int number;
        public TurnInfo(int num)
        {
            number = num;
        }
    }
    public TurnInfo getTurnInfo()
    {
        int turnNum = actionStack[stackIndex].turnId;
        return new TurnInfo(turnNum);
    }
}