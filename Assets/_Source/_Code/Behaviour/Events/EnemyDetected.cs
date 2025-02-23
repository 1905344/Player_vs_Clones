using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Enemy Detected")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Enemy Detected", message: "[Agent] has spotted [enemy]", category: "Events", id: "50173fd4d1e89d05bf68863f9beee85e")]
public partial class EnemyDetected : EventChannelBase
{
    public delegate void EnemyDetectedEventHandler(GameObject Agent, GameObject enemy);
    public event EnemyDetectedEventHandler Event; 

    public void SendEventMessage(GameObject Agent, GameObject enemy)
    {
        Event?.Invoke(Agent, enemy);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> AgentBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Agent = AgentBlackboardVariable != null ? AgentBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<GameObject> enemyBlackboardVariable = messageData[1] as BlackboardVariable<GameObject>;
        var enemy = enemyBlackboardVariable != null ? enemyBlackboardVariable.Value : default(GameObject);

        Event?.Invoke(Agent, enemy);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        EnemyDetectedEventHandler del = (Agent, enemy) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Agent;

            BlackboardVariable<GameObject> var1 = vars[1] as BlackboardVariable<GameObject>;
            if(var1 != null)
                var1.Value = enemy;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as EnemyDetectedEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as EnemyDetectedEventHandler;
    }
}

