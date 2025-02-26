using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check for Agents within Range", story: "Update [Range_Sensor] to check for [Agents] within [range]", category: "Action", id: "7a7f8308d24e5b1e3d5c1f55c138c37f")]
public partial class CheckForAgentsWithinRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeSensor> Range_Sensor;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Agents;
    [SerializeReference] public BlackboardVariable<float> Range;

    private RangeSensor rangeSensor;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var nearbyAgent = rangeSensor.GetNearestTarget("Enemy");
        
        if (nearbyAgent == null)
        {
            return Status.Running;
        }

        for(var i = 0; i < Agents.Value.Count; i++)
        {
            Agents.Value[i] = nearbyAgent.gameObject;
        }

        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

