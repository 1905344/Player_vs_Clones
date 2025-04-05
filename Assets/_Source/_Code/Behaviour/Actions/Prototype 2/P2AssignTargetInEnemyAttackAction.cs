using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "P2_Assign Target In Enemy Attack", story: "Assign [Target] in [P2_Enemy_Attack]", category: "Action", id: "4c6720209adb11fbbb2d2c300a0dbeef")]
public partial class P2AssignTargetInEnemyAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<P2_enemyAttack> P2_Enemy_Attack;

    protected override Status OnUpdate()
    {
        if (Target == null)
        {
            return Status.Failure;
        }

        P2_Enemy_Attack.Value.targetPlayer = Target.Value;
        return Status.Success;
    }
}

