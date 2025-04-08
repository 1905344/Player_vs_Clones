using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "P3_Set Enemy Type", story: "Set enemy type in [base_script] to [enemy_type]", category: "Action", id: "14eaf35dc372fc1bc39abf3103eb1c40")]
public partial class P3SetEnemyTypeAction : Action
{
    [SerializeReference] public BlackboardVariable<P3_EnemyBase> Base_script;
    [SerializeReference] public BlackboardVariable<P3_Enemy_Types> Enemy_type;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

