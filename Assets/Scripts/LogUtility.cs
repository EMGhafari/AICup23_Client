using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Utilities
{
    public static class LogUtility
    {
        public class Log
        {
            public int[][] initialize;
            public int[] score;
            public Dictionary<string, Turn> turns;

            public override string ToString()
            {
                string toString = "initialize: \n";
                foreach (int[] pair in initialize)
                {
                    toString += pair[0] + ": " + pair[1] + "\n";
                }
                toString += "\n";
                foreach (KeyValuePair<string, Turn> turn in turns)
                {
                    toString += "***************" + turn.Key + ": " + turn.Value + "\n";
                }

                return toString;
            }
        }

        public class Turn
        {
            public int[] nodes_owner;
            public int[] troop_count;
            public int[][] add_troop;
            public Attack[] attack;
            public Fortify fortify;
            public int[] fort;

            public override string ToString()
            {
                string toString = "\n";
                toString += PrintArray(nodes_owner) + '\n';
                toString += PrintArray(troop_count) + '\n';
                foreach (int[] pair in add_troop)
                {
                    toString += "add: " + PrintArray(pair) + '\n';
                }
                foreach (Attack battle in attack)
                {
                    toString += "attack: " + battle.ToString() + '\n';
                }
                toString += "fortify: " + fortify.ToString();
                return toString;
            }
        }

        public struct Attack
        {
            public int attacker;
            public int target;
            public int new_troop_count_attacker;
            public int new_troop_count_target;
            public int new_target_owner;
            public int new_fort_troop;

            public override string ToString()
            {
                return "Attacker: " + attacker.ToString() + " (new troops: " + new_troop_count_attacker
                    + ")    Target: " + target.ToString() + " (new troops: " + new_troop_count_target + ")  NewOwner: " + new_target_owner;
            }
        }

        public struct Fortify
        {
            public int number_of_troops;
            public int[] path;

            public override string ToString()
            {
                return "number of troops: " + number_of_troops + " path: " + PrintArray(path);
            }
        }

        //for debugging purposes
        static string PrintArray<T>(T[] array)
        {
            string print = "";
            foreach (T item in array)
            {
                print += item.ToString() + ", ";
            }
            return print;
        }


        public static Log Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Log>(json);
        }
    }

}
