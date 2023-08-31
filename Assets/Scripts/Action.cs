using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Actions
{
    [System.Serializable]
    public abstract class Action
    {
        public int[] owners;
        public int[] counts;
        public int[] forts;

        public Action(int turnId, int[] owners, int[] counts, int[] forts)
        {
            this.turnId = turnId;
            this.owners = (int[])owners.Clone();
            this.counts = (int[])counts.Clone();
            this.forts = (int[])forts.Clone();
        }

        public enum Type
        {
            attack,
            fortify,
            add,
            //update,
        }

        public int turnId;
        public float durationMultiplyer;
        public Type type;
        protected const string divider = "\t\t";


        public abstract void Perform(IActionPerformer performer);

        public override string ToString()
        {
            return turnId + ": ";
        }
    }

    public class Attack : Action
    {
        public int attacker;
        public int target;
        public int new_troop_count_attacker;
        public int new_troop_count_target;
        public int new_target_owner;
        public int new_fort_troop;


        public Attack(LogUtility.Attack attack , int turnId, int[] owners, int[] counts, int[] forts) : base(turnId, owners, counts, forts) 
        {
            type = Type.attack;
            durationMultiplyer = 1;
            attacker = attack.attacker;
            target = attack.target;
            new_troop_count_attacker = attack.new_troop_count_attacker;
            new_troop_count_target = attack.new_troop_count_target;
            new_target_owner = attack.new_target_owner;
            new_fort_troop = attack.new_fort_troop;
        }
        public override void Perform(IActionPerformer performer)
        {
            LogUtility.Attack info = new LogUtility.Attack
            {
                attacker = attacker,
                target = target,
                new_target_owner = new_target_owner,
                new_troop_count_attacker = new_troop_count_attacker,
                new_troop_count_target = new_troop_count_target,
                new_fort_troop = new_fort_troop,
            };
            performer.PerformAttack(info);
        }

        public override string ToString()
        {
            string result = owners[attacker] == new_target_owner ? "Success" : "Failure";
            string output = base.ToString() + "Attacking from " + attacker + " to " + target + 
                "\nResult: " + result +  divider + "New attacker troops: " + new_troop_count_attacker
                + divider + "new target troops: " + new_troop_count_target + divider + "target owner: " + new_target_owner;
            return output;
        }
    }

    public class Fortify : Action
    {
        public int number_of_troops;
        public int[] path;
        public Fortify(LogUtility.Fortify fortify, int turnId, int[] owners, int[] counts, int[] forts)
            : base(turnId, owners, counts, forts)
        {
            type = Type.fortify;
            durationMultiplyer = 0.75f;
            number_of_troops = fortify.number_of_troops;
            path = fortify.path;
        }
        public override void Perform(IActionPerformer performer)
        {
            LogUtility.Fortify info = new LogUtility.Fortify
            {
                path = path,
                number_of_troops = number_of_troops,
            };
            performer.PerformFortify(info);
        }
        public override string ToString()
        {
            string output = base.ToString() + "Fortifying " + number_of_troops + " troops" + divider + "Path: " + Utilities.ArrayToString(path);
            return output;
        }
    }

    public class Add:Action
    {
        public Add(int node, int amount, int turnId, int[] owners, int[] counts, int[] forts, int? owner = null)
            : base(turnId, owners, counts, forts)
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
            performer.PerformAdd(node, amount, owner);
        }
        public override string ToString()
        {
            string output = base.ToString() + "Adding " + amount + " troops to " + node;
            return output;
        }
    }

    public class Fort : Action
    {
        int node;
        int amount;

        public Fort(int node, int amount, int[] owners, int[] counts, int[] forts, int turnId) : base(turnId,owners,counts, forts)
        {
            durationMultiplyer = 0.35f;
            this.node = node;
            this.amount = amount;
        }
        public override void Perform(IActionPerformer performer)
        {
            performer.PerformFort(node, amount);
        }

        public override string ToString()
        {
            return base.ToString() + "Fort " + amount + " troops in " + node;
        }
    }



    /*
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
            performer.PerformUpdate(nodes_owner, troop_count);
        }
        public override string ToString()
        {
            string output = base.ToString() + "Updating...";
            return output;
        }
    }
    */
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

        public static void UpdateMapInfo(int[] owners, int[] counts, Attack action)
        {
            counts[action.attacker] = action.new_troop_count_attacker;
            counts[action.target] = action.new_troop_count_target;
            owners[action.target] = action.new_target_owner;
        }
        public static void UpdateMapInfo(int[] owners, int[] counts, Add action)
        {
            if(action.owner != null) { owners[action.node] = action.owner??-1; }
            counts[action.node] += action.amount;
        }
        public static void UpdateMapInfo(int[] owners, int[] counts, Fortify action)
        {
            counts[action.path[0]] -= action.number_of_troops;
            counts[action.path[action.path.Length - 1]] += action.number_of_troops;
        }
    }
}