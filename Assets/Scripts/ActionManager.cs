using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ActionManager : MonoBehaviour , IActionPerformer
{
    [SerializeField][TextArea] string testJSON;

    Queue<Actions.Action> actionStack;
    List<Planet> planets;

    [SerializeField] float gameSpeed = 1;


    // Start is called before the first frame update
    void Start()
    {
        planets = new List<Planet>();
        LoadActions();
        if(actionStack != null)
        {
            StartCoroutine(RunAction(actionStack.Dequeue()));
        }
        else
        {
            Debug.Log("No action to perform");
        }
    }

    void LoadActions()
    {
        actionStack = new Queue<Actions.Action>();
        LogUtility.Log loadedLog = LogUtility.Deserialize(testJSON);
        foreach(int[] move in loadedLog.initialize)
        {
            actionStack.Enqueue(new Actions.Add(move[0], move[1]));
        }
        foreach(KeyValuePair<string,LogUtility.Turn> turn in loadedLog.turns)
        {
            actionStack.Enqueue(new Actions.Update(turn.Value.nodes_owner, turn.Value.troop_count));
            foreach (int[] add in turn.Value.add_troop)
            {
                actionStack.Enqueue(new Actions.Add(add[0], add[1]));
            }
            foreach (LogUtility.Attack attack in turn.Value.attack)
            {
                actionStack.Enqueue(new Actions.Attack(attack));
            }
            actionStack.Enqueue(new Actions.Fortify(turn.Value.fortify));
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


    IEnumerator RunAction(Actions.Action action)
    {
        yield return new WaitForSeconds(1 / gameSpeed);
        action.Perform(this);
        
        if(actionStack != null && actionStack.Count > 0)
        {
            StartCoroutine (RunAction(actionStack.Dequeue()));
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
    }

    public void PerformAttack(LogUtility.Attack info)
    {
        foreach (Planet planet in planets)
        {
            planet.OnDeselect();
        }
        planets[info.attacker].OnSelect(Planet.SelectionMode.attacker);
        planets[info.target].OnSelect(Planet.SelectionMode.defender);
    }

    public void PerformAdd(int node, int amount)
    {
        foreach(Planet planet in planets)
        {
            planet.OnDeselect();
        }
        planets[node].OnSelect(Planet.SelectionMode.add);
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
    }
}
