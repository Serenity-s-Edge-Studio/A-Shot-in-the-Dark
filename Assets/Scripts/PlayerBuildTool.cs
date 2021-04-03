using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBuildTool : MonoBehaviour
{
    private Building _SelectedBuildingPreview;
    private ConstructableSO _SelectedBuildingSO;
    private PlayerInput.BuildToolActions input;
    private new Camera camera;
    private List<BuildingCostUI> _BuildingCosts = new List<BuildingCostUI>();
    private InventoryController inventoryController;
    private bool _IsRotating = false;
    
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
        input.RotateBuilding.performed += ctx => _IsRotating = ctx.ReadValueAsButton();
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
        if (_SelectedBuildingPreview != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 worldPos = camera.ScreenToWorldPoint(mousePos);
            if (_IsRotating && _SelectedBuildingSO.CanRotate)
            {
                Vector2 dir = worldPos - (Vector2)_SelectedBuildingPreview.transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _SelectedBuildingPreview.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                _SelectedBuildingPreview.transform.position = worldPos;
            }
        }
    }
    private void PlaceBuilding_performed(InputAction.CallbackContext obj)
    {
        if (_SelectedBuildingPreview != null && _SelectedBuildingSO)
        {
            Debug.Log("Place Building");
            if (_SelectedBuildingPreview.IsBuildAreaClear() && inventoryController.inventory.ContainsAll(_SelectedBuildingSO.RequiredResources))
            {
                foreach (ItemStack stack in _SelectedBuildingSO.RequiredResources)
                    inventoryController.inventory.TryRetriveItems(stack.item, stack.Amount);
                _SelectedBuildingPreview.Build();
                _SelectedBuildingPreview = Instantiate(_SelectedBuildingSO.buildingPrefab);
            }
        }
    }

    private void ToggleBuildTool_performed(InputAction.CallbackContext obj)
    {
        _BuildingUI.SetActive(!_BuildingUI.activeInHierarchy);
        if (_SelectedBuildingPreview != null) Destroy(_SelectedBuildingPreview.gameObject);
    }
    public void SelectBuilding(ConstructableSO constructable)
    {
        Debug.Log("Building Selected");
        _BuildingUI.SetActive(false);
        _SelectedBuildingSO = constructable;
        _SelectedBuildingPreview = Instantiate(constructable.buildingPrefab);
    }
    public void UpdatePriceInfo(ConstructableSO constructable)
    {
        _NameText.gameObject.SetActive(true);
        _NameText.text = constructable.name;
        for (int i = 0; i < constructable.RequiredResources.Count; i++)
        {
            if (i >= _BuildingCosts.Count)
            {
                _BuildingCosts.Add(Instantiate(_BuildingCostViewPrefab, _BuildingCostContainer));
            }
            _BuildingCosts[i].name = constructable.RequiredResources[i].item.name;
            _BuildingCosts[i].UpdateUI(constructable.RequiredResources[i], inventoryController);
            _BuildingCosts[i].gameObject.SetActive(true);
        }
        for (int i = constructable.RequiredResources.Count; i < _BuildingCosts.Count; i++)
        {
            _BuildingCosts[i].gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        input.Disable();
    }
}
