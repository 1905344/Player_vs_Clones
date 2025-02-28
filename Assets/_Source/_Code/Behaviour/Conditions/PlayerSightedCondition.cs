using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player Sighted", story: "[Target] within sight range of [Range_Sensor]", category: "Conditions", id: "eed1b7a722194ba118626bef718dc8e0")]
public partial class PlayerSightedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<RangeSensor> Range_Sensor;

    public override bool IsTrue()
    {
        return Range_Sensor.Value.OnDetectionPerformed(Target.Value) != null;
    }
}
