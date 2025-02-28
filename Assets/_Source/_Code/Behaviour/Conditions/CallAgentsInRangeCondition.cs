using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Agents within range", story: "Any [Other_Agent] near to [Agent]", category: "Conditions", id: "bca78fde39abd4c555c3c986e2b06812")]
public partial class CallAgentsInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Other_Agent;

    private RangeSensor rangeSensor;

    public override bool IsTrue()
    {
        var otherAgent = rangeSensor.GetNearestTarget(Other_Agent.Value.tag);

        return otherAgent != null;
    }

    //public override void OnStart()
    //{
    //}

    //public override void OnEnd()
    //{
    //}
}
