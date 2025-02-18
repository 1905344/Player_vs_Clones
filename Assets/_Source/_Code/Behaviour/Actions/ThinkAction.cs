using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Think", story: "[Agent] stops for [X] seconds to think for [Y] seconds", category: "Action", id: "6f722ef5a9ddea4f4ab4cf6bb2e9f925")]
public partial class ThinkAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> X;
    [SerializeReference] public BlackboardVariable<float> Y;
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

