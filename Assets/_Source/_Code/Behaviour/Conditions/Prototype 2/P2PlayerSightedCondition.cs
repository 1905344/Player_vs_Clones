using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "P2_PlayerSightedCondition", story: "[Target] within sight range of [P2_Range_Sensor]", category: "Conditions", id: "b8da9370f67f6bcf6ca0e4e5932b02b4")]
public partial class P2PlayerSightedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<P2_RangeSensor> P2_Range_Sensor;

    public override bool IsTrue()
    {
        return P2_Range_Sensor.Value.OnDetectionPerformed(Target.Value) != null;
    }
}
