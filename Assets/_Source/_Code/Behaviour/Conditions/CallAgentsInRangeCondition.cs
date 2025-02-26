using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Agents within range", story: "Any [agents] within [range]", category: "Conditions", id: "bca78fde39abd4c555c3c986e2b06812")]
public partial class CallAgentsInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Agents;
    [SerializeReference] public BlackboardVariable<Vector3Int> Range;
    
    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
