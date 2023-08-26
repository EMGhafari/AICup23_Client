using ForceDirectedGraph.DataStructure;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
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


    bool IsFort { 
        get
        {
            return _IsFort;
        }
        set
        {
            _IsFort = value;
            shield.gameObject.SetActive(value);
            shield.startColor = styles.GetStyle(owner).color;
        }
    }
    bool _IsFort = false;
    bool isStrategic;

    [SerializeField] ParticleSystem shield;


    [Header("UI")]
    [SerializeField] Text troopCountIndicatorText;
    [SerializeField] Text idIndicatorText;
    [SerializeField] float popUpLifetime = 1f;
    [SerializeField] RectTransform canvasRootObject;


    [Header("Player Styles")]
    [SerializeField] PlayerStyles styles;



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

    public string GetID()
    {
        return (id);
    }

    public void SetID(string id)
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
        if (owner < 0)
        {
            stylizer.SetOutline(0);
            return;
        }
        stylizer.SetOutline(1, styles.GetStyle(owner));
    }

    void UpdateUI()
    {
        troopCountIndicatorText.text = TroopCount.ToString();
        troopCountIndicatorText.color = owner >= 0? styles.GetStyle(owner).color : new Color(0,0,0,0);
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

    public void SetOwner(int index)
    {
        owner = index;
    }

    public int GetOwner()
    {
        return owner;
    }

    public Color GetPlanetColor()
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
