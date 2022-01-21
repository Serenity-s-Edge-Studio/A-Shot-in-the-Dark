using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Scripts.AI {
    public class RTSObject : Entity
    {
        public Stack<Order> IterativeOrders = new Stack<Order>();
        public List<Order> ContinousOrders = new List<Order>();

        protected virtual void Start()
        {
            addToGrid();
        }
        private void Update()
        {
            if (ContinousOrders.Count > 0 && !(IterativeOrders.Count > 0 && IterativeOrders.Peek().isBlocking))
            {
                ContinousOrders = ContinousOrders.Where((Order order) =>
                {
                    order.Continue(this);
                    return !order.isComplete;
                }) as List<Order>;
            }
            if (IterativeOrders.Count() > 0)
            {
                IterativeOrders.Peek().Continue(this);
                if (IterativeOrders.Peek().isComplete)
                    IterativeOrders.Pop();
            }
        }

        public override void Damage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                removeFromGrid();
                Destroy(gameObject);
            }
        }

        public override void Heal(int amount)
        {
            health += amount;
        }

    }
}
