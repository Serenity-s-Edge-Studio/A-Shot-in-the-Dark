using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player.Tools
{
    public abstract class ToolBase : MonoBehaviour
    {
        protected virtual void Start()
        {
            enabled = false;
        }
    }
}