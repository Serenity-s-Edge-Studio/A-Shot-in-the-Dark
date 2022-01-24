using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts.AI;
using System.Collections.Generic;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Player.Tools
{
    [RequireComponent(typeof(ToolsManager))]
    public class SelectUnits : ToolBase
    {
        private ToolsManager toolsManager;
        public bool isSelecting { get; internal set; }
        [SerializeField]
        public List<RTSObject> selectedUnits { get; internal set; }
        private Vector2 startPos;
        private Vector2 currentPos;
        [SerializeField]
        private GameObject graphic;
        private void Start()
        {
            toolsManager = GetComponent<ToolsManager>();
            isSelecting = false;
        }
        private void Update()
        {
            currentPos = toolsManager.mouseWorldPos;
            if (isSelecting)
            {
                Bounds bounds = new Bounds();
                bounds.SetMinMax(startPos, currentPos);
                graphic.transform.position = bounds.center;
                graphic.transform.localScale = bounds.size;
            }
        }
        private void OnPrimaryActionHeld(InputValue context)
        {
            if (!enabled) return;
            if (context.isPressed)
            {
                startPos = currentPos;
                isSelecting = true;
                graphic.SetActive(true);
            }
            else
            {
                Vector2 endPos = currentPos;
                Bounds bounds = new Bounds();
                bounds.SetMinMax(Vector2.Min(startPos, endPos), Vector2.Max(startPos, endPos));
                selectedUnits = new List<RTSObject>(GridManager.instance.GetGridObjectsInBounds<RTSObject>(bounds));
                isSelecting = false;
                graphic.SetActive(false);
            }
        }
    }
}