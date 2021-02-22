using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBuildTool : MonoBehaviour
{
    private Building SelectedBuilding;
    private PlayerInput.BuildToolActions input;
    private new Camera camera;
    
    [SerializeField]
    private ConstructableSO[] _AvailableBuildings;
    [SerializeField]
    private Transform _ButtonContainer;
    [SerializeField]
    private GameObject _BuildingUI;
    [SerializeField]
    private Button _ButtonPrefab;

    private void Start()
    {
        input = new PlayerInput().BuildTool;
        input.ToggleBuildTool.performed += ToggleBuildTool_performed;
        input.PlaceBuilding.performed += PlaceBuilding_performed;
        camera = Camera.main;
        //Generate buttons
        foreach (ConstructableSO constructable in _AvailableBuildings)
        {
            Button button = Instantiate(_ButtonPrefab, _ButtonContainer);
            button.onClick.AddListener(() => SelectBuilding(constructable));
        }
    }

    private void Update()
    {
        if (SelectedBuilding != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
            SelectedBuilding.transform.position = worldPos;
        }
    }
    private void PlaceBuilding_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    private void ToggleBuildTool_performed(InputAction.CallbackContext obj)
    {
        _BuildingUI.SetActive(obj.ReadValueAsButton());
        if (SelectedBuilding != null) Destroy(SelectedBuilding);
    }
    public void SelectBuilding(ConstructableSO constructable)
    {
        Debug.Log("Building Selected");
    }
}
