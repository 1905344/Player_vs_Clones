using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Walk Between Two Locations", story: "[Agent] walks from [Location_1] to [Location_2]", category: "Action", id: "b9d60a72accbb904fa0d68295e546841")]
public partial class WalkBetweenTwoLocationsAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Transform> Location_1;
    [SerializeReference] public BlackboardVariable<Transform> Location_2;

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

