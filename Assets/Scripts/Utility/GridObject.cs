using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public abstract class GridObject : MonoBehaviour
    {
        public virtual void addToGrid()
        {
            GridManager.instance.Add(this);
        }
        public virtual void removeFromGrid()
        {
            GridManager.instance.Remove(this);
        }
    }
}