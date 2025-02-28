using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Trigger Enemy Attack", story: "Set [enemy_attack] trigger to [bool]", category: "Action", id: "f6cb3239d322673f4accc9ee42cb9f75")]
public partial class TriggerEnemyAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<enemyAttack> Enemy_attack;
    [SerializeReference] public BlackboardVariable<bool> Bool;
    //protected override Status OnStart()
    //{
    //    return Status.Running;
    //}

    protected override Status OnUpdate()
    {
        if (!Bool)
        {
            Enemy_attack.Value.isAttacking = false;
            return Status.Failure;
        }
        else
        {
            Enemy_attack.Value.isAttacking = true;

            return Status.Success;
        }
    }

    //protected override void OnEnd()
    //{
    //}
}

