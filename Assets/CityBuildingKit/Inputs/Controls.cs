//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/CityBuildingKit/Inputs/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Inputs
{
    public partial class @Controls: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Main"",
            ""id"": ""a361b01b-d79b-4ebd-9b82-37d5aad45b4b"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""491234c6-6635-4856-97fa-2bca95b50d81"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveDelta"",
                    ""type"": ""Value"",
                    ""id"": ""5aced688-4b8d-495f-96ec-3169ef196b02"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MouseScroll"",
                    ""type"": ""Value"",
                    ""id"": ""6a360ad8-b363-482e-b5b4-d1c9e8d2e502"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""a4f6a325-8a36-4b29-b427-9b9a85e16921"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TouchZoom"",
                    ""type"": ""Button"",
                    ""id"": ""9ce4c06d-da8d-4087-8e47-7004ce455904"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TouchPosition0"",
                    ""type"": ""Value"",
                    ""id"": ""311fffc7-be83-47cd-bf00-32cc90c0864e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""TouchPosition1"",
                    ""type"": ""Value"",
                    ""id"": ""cfbb727f-e7ae-40ed-b8ba-6909a6357de5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PointerPosition"",
                    ""type"": ""Value"",
                    ""id"": ""98c6a63d-8336-42af-a4d3-457001d258ef"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""PointerClick"",
                    ""type"": ""Button"",
                    ""id"": ""8991922c-c2a3-468d-a2f4-2848ea1c8005"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0397663b-2d07-4d26-97e0-5eeabd465f7a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62be907f-afc4-4075-bc1f-903dd63966f4"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""685b8b5b-ddfd-497b-92b5-53d209da78d4"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""087112fd-f40d-4529-8afe-3265ab2074b8"",
                    ""path"": ""<Touchscreen>/primaryTouch/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""02133508-720e-4682-a8c2-8cfb140cbcf5"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62e07e17-1d07-43d1-a847-1bfa0b6735f1"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""One Modifier"",
                    ""id"": ""28826853-3ca5-40a5-ae9c-6596dbaf91e4"",
                    ""path"": ""OneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchZoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""6a38e90b-756b-4f84-babd-018dbf13f5d8"",
                    ""path"": ""<Touchscreen>/touch0/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""binding"",
                    ""id"": ""756346ec-4461-4cab-83fb-5992f123c73f"",
                    ""path"": ""<Touchscreen>/touch1/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b759d91a-0e53-4ca7-8989-ba4417290083"",
                    ""path"": ""<Touchscreen>/touch0/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition0"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a9138bcc-f391-4480-aa37-a28d015081cb"",
                    ""path"": ""<Touchscreen>/touch1/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TouchPosition1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b643bd5-d838-46c4-b843-a4fbe1565361"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4753a3a1-74cf-490e-aa6e-4550355c0873"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b6908fc6-a13b-4257-9be0-c71707d452a3"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40d65b14-0362-412d-bf0f-c8825511087b"",
                    ""path"": ""<Touchscreen>/primaryTouch/tap"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Main
            m_Main = asset.FindActionMap("Main", throwIfNotFound: true);
            m_Main_Move = m_Main.FindAction("Move", throwIfNotFound: true);
            m_Main_MoveDelta = m_Main.FindAction("MoveDelta", throwIfNotFound: true);
            m_Main_MouseScroll = m_Main.FindAction("MouseScroll", throwIfNotFound: true);
            m_Main_MousePosition = m_Main.FindAction("MousePosition", throwIfNotFound: true);
            m_Main_TouchZoom = m_Main.FindAction("TouchZoom", throwIfNotFound: true);
            m_Main_TouchPosition0 = m_Main.FindAction("TouchPosition0", throwIfNotFound: true);
            m_Main_TouchPosition1 = m_Main.FindAction("TouchPosition1", throwIfNotFound: true);
            m_Main_PointerPosition = m_Main.FindAction("PointerPosition", throwIfNotFound: true);
            m_Main_PointerClick = m_Main.FindAction("PointerClick", throwIfNotFound: true);
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

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Main
        private readonly InputActionMap m_Main;
        private List<IMainActions> m_MainActionsCallbackInterfaces = new List<IMainActions>();
        private readonly InputAction m_Main_Move;
        private readonly InputAction m_Main_MoveDelta;
        private readonly InputAction m_Main_MouseScroll;
        private readonly InputAction m_Main_MousePosition;
        private readonly InputAction m_Main_TouchZoom;
        private readonly InputAction m_Main_TouchPosition0;
        private readonly InputAction m_Main_TouchPosition1;
        private readonly InputAction m_Main_PointerPosition;
        private readonly InputAction m_Main_PointerClick;
        public struct MainActions
        {
            private @Controls m_Wrapper;
            public MainActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Main_Move;
            public InputAction @MoveDelta => m_Wrapper.m_Main_MoveDelta;
            public InputAction @MouseScroll => m_Wrapper.m_Main_MouseScroll;
            public InputAction @MousePosition => m_Wrapper.m_Main_MousePosition;
            public InputAction @TouchZoom => m_Wrapper.m_Main_TouchZoom;
            public InputAction @TouchPosition0 => m_Wrapper.m_Main_TouchPosition0;
            public InputAction @TouchPosition1 => m_Wrapper.m_Main_TouchPosition1;
            public InputAction @PointerPosition => m_Wrapper.m_Main_PointerPosition;
            public InputAction @PointerClick => m_Wrapper.m_Main_PointerClick;
            public InputActionMap Get() { return m_Wrapper.m_Main; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MainActions set) { return set.Get(); }
            public void AddCallbacks(IMainActions instance)
            {
                if (instance == null || m_Wrapper.m_MainActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_MainActionsCallbackInterfaces.Add(instance);
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @MoveDelta.started += instance.OnMoveDelta;
                @MoveDelta.performed += instance.OnMoveDelta;
                @MoveDelta.canceled += instance.OnMoveDelta;
                @MouseScroll.started += instance.OnMouseScroll;
                @MouseScroll.performed += instance.OnMouseScroll;
                @MouseScroll.canceled += instance.OnMouseScroll;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @TouchZoom.started += instance.OnTouchZoom;
                @TouchZoom.performed += instance.OnTouchZoom;
                @TouchZoom.canceled += instance.OnTouchZoom;
                @TouchPosition0.started += instance.OnTouchPosition0;
                @TouchPosition0.performed += instance.OnTouchPosition0;
                @TouchPosition0.canceled += instance.OnTouchPosition0;
                @TouchPosition1.started += instance.OnTouchPosition1;
                @TouchPosition1.performed += instance.OnTouchPosition1;
                @TouchPosition1.canceled += instance.OnTouchPosition1;
                @PointerPosition.started += instance.OnPointerPosition;
                @PointerPosition.performed += instance.OnPointerPosition;
                @PointerPosition.canceled += instance.OnPointerPosition;
                @PointerClick.started += instance.OnPointerClick;
                @PointerClick.performed += instance.OnPointerClick;
                @PointerClick.canceled += instance.OnPointerClick;
            }

            private void UnregisterCallbacks(IMainActions instance)
            {
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
                @MoveDelta.started -= instance.OnMoveDelta;
                @MoveDelta.performed -= instance.OnMoveDelta;
                @MoveDelta.canceled -= instance.OnMoveDelta;
                @MouseScroll.started -= instance.OnMouseScroll;
                @MouseScroll.performed -= instance.OnMouseScroll;
                @MouseScroll.canceled -= instance.OnMouseScroll;
                @MousePosition.started -= instance.OnMousePosition;
                @MousePosition.performed -= instance.OnMousePosition;
                @MousePosition.canceled -= instance.OnMousePosition;
                @TouchZoom.started -= instance.OnTouchZoom;
                @TouchZoom.performed -= instance.OnTouchZoom;
                @TouchZoom.canceled -= instance.OnTouchZoom;
                @TouchPosition0.started -= instance.OnTouchPosition0;
                @TouchPosition0.performed -= instance.OnTouchPosition0;
                @TouchPosition0.canceled -= instance.OnTouchPosition0;
                @TouchPosition1.started -= instance.OnTouchPosition1;
                @TouchPosition1.performed -= instance.OnTouchPosition1;
                @TouchPosition1.canceled -= instance.OnTouchPosition1;
                @PointerPosition.started -= instance.OnPointerPosition;
                @PointerPosition.performed -= instance.OnPointerPosition;
                @PointerPosition.canceled -= instance.OnPointerPosition;
                @PointerClick.started -= instance.OnPointerClick;
                @PointerClick.performed -= instance.OnPointerClick;
                @PointerClick.canceled -= instance.OnPointerClick;
            }

            public void RemoveCallbacks(IMainActions instance)
            {
                if (m_Wrapper.m_MainActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IMainActions instance)
            {
                foreach (var item in m_Wrapper.m_MainActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_MainActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public MainActions @Main => new MainActions(this);
        public interface IMainActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnMoveDelta(InputAction.CallbackContext context);
            void OnMouseScroll(InputAction.CallbackContext context);
            void OnMousePosition(InputAction.CallbackContext context);
            void OnTouchZoom(InputAction.CallbackContext context);
            void OnTouchPosition0(InputAction.CallbackContext context);
            void OnTouchPosition1(InputAction.CallbackContext context);
            void OnPointerPosition(InputAction.CallbackContext context);
            void OnPointerClick(InputAction.CallbackContext context);
        }
    }
}
