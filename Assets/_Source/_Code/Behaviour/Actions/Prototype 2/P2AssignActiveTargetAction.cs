using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "P2_Assign Active Target", story: "Check [Player_Manager] to assign [Active_Target] from [Targets]", category: "Action", id: "6d676a34ffc0b569f9d69671acb3b8fe")]
public partial class P2AssignActiveTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<P2_PlayerManager> Player_Manager;
    [SerializeReference] public BlackboardVariable<GameObject> Active_Target;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Targets;

    private int currentIndex = 0;

    protected override Status OnUpdate()
    {
        currentIndex = Player_Manager.Value.GetCurrentCharacter();
        Active_Target.Value = Targets.Value[currentIndex].gameObject;

        return Active_Target.Value == null ? Status.Failure : Status.Success;
    }
}

