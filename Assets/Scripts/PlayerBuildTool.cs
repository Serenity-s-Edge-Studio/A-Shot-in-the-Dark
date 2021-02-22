using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBuildTool : MonoBehaviour
{
    private Building SelectedBuilding;
    private PlayerInput.BuildToolActions input;
    private new Camera camera;
    private List<BuildingCostUI> _BuildingCosts = new List<BuildingCostUI>();
    private InventoryController inventoryController;
    
    [SerializeField]
    private ConstructableSO[] _AvailableBuildings;
    [SerializeField]
    private Transform _ButtonContainer;
    [SerializeField]
    private GameObject _BuildingUI;
    [SerializeField]
    private BuildingToolButton _ButtonPrefab;
    [SerializeField]
    private BuildingCostUI _BuildingCostViewPrefab;
    [SerializeField]
    private Transform _BuildingCostContainer;
    [SerializeField]
    private TextMeshProUGUI _NameText;

    private void Start()
    {
        input = new PlayerInput().BuildTool;
        input.ToggleBuildTool.performed += ToggleBuildTool_performed;
        input.PlaceBuilding.performed += PlaceBuilding_performed;
        input.Enable();
        camera = Camera.main;
        inventoryController = GetComponentInChildren<InventoryController>();
        //Generate buttons
        foreach (ConstructableSO constructable in _AvailableBuildings)
        {
            BuildingToolButton buildingToolButton = Instantiate(_ButtonPrefab, _ButtonContainer);
            buildingToolButton.button.onClick.AddListener(() => SelectBuilding(constructable));
            buildingToolButton.OnPointerEnter.AddListener(() => UpdatePriceInfo(constructable));
            buildingToolButton.image.sprite = constructable.icon;
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
        if (SelectedBuilding != null)
        {
            Debug.Log("Place Building");
        }
    }

    private void ToggleBuildTool_performed(InputAction.CallbackContext obj)
    {
        _BuildingUI.SetActive(!_BuildingUI.activeInHierarchy);
        if (SelectedBuilding != null) Destroy(SelectedBuilding);
    }
    public void SelectBuilding(ConstructableSO constructable)
    {
        Debug.Log("Building Selected");
    }
    public void UpdatePriceInfo(ConstructableSO constructable)
    {
        _NameText.gameObject.SetActive(true);
        _NameText.text = constructable.name;
        for (int i = 0; i < constructable.RequiredResources.Length; i++)
        {
            if (i >= _BuildingCosts.Count)
            {
                _BuildingCosts.Add(Instantiate(_BuildingCostViewPrefab, _BuildingCostContainer));
            }
            _BuildingCosts[i].name = constructable.RequiredResources[i].item.name;
            _BuildingCosts[i].UpdateUI(constructable.RequiredResources[i], inventoryController);
            _BuildingCosts[i].gameObject.SetActive(true);
        }
        for (int i = constructable.RequiredResources.Length; i < _BuildingCosts.Count; i++)
        {
            _BuildingCosts[i].gameObject.SetActive(false);
        }
    }
}
