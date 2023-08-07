using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Actions
{
    public abstract class Action
    {
        public enum Type
        {
            attack,
            fortify,
            add,
            update,
        }

        public Type type;

        public abstract void Perform(IActionPerformer performer);
    }

    public class Attack : Action
    {
        public int attacker;
        public int target;
        public int new_troop_count_attacker;
        public int new_troop_count_target;
        public int new_target_owner;

        public Attack(LogUtility.Attack attack)
        {
            type = Type.attack;
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
    }

    public class Fortify : Action
    {
        public int number_of_troops;
        public int[] path;
        public Fortify(LogUtility.Fortify fortify)
        {
            type = Type.fortify;
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
    }

    public class Add:Action
    {
        public Add(int node, int amount)
        {
            type = Type.add;
            this.node = node;
            this.amount = amount;
        }
        public int node;
        public int amount;
        public override void Perform(IActionPerformer performer)
        {
            Debug.Log("Adding " +  amount + "soldiers to " +  node);
            performer.PerformAdd(node, amount);
        }
    }

    public class Update : Action
    {
        public int[] nodes_owner;
        public int[] troop_count;
        public Update(int[] owners, int[] count)
        {
            type = Type.update;
            nodes_owner = owners;
            troop_count = count;
        }
        public override void Perform(IActionPerformer performer)
        {
            Debug.Log("Updating...");
            performer.PerformUpdate(nodes_owner, troop_count);
        }
    }
}

