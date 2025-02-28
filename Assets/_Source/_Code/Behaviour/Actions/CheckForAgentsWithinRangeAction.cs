using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check for Agents within Range", story: "[Agent] uses range sensor to check for [Another_Agent] in range", category: "Action", id: "7a7f8308d24e5b1e3d5c1f55c138c37f")]
public partial class CheckForAgentsWithinRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Another_Agent;

    private RangeSensor rangeSensor;

    //protected override Status OnStart()
    //{
    //    return Status.Running;
    //}

    protected override Status OnUpdate()
    {
        var target = rangeSensor.GetNearestTarget(Another_Agent.Value.tag);

        if (target == null)
        {
            return Status.Running;
        }

        Another_Agent.Value = target.gameObject;

        return Status.Success;
    }

    //protected override void OnEnd()
    //{
    //}
}

