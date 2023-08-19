using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ActionManager : MonoBehaviour , IActionPerformer
{
    [SerializeField][TextArea] string testJSON;

    List<Actions.Action> actionStack;
    int stackIndex = 0;

    List<Planet> planets;

    [SerializeField] float gameSpeed = 1;
    [SerializeField] float coolDownTime = 5;

    [Header("Visuals")]
    [SerializeField] ParticleSystem troopsAttackParticle;
    LineRenderer actionLine;    
    [SerializeField] PlayerStyles playerStyles;


    [Header("UI references")]
    [SerializeField] ConsoleUI consoleUI;

    // Start is called before the first frame update
    void Start()
    {
        planets = new List<Planet>();
        actionLine = GetComponent<LineRenderer>();
        LoadActions();
        StartCoroutine(StartFirstPlayback());
    }

    void LoadActions()
    {
        int turnID = 0;
        actionStack = new List<Actions.Action>();

        string logJson = GameManager.Instance == null ? testJSON : GameManager.Instance.getLog();
        LogUtility.Log loadedLog = LogUtility.Deserialize(logJson);
        foreach(int[] move in loadedLog.initialize)
        {
            actionStack.Add(new Actions.Add(move[1], 1, move[0], turnID++));
        }
        foreach (KeyValuePair<string,LogUtility.Turn> turn in loadedLog.turns)
        {
            actionStack.Add(new Actions.Update(turn.Value.nodes_owner, turn.Value.troop_count, turnID));
            foreach (int[] add in turn.Value.add_troop)
            {
                actionStack.Add(new Actions.Add(add[0], add[1], turnId: turnID));
            }
            foreach (LogUtility.Attack attack in turn.Value.attack)
            {
                actionStack.Add(new Actions.Attack(attack, turn.Value.nodes_owner , turnID));
            }
            if(turn.Value.fortify.number_of_troops != 0) actionStack.Add(new Actions.Fortify(turn.Value.fortify, turnID));
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

    Coroutine loopCoroutine = null;
    IEnumerator StartFirstPlayback()
    {
        Time.timeScale = 10;
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1;
        yield return new WaitForSeconds(coolDownTime);
        if (actionStack != null)
        {
            loopCoroutine = StartCoroutine(RunAction(actionStack[0]));
        }
        else
        {
            Debug.Log("No action to perform");
        }
    }

    IEnumerator RunAction(Actions.Action action)
    {
        action.Perform(this);
        consoleUI.AddLogEntry(action.ToString());
        yield return new WaitForSeconds ((1 / gameSpeed) * action.durationMultiplyer);
        stackIndex++;
        
        if(actionStack != null && stackIndex < actionStack.Count)
        {
            loopCoroutine = StartCoroutine (RunAction(actionStack[stackIndex]));
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

    public void PerformAttack(LogUtility.Attack info)
    {
        foreach (Planet planet in planets)
        {
            planet.OnDeselect();
        }
        planets[info.attacker].OnSelect();
        planets[info.target].OnSelect();


        Vector3 attachPos = planets[info.attacker].gameObject.transform.position;
        Vector3 targetPos = planets[info.target].gameObject.transform.position;

        Vector3 attackDir = (targetPos - attachPos).normalized;

        Camera.main.GetComponent<CameraController>().SetTarget(attachPos, targetPos);

        ParticleSystem troops = Instantiate(troopsAttackParticle, attachPos + attackDir , Quaternion.LookRotation(attackDir));
        SetActionLine(new Vector3[] {attachPos, targetPos}, planets[info.attacker].getPlanetColor(), planets[info.target].getPlanetColor());
    }

    public void PerformAdd(int node, int amount, int? owner = null)
    {
        foreach(Planet planet in planets)
        {
            planet.OnDeselect();
        }
        if (owner != null) planets[node].SetOwner(owner??-1, playerStyles);
        planets[node].OnSelect();
        planets[node].SpawnText("+" + amount);
        //Camera.main.GetComponent<CameraController>().SetTarget(planets[node].gameObject.transform.position);
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
        //Camera.main.GetComponent<CameraController>().SetTarget(planets[info.path[info.path.Length-1]].gameObject.transform.position);
        ResetActionLine();
    }

    public void SetPlayheadPos(int stackIndex)
    {
        if (actionStack == null || stackIndex >= actionStack.Count) return;
        
        StopCoroutine(loopCoroutine);
        this.stackIndex = stackIndex;
        loopCoroutine = StartCoroutine(RunAction(actionStack[stackIndex]));
    }

    public void Pause()
    {
        StopCoroutine(loopCoroutine);
    }

    void ResetActionLine()
    {
        actionLine.positionCount = 0;
    }

    void SetActionLine(Vector3[] positions, Color color1 = default, Color color2 = default)
    {
        actionLine.positionCount = positions.Length;
        actionLine.SetPositions(positions);
        actionLine.startColor = color1;
        actionLine.endColor = color2;
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