using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player Sighted", story: "[Player] within sight [range] of [agent]", category: "Conditions", id: "eed1b7a722194ba118626bef718dc8e0")]
public partial class PlayerSightedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<Vector3> Range;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

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
