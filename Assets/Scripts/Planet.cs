using ForceDirectedGraph.DataStructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planet : MonoBehaviour, INode
{
    string id;

    public int TroopCount
    {
        get
        {
            return troopCount;
        }
        set
        {
            troopCount = value;
            UpdateUI();
        }
    }
    int troopCount = 0;


    string troopOwner;
    bool towerEnable;
    bool isStrategic;

    [SerializeField] Text troopCountIndicatorText;
    [SerializeField] RectTransform canvasRootObject;

    PlanetStylizer stylizer;

    public static event PlanetDelegate OnPlanetCreated;
    public delegate void PlanetDelegate(Planet planet);
    // Start is called before the first frame update
    void Start()
    {
        OnPlanetCreated?.Invoke(this);
        stylizer = GetComponent<PlanetStylizer>();    
        cam = Camera.main;
    }


    Camera cam;
    // Update is called once per frame
    void Update()
    {
        canvasRootObject.position = cam.WorldToScreenPoint(transform.position);
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

    void UpdateUI()
    {
        troopCountIndicatorText.text = TroopCount.ToString();
    }


    public void OnDeselect()
    {
        stylizer.SetOutline(0);
    }
}
