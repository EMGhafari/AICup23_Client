using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

public class ActionManager : MonoBehaviour , IActionPerformer
{
    [SerializeField][TextArea] string testJSON;

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
        LoadActions();
        mainUI.Initialize(playerStyles, this, actionStack.Count , 3);
        StartCoroutine(StartFirstPlayback());
    }

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

    void LoadActions()
    {
        int turnID = 0;
        actionStack = new List<Actions.Action>();

        string logJson = GameManager.Instance == null ? testJSON : GameManager.Instance.getLog();
        LogUtility.Log loadedLog = LogUtility.Deserialize(logJson);

        int planetCount = loadedLog.turns.Values.ToArray()[0].nodes_owner.Length;

        int[] owners = new int[planetCount];
        int[] counts = new int[planetCount];
        for(int i = 0; i < planetCount; i++) { owners[i] = -1; }

        foreach(int[] move in loadedLog.initialize)
        {
            Actions.Add action = new Actions.Add(move[1], 1, turnID++, owners, counts, move[0]);
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
                Actions.Add action = new Actions.Add(add[0], add[1], turnID, owners, counts);
                actionStack.Add(action);
                Actions.Utilities.UpdateMapInfo(owners, counts, action);
            }
            foreach (LogUtility.Attack attack in turn.Value.attack)
            {
                Actions.Attack action = new Actions.Attack(attack, turnID, owners, counts);
                actionStack.Add(action);
                Actions.Utilities.UpdateMapInfo(owners, counts, action);
            }
            if (turn.Value.fortify.number_of_troops != 0)
            {
                Actions.Fortify action = new Actions.Fortify(turn.Value.fortify, turnID, owners, counts);
                actionStack.Add(action);
                Actions.Utilities.UpdateMapInfo(owners, counts, action);
            }
            turnID++;
        }
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
        UpdateMapData(action.owners, action.counts);
        action.Perform(this);
        currentTurn = action.turnId;
        consoleUI.AddLogEntry(action.ToString());
        mainUI.UpdateSliderValue(stackIndex);
        yield return new WaitForSeconds ((1 / gameSpeed) * action.durationMultiplyer * 2);
        stackIndex++;
        
        if(actionStack != null && stackIndex < actionStack.Count)
        {
            loopCoroutine = StartCoroutine (RunAction(actionStack[stackIndex]));
        }
    }

    void UpdateMapData(int[] owners, int[] counts)
    {
        for (int i = 0; i < owners.Length; i++)
        {
            planets[i].SetOwner(owners[i]);
            planets[i].TroopCount = counts[i];
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
        ResetActionLine();
    }

    public void SetPlayheadPos(int stackIndex)
    {
        if (actionStack == null || stackIndex >= actionStack.Count) return;
        
        if(loopCoroutine != null) StopCoroutine(loopCoroutine);
        ClearInstantiatedObjects();


        this.stackIndex = stackIndex;
        loopCoroutine = StartCoroutine(RunAction(actionStack[stackIndex]));
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