using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "P3_Assign Targets", story: "Assign [player] and [lighthouse] variables", category: "Action", id: "85ff3fe3868176d4ce3db7f1f27d88f1")]
public partial class P3AssignTargetsAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Lighthouse;
    [SerializeReference] public BlackboardVariable<P3_EnemyBase> enemyBaseScript;

    protected override Status OnUpdate()
    {
        Player.Value = enemyBaseScript.Value.playerRef;
        Lighthouse.Value = enemyBaseScript.Value.lighthouseRef;
        return Status.Success;
    }
}

