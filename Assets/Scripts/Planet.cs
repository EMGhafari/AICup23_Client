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
            return _TroopCount;
        }
        set
        {
            _TroopCount = value;
            UpdateUI();
        }
    }
    int _TroopCount = 0;
    int owner = -1;


    bool towerEnable;
    bool isStrategic;


    [Header("UI")]
    [SerializeField] Text troopCountIndicatorText;
    [SerializeField] Text idIndicatorText;
    [SerializeField] float popUpLifetime = 1f;
    [SerializeField] RectTransform canvasRootObject;

    PlanetStylizer stylizer;

    public static event PlanetDelegate OnPlanetCreated;
    public delegate void PlanetDelegate(Planet planet);


    TextRendererParticleSystemSimple UIparticle;
    // Start is called before the first frame update
    void Start()
    {
        OnPlanetCreated?.Invoke(this);
        stylizer = GetComponent<PlanetStylizer>();  
        UIparticle = GetComponentInChildren<TextRendererParticleSystemSimple>();
        cam = Camera.main;
    }


    Camera cam;
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Renderer>().isVisible)
        {
            canvasRootObject.gameObject.SetActive(true);
            canvasRootObject.position = cam.WorldToScreenPoint(transform.position);
        } else
        {
            canvasRootObject.gameObject.SetActive(false);
        }
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

    public void OnSelect()
    {
        if (owner < 0) return;
        stylizer.SetOutline(1, styles.GetStyle(owner));
    }

    void UpdateUI()
    {
        troopCountIndicatorText.text = TroopCount.ToString();
    }

    public void SpawnText(string text)
    {
        UIparticle.SpawnParticle(transform.position, text, owner >= 0? Color.Lerp(styles.GetStyle(owner).color,Color.white,0.5f) : Color.white);
    }

    public void OnDeselect()
    {
        stylizer.SetOutline(0);
    }

    private void OnMouseDown()
    {
        StartCoroutine(ShowID());
    }

    PlayerStyles styles;
    public void SetOwner(int index, PlayerStyles styles)
    {
        owner = index;
        this.styles = styles;
    }

    public Color getPlanetColor()
    {
        return styles.GetStyle(owner).color;
    }


    IEnumerator ShowID()
    {
        idIndicatorText.transform.parent.gameObject.SetActive(true);
        idIndicatorText.text = id;
        yield return new WaitForSeconds(popUpLifetime);
        idIndicatorText.transform.parent.gameObject.SetActive(false);
    }
}
