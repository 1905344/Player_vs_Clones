using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Range Sensor", story: "Update [Range_Sensor] and assign [Target]", category: "Action", id: "c15da536fcb94fcbe86e395af4bdd391")]
public partial class RangeSensorAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeSensor> Range_Sensor;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnUpdate()
    {
        Target.Value = Range_Sensor.Value.UpdateSensor();
        return Target.Value == null ? Status.Failure : Status.Success;
    }
}

