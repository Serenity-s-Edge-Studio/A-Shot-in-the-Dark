using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace Assets.Scripts.Player.Tools
{
    public class ToolsManager : MonoBehaviour
    {
        public static ToolsManager instance;
        private void Awake()
        {
            if (instance)
                Destroy(this);
            instance = this;
        }
        public Vector2 mousePosition { get; internal set; }
        public Vector2 mouseWorldPos { get; internal set; }
        private new Camera camera;
        [SerializeField]
        private CinemachineVirtualCamera vCam;
        private void Start()
        {
            camera = Camera.main;
        }
        private void Update()
        {
            mousePosition = Mouse.current.position.ReadValue();
            mouseWorldPos = camera.ScreenToWorldPoint(mousePosition);
        }
        private void OnToggleRTSMode()
        {
            enabled = !enabled;
            foreach (var tool in GetComponents<ToolBase>())
                tool.enabled = enabled;
            vCam.Priority = enabled ? 15 : 5;
        }
    }
}