using ForceDirectedGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class MapMaker : MonoBehaviour
{
    private enum MapMode
    {
        mode2D,
        mode3D,
    }

    [SerializeField] MapMode mode = MapMode.mode3D;

    [SerializeField][TextArea] string testJson;
    [SerializeField] GraphManager3D graphManager;
    [SerializeField] GraphManager graphManager2D;
  
    public int CreateMap()
    {
        string mapJson = GameManager.Instance == null ? testJson : GameManager.Instance.getMap();
        MapUtility.Map map = MapUtility.Deserialize(mapJson);

        switch (mode)
        {
            case MapMode.mode2D:
                graphManager2D.Initialize(MapUtility.ConvertToGraph(map));
                break;
            case MapMode.mode3D:
                graphManager.Initialize(MapUtility.ConvertToGraph(map));
                break;
        }

        return map.number_of_nodes;
    }
}
