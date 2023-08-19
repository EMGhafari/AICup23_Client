using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public static class MapUtility
    {
        public class Map
        {
            public int number_of_nodes;
            public int number_of_edges;
            public int[][] list_of_edges;
            public int[] special_node;
        }
       
        public static Map Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Map>(json);
        }

        public static ForceDirectedGraph.DataStructure.Network ConvertToGraph(Map map)
        {
            List<ForceDirectedGraph.DataStructure.Node> nodes = new List<ForceDirectedGraph.DataStructure.Node>();
            for (int i = 0; i < map.number_of_nodes; i++)
            {
                ForceDirectedGraph.DataStructure.Node currentNode = new ForceDirectedGraph.DataStructure.Node(System.Guid.NewGuid(), i.ToString(), Color.black, map.special_node.Contains(i)? 2.5f:1);
                nodes.Add(currentNode);
            }
            List<ForceDirectedGraph.DataStructure.Link> edges = new List<ForceDirectedGraph.DataStructure.Link>();
            for (int i =0; i < map.number_of_edges; i ++)
            {
                ForceDirectedGraph.DataStructure.Link currentEdge = 
                    new ForceDirectedGraph.DataStructure.Link(nodes[map.list_of_edges[i][0]].Id, nodes[map.list_of_edges[i][1]].Id);
                edges.Add(currentEdge);
            }

            return new ForceDirectedGraph.DataStructure.Network(nodes,edges);
        }
    }
}
