using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.AI
{
    public class MoveTo : Order
    {
        private Vector2 Target;
        public MoveTo(Vector2 TargetPosition)
        {
            Target = TargetPosition;
        }
        public override void Continue(RTSObject caller)
        {
            if (caller.TryGetComponent(out NavMeshAgent agent))
                isComplete = agent.SetDestination(Target);
            else
                throw new MissingComponentException("Cannot set the target of a non-agent!");
        }
    }
}