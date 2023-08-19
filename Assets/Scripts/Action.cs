using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utilities;
using static Utilities.LogUtility;

namespace Actions
{
    [System.Serializable]
    public abstract class Action
    {
        public enum Type
        {
            attack,
            fortify,
            add,
            update,
        }

        public Action(int turnId = 0)
        {
            this.turnId = turnId;
        }

        public int turnId;

        public float durationMultiplyer;

        public Type type;

        public abstract void Perform(IActionPerformer performer);


        protected const string divider = " || ";

        public override string ToString()
        {
            return turnId + ": " + type.ToString();
        }
    }

    public class Attack : Action
    {
        public int attacker;
        public int target;
        public int new_troop_count_attacker;
        public int new_troop_count_target;
        public int new_target_owner;

        public Attack(LogUtility.Attack attack, int turnId = 0) : base(turnId) 
        {
            type = Type.attack;
            durationMultiplyer = 1;
            attacker = attack.attacker;
            target = attack.target;
            new_troop_count_attacker = attack.new_troop_count_attacker;
            new_troop_count_target = attack.new_troop_count_target;
            new_target_owner = attack.new_target_owner;
        }
        public override void Perform(IActionPerformer performer)
        {
            Debug.Log ("Attacking from " + attacker + " to " + target);
            LogUtility.Attack info = new LogUtility.Attack
            {
                attacker = attacker,
                target = target,
                new_target_owner = new_target_owner,
                new_troop_count_attacker = new_troop_count_attacker,
                new_troop_count_target = new_target_owner,
            };
            performer.PerformAttack(info);
        }

        public override string ToString()
        {
            string result = attacker == new_target_owner ? "Success" : "Failure";
            string output = "Attacking from " + attacker + " to " + target + 
                "\nResult: " + result + divider + "new attacker troops: " + new_troop_count_attacker
                + divider + "new target troops: " + new_troop_count_target + divider + "target owner: " + new_target_owner;
            return output;
        }
    }

    public class Fortify : Action
    {
        public int number_of_troops;
        public int[] path;
        public Fortify(LogUtility.Fortify fortify, int turnId = 0) : base(turnId)
        {
            type = Type.fortify;
            durationMultiplyer = 0.75f;
            number_of_troops = fortify.number_of_troops;
            path = fortify.path;
        }
        public override void Perform(IActionPerformer performer)
        {
            Debug.Log("Fortifying " + number_of_troops + " troops");
            LogUtility.Fortify info = new LogUtility.Fortify
            {
                path = path,
                number_of_troops = number_of_troops,
            };
            performer.PerformFortify(info);
        }
        public override string ToString()
        {
            string output = "Fortifying " + number_of_troops + " troops" + divider + "Path: " + Utilities.ArrayToString(path);
            return output;
        }
    }

    public class Add:Action
    {
        public Add(int node, int amount, int? owner = null, int turnId = 0) : base(turnId)
        {
            type = Type.add;
            durationMultiplyer = 0.2f;
            this.node = node;
            this.amount = amount;
            this.owner = owner;
        }
        public int node;
        public int amount;
        public int? owner;
        public override void Perform(IActionPerformer performer)
        {
            Debug.Log("Adding " +  amount + " soldiers to " +  node);
            performer.PerformAdd(node, amount, owner);
        }
        public override string ToString()
        {
            string output = "Adding " + amount + " troops to " + node;
            return output;
        }
    }

    public class Update : Action
    {
        public int[] nodes_owner;
        public int[] troop_count;
        public Update(int[] owners, int[] count, int turnId = 0) : base(turnId)
        {
            type = Type.update;
            durationMultiplyer = 0.1f;
            nodes_owner = owners;
            troop_count = count;
        }
        public override void Perform(IActionPerformer performer)
        {
            Debug.Log("Updating...");
            performer.PerformUpdate(nodes_owner, troop_count);
        }
        public override string ToString()
        {
            string output = "Updating...";
            return output;
        }
    }


    public static class Utilities
    {
        public static string ArrayToString<T>(T[] array)
        {
            string result = "[";
            for (int i = 0; i < array.Length; i++)
            {
                result += array[i].ToString();
                if(i < array.Length - 1) result += ", ";
            }
            result += "]";
            return result;
        }
    }
}