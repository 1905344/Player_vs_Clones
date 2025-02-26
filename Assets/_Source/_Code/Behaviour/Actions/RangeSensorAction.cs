using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Range Sensor", story: "Update [Range_Sensor] and assign [Target]", category: "Action", id: "c15da536fcb94fcbe86e395af4bdd391")]
public partial class RangeSensorAction : Action
{
    [SerializeReference] public BlackboardVariable<RangeSensor> Range_Sensor;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    NavMeshAgent nmAgent;
    RangeSensor rangeSensor;

    //protected override Status OnStart()
    //{
    //    return Status.Running;
    //}

    protected override Status OnUpdate()
    {
        var target = rangeSensor.GetNearestTarget("Player");

        if (target == null)
        {
            return Status.Running;
        }

        Target.Value = target.gameObject;

        return Status.Success;
    }

    //protected override void OnEnd()
    //{
    //}
}

