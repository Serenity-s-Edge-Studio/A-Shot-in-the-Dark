using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Soldier : RTSObject
    {
        private NavMeshAgent agent;
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            agent = GetComponent<NavMeshAgent>();
            agent.updateUpAxis = true;
            agent.updateRotation = true;
        }
    }
}