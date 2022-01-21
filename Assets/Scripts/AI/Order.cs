using System.Collections;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public abstract class Order
    {
        public bool isBlocking { get; protected set; }
        public bool isComplete { get; protected set; }
        public abstract void Continue(RTSObject caller);
    }
}