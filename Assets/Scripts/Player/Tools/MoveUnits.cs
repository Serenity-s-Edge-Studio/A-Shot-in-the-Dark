using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts.AI;

namespace Assets.Scripts.Player.Tools
{
    [RequireComponent(typeof(SelectUnits))]
    public class MoveUnits : ToolBase
    {
        private void OnSecondaryAction(InputValue value)
        {
            if (!enabled) return; 
            SelectUnits selector = GetComponent<SelectUnits>();
            if (!selector.isSelecting)
            {
                foreach (Soldier soldier in selector.selectedUnits)
                {
                    soldier.IterativeOrders.Push(new MoveTo(ToolsManager.instance.mouseWorldPos));
                }
            }
        }
    }
}