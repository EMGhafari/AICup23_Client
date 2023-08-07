using ForceDirectedGraph.DataStructure;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling;
using UnityEngine;

public class Planet : MonoBehaviour, INode
{
    string id;
    string[] neigbers;
    int troopCount;
    string troopOwner;
    bool towerEnable;
    bool isStrategic;

    PlanetStylizer stylizer;

    public static event PlanetDelegate OnPlanetCreated;
    public delegate void PlanetDelegate(Planet planet);
    // Start is called before the first frame update
    void Start()
    {
        OnPlanetCreated?.Invoke(this);
        stylizer = GetComponent<PlanetStylizer>();    
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override string ToString()
    {
        return id;
    }

    public string getID()
    {
        return (id);
    }

    public void setID(string id)
    {
        this.id = id;
    }


    public enum SelectionMode
    {
        attacker,
        defender,
        add
    }

    public void OnSelect(SelectionMode mode)
    {
        stylizer.SetOutline(1, mode);
    }

    public void OnDeselect()
    {
        stylizer.SetOutline(0);
    }
}
