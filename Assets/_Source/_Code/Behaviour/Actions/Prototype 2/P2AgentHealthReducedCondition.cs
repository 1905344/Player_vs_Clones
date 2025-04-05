using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "P2_Agent Health Reduced", story: "Check if [health] from [health_script] is less than [max_health]", category: "Conditions", id: "d7eeba218e4aa5cd1a4f7b1ee163e1a0")]
public partial class P2AgentHealthReducedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<int> Health;
    [SerializeReference] public BlackboardVariable<P2_enemyHealth> Health_script;
    [Comparison(comparisonType: ComparisonType.All)]
    [SerializeReference] public BlackboardVariable<int> Max_health;

    public override bool IsTrue()
    {
        if (Health.Value < Max_health.Value)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void OnStart()
    {
        Health.Value = Health_script.Value.health;
        Max_health.Value = Health_script.Value.health;
    }

    public override void OnEnd()
    {
    }
}
