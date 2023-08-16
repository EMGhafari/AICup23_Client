using Necromancy.UI;
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

    [SerializeField] ParticleSystem troopsAttackParticle;

    LineRenderer actionLine;

    // Start is called before the first frame update
    void Start()
    {
        planets = new List<Planet>();
        actionLine = GetComponent<LineRenderer>();
        LoadActions();
        if(actionStack != null)
        {
            loopCoroutine = StartCoroutine(RunAction(actionStack[stackIndex]));
        }
        else
        {
            Debug.Log("No action to perform");
        }
    }

    void LoadActions()
    {
        actionStack = new List<Actions.Action>();
        LogUtility.Log loadedLog = LogUtility.Deserialize(testJSON);
        foreach(int[] move in loadedLog.initialize)
        {
            actionStack.Add(new Actions.Add(move[0], move[1]));
        }
        foreach(KeyValuePair<string,LogUtility.Turn> turn in loadedLog.turns)
        {
            actionStack.Add(new Actions.Update(turn.Value.nodes_owner, turn.Value.troop_count));
            foreach (int[] add in turn.Value.add_troop)
            {
                actionStack.Add(new Actions.Add(add[0], add[1]));
            }
            foreach (LogUtility.Attack attack in turn.Value.attack)
            {
                actionStack.Add(new Actions.Attack(attack));
            }
            actionStack.Add(new Actions.Fortify(turn.Value.fortify));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //PrintPlanets();
        }
    }

    private void OnEnable()
    {
        Planet.OnPlanetCreated += AddPlanet;
    }
    void OnDisable()
    {
        Planet.OnPlanetCreated -= AddPlanet;
    }

    Coroutine loopCoroutine = null;
    IEnumerator RunAction(Actions.Action action)
    {
        yield return new WaitForSeconds(1 / gameSpeed);
        action.Perform(this);
        stackIndex++;
        
        if(actionStack != null && stackIndex < actionStack.Count)
        {
            loopCoroutine = StartCoroutine (RunAction(actionStack[stackIndex]));
        }
    }

    //For Debugging
    public void PrintPlanets()
    {
        foreach (Planet planet in planets)
        {
            Debug.Log(planet.getID() + "\n");
        }
    }

    public void AddPlanet(Planet planet)
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
        planets[info.attacker].OnSelect(Planet.SelectionMode.attacker);
        planets[info.target].OnSelect(Planet.SelectionMode.defender);

        Vector3 attachPos = planets[info.attacker].gameObject.transform.position;
        Vector3 targetPos = planets[info.target].gameObject.transform.position;

        Vector3 attackDir = (targetPos - attachPos).normalized;


        Camera.main.GetComponent<CameraController>().SetTarget(attachPos, targetPos);

        ParticleSystem troops = Instantiate(troopsAttackParticle, attachPos + attackDir , Quaternion.LookRotation(attackDir));
        SetActionLine(new Vector3[] {attachPos, targetPos}, Color.red);
    }

    public void PerformAdd(int node, int amount)
    {
        foreach(Planet planet in planets)
        {
            planet.OnDeselect();
        }
        planets[node].OnSelect(Planet.SelectionMode.add);
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
            planets[i].OnSelect(Planet.SelectionMode.add);
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

    void ResetActionLine()
    {
        actionLine.positionCount = 0;
    }

    void SetActionLine(Vector3[] positions, Color color = default)
    {
        actionLine.positionCount = positions.Length;
        actionLine.SetPositions(positions);
    }
}