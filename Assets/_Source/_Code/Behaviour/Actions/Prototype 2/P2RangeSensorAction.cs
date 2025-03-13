using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "P2_RangeSensor", story: "Update [P2_Range_Sensor] and assign [Target]", category: "Action", id: "ee12b085bedc455ebd67265e17e0aac9")]
public partial class P2RangeSensorAction : Action
{
    [SerializeReference] public BlackboardVariable<P2_RangeSensor> P2_Range_Sensor;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    protected override Status OnUpdate()
    {
        Target.Value = P2_Range_Sensor.Value.UpdateSensor();
        return Target.Value == null ? Status.Failure : Status.Success;
    }
}