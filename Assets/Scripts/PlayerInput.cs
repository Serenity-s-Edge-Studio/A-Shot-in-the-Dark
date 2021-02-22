// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""b858ffdd-3be5-48a2-8754-cf58371eaaff"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""9049de6a-b93a-4621-a6bd-1aed376b9b39"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""167666ee-f23e-4c3a-95cb-b3356e12c7e1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Mouse position"",
                    ""type"": ""Value"",
                    ""id"": ""c0797d7f-6008-4575-84a4-196977166308"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Drop Weapon"",
                    ""type"": ""Button"",
                    ""id"": ""c1436ce9-4657-4aaf-9896-dc2614136252"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""aff92b58-da60-4c8f-ae04-adb7d48dfe68"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Open inventory"",
                    ""type"": ""Button"",
                    ""id"": ""a5e828bd-399e-470c-b535-63e1b1aea39c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""9b9b06e8-a42e-4c78-a453-1b353cf8271d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5fa12563-439b-423b-bf7b-d145f0d624e4"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6a494d56-bac2-4b05-96dc-2146ce176f63"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f102fdb3-9949-4a4e-9c6e-ceb6f557cf6f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""99e11328-91ff-4c86-8613-44d1887d7fd3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""4d60d1c9-3c37-4d29-8231-2a4c6be86b14"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""90bb7b79-15f3-43ee-b427-ea9ff76f1ad2"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6f8f9b53-c00b-4ae7-bacd-852f53f25a7f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b848d6d6-3d92-4331-a86d-59964350b3b0"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""771253c9-3ce6-4457-96b7-a9ee56410f86"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b6f155ac-df25-43ef-bd41-63f64177e12e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""471cee28-fe9b-440e-b530-e17f85574d11"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c28e569-ab19-485e-89aa-f67555099021"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drop Weapon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b992ce12-b7e1-4fcf-904b-7aa8fdab31f2"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e653697-f136-4111-916d-b99971434b82"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Open inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""BuildTool"",
            ""id"": ""89dc070b-f183-4b1f-947f-e00bf8d3a049"",
            ""actions"": [
                {
                    ""name"": ""ToggleBuildTool"",
                    ""type"": ""Button"",
                    ""id"": ""2328135f-6cf2-4ef2-b55c-a7385cac3bcf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PlaceBuilding"",
                    ""type"": ""Button"",
                    ""id"": ""2671aedd-d67a-48b0-b3f9-05cc5a33bcb0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1faf628d-1273-49c7-83f4-d28ed5098a7f"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleBuildTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eaf66afa-edbc-4e84-af35-f568171f0575"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PlaceBuilding"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Shoot = m_Player.FindAction("Shoot", throwIfNotFound: true);
        m_Player_Mouseposition = m_Player.FindAction("Mouse position", throwIfNotFound: true);
        m_Player_DropWeapon = m_Player.FindAction("Drop Weapon", throwIfNotFound: true);
        m_Player_Reload = m_Player.FindAction("Reload", throwIfNotFound: true);
        m_Player_Openinventory = m_Player.FindAction("Open inventory", throwIfNotFound: true);
        // BuildTool
        m_BuildTool = asset.FindActionMap("BuildTool", throwIfNotFound: true);
        m_BuildTool_ToggleBuildTool = m_BuildTool.FindAction("ToggleBuildTool", throwIfNotFound: true);
        m_BuildTool_PlaceBuilding = m_BuildTool.FindAction("PlaceBuilding", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Shoot;
    private readonly InputAction m_Player_Mouseposition;
    private readonly InputAction m_Player_DropWeapon;
    private readonly InputAction m_Player_Reload;
    private readonly InputAction m_Player_Openinventory;
    public struct PlayerActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Shoot => m_Wrapper.m_Player_Shoot;
        public InputAction @Mouseposition => m_Wrapper.m_Player_Mouseposition;
        public InputAction @DropWeapon => m_Wrapper.m_Player_DropWeapon;
        public InputAction @Reload => m_Wrapper.m_Player_Reload;
        public InputAction @Openinventory => m_Wrapper.m_Player_Openinventory;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Shoot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Mouseposition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseposition;
                @Mouseposition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseposition;
                @Mouseposition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseposition;
                @DropWeapon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropWeapon;
                @DropWeapon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropWeapon;
                @DropWeapon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropWeapon;
                @Reload.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnReload;
                @Openinventory.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpeninventory;
                @Openinventory.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpeninventory;
                @Openinventory.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpeninventory;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Mouseposition.started += instance.OnMouseposition;
                @Mouseposition.performed += instance.OnMouseposition;
                @Mouseposition.canceled += instance.OnMouseposition;
                @DropWeapon.started += instance.OnDropWeapon;
                @DropWeapon.performed += instance.OnDropWeapon;
                @DropWeapon.canceled += instance.OnDropWeapon;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @Openinventory.started += instance.OnOpeninventory;
                @Openinventory.performed += instance.OnOpeninventory;
                @Openinventory.canceled += instance.OnOpeninventory;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // BuildTool
    private readonly InputActionMap m_BuildTool;
    private IBuildToolActions m_BuildToolActionsCallbackInterface;
    private readonly InputAction m_BuildTool_ToggleBuildTool;
    private readonly InputAction m_BuildTool_PlaceBuilding;
    public struct BuildToolActions
    {
        private @PlayerInput m_Wrapper;
        public BuildToolActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToggleBuildTool => m_Wrapper.m_BuildTool_ToggleBuildTool;
        public InputAction @PlaceBuilding => m_Wrapper.m_BuildTool_PlaceBuilding;
        public InputActionMap Get() { return m_Wrapper.m_BuildTool; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BuildToolActions set) { return set.Get(); }
        public void SetCallbacks(IBuildToolActions instance)
        {
            if (m_Wrapper.m_BuildToolActionsCallbackInterface != null)
            {
                @ToggleBuildTool.started -= m_Wrapper.m_BuildToolActionsCallbackInterface.OnToggleBuildTool;
                @ToggleBuildTool.performed -= m_Wrapper.m_BuildToolActionsCallbackInterface.OnToggleBuildTool;
                @ToggleBuildTool.canceled -= m_Wrapper.m_BuildToolActionsCallbackInterface.OnToggleBuildTool;
                @PlaceBuilding.started -= m_Wrapper.m_BuildToolActionsCallbackInterface.OnPlaceBuilding;
                @PlaceBuilding.performed -= m_Wrapper.m_BuildToolActionsCallbackInterface.OnPlaceBuilding;
                @PlaceBuilding.canceled -= m_Wrapper.m_BuildToolActionsCallbackInterface.OnPlaceBuilding;
            }
            m_Wrapper.m_BuildToolActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ToggleBuildTool.started += instance.OnToggleBuildTool;
                @ToggleBuildTool.performed += instance.OnToggleBuildTool;
                @ToggleBuildTool.canceled += instance.OnToggleBuildTool;
                @PlaceBuilding.started += instance.OnPlaceBuilding;
                @PlaceBuilding.performed += instance.OnPlaceBuilding;
                @PlaceBuilding.canceled += instance.OnPlaceBuilding;
            }
        }
    }
    public BuildToolActions @BuildTool => new BuildToolActions(this);
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnMouseposition(InputAction.CallbackContext context);
        void OnDropWeapon(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnOpeninventory(InputAction.CallbackContext context);
    }
    public interface IBuildToolActions
    {
        void OnToggleBuildTool(InputAction.CallbackContext context);
        void OnPlaceBuilding(InputAction.CallbackContext context);
    }
}
