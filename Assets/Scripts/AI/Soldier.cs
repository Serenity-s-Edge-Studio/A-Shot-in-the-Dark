using System.Collections;
using UnityEngine;
using Pathfinding;

namespace Assets.Scripts.AI
{
    [RequireComponent(typeof(AIPath))]
    public class Soldier : RTSObject
    {
        private Seeker agent;
        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            agent = GetComponent<Seeker>();
        }
    }
}