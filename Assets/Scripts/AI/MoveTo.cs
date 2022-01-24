using System.Collections;
using UnityEngine;
using Pathfinding;

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
            if (caller.TryGetComponent(out AIPath agent))
            {
                agent.destination = Target;
                isComplete = true;
            }
            else
                throw new MissingComponentException("Cannot set the target of a non-agent!");
        }
    }
}