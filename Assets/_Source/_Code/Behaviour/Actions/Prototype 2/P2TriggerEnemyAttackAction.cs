using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "P2_Trigger Enemy Attack", story: "Set [P2_Enemy_Attack] trigger to [bool]", category: "Action", id: "d0dbf44fba6098499afa1258da565c6e")]
public partial class P2TriggerEnemyAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<P2_enemyAttack> P2_Enemy_Attack;
    [SerializeReference] public BlackboardVariable<bool> Bool;

    protected override Status OnUpdate()
    {
        if (!Bool)
        {
            P2_Enemy_Attack.Value.isAttacking = false;
            return Status.Failure;
        }
        else
        {
            P2_Enemy_Attack.Value.isAttacking = true;

            return Status.Success;
        }
    }
}

