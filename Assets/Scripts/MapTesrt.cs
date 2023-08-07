using ForceDirectedGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class MapTesrt : MonoBehaviour
{

    [SerializeField][TextArea] string testJson;
    [SerializeField] GraphManager3D graphManager;

    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
    }

    void CreateMap()
    {
        MapUtility.Map map = MapUtility.Deserialize(testJson);
        graphManager.Initialize(MapUtility.ConvertToGraph(map));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
