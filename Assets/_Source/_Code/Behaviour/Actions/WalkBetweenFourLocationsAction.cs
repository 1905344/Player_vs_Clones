using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Walk between four locations", story: "Agent walks from [Location_1] to [Location_2] to [Location_3] then [Location_4]", category: "Action", id: "23937f6560880cb7b0ee165aa21cf966")]
public partial class WalkBetweenFourLocationsAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Location_1;
    [SerializeReference] public BlackboardVariable<Transform> Location_2;
    [SerializeReference] public BlackboardVariable<Transform> Location_3;
    [SerializeReference] public BlackboardVariable<Transform> Location_4;

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

