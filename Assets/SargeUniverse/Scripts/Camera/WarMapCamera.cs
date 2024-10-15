using System.Collections.Generic;
using System.Linq;
using Inputs;
using SargeUniverse.Scripts.Controller;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace SargeUniverse.Scripts.Camera
{
    public class WarMapCamera : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Camera _camera = null;
        [SerializeField]
        private float _moveSpeed = 50f;
        [SerializeField]
        private float _moveSmooth = 5f;
        [SerializeField]
        private float _zoomSpeed = 5f;
        [SerializeField]
        private float _zoomSmooth = 5f;
        
        private Controls _input = null;
        
        private bool _moving = false;
        private bool _zooming = false;
        
        private Vector3 _center = Vector3.zero;
        private float _right = 10f;
        private float _left = 10f;
        private float _up = 10f;
        private float _down = 10f;
        private float _angle = 45f;
        private float _zoom = 5f;
        private float _zoomMin = 1f;
        private float _zoomMax = 25f;
        
        public static float zoomScale;
        
        private Vector3 _moveRootBasePosition = Vector3.zero;
        private Vector3 _moveInputBaseWorldPosition = Vector3.zero;
        private Vector2 _moveInputBaseScreenPosition = Vector2.zero;
        private Vector2 _moveBaseDirection = Vector2.zero;
        
        private Vector2 _zoomPositionOnScreen = Vector2.zero;
        private Vector3 _zoomPositionInWorld = Vector3.zero;
        private float _zoomBaseValue = 0;
        private float _zoomBaseDistance = 0;

        private Transform _root = null;
        private Transform _pivot = null;
        private Transform _target = null;
        
        private void Awake()
        {
            _input = new Controls();
            _root = new GameObject("CameraHelper").transform;
            _pivot = new GameObject("CameraPivot").transform;
            _target = new GameObject("CameraTarget").transform;

            _camera.orthographic = true;
            _camera.nearClipPlane = 0;
            
            CameraUtils.Init(_camera);
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            _camera.nearClipPlane = 0.1f;
            _camera.farClipPlane = 100f;
            _zooming = false;
            _moving = false;
            _pivot.SetParent(_root);
            _target.SetParent(_pivot);
            
            _root.position = WarMapManager.Instanse.Grid.GetGridCenter();
            _root.eulerAngles = Vector3.zero;
            _pivot.localPosition = Vector3.zero;
            _pivot.localEulerAngles = Vector3.zero;
            _target.localPosition = new Vector3(0, 0, -50);
            _target.localEulerAngles = Vector3.zero;
            
            AdjustBounds();
            _camera.transform.position = _root.position;
            _camera.orthographicSize = _zoom;
            
            zoomScale = _camera.orthographicSize / _zoomMax;
        }

        private void OnEnable()
        {
            _input.Enable();
            _input.Main.Move.started += _ => MoveStarted();
            _input.Main.Move.canceled += _ => MoveCanceled();
            _input.Main.TouchZoom.started += _ => ZoomStarted();
            _input.Main.TouchZoom.canceled += _ => ZoomCanceled();
            _input.Main.PointerClick.performed += _ => ScreenClicked();
        }

        private void OnDisable()
        {
            _input.Main.Move.started -= _ => MoveStarted();
            _input.Main.Move.canceled -= _ => MoveCanceled();
            _input.Main.TouchZoom.started -= _ => ZoomStarted();
            _input.Main.TouchZoom.canceled -= _ => ZoomCanceled();
            _input.Main.PointerClick.performed -= _ => ScreenClicked();
            _input.Disable();
        }

        private void ScreenClicked()
        {
            var position = _input.Main.PointerPosition.ReadValue<Vector2>();
            
            var data = new PointerEventData(EventSystem.current) { position = position };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            
            if (results.Count <= 0)
            {
                var found = false;
                var planePosition = CameraUtils.CameraScreenPositionToWorldPosition(position);
                var gridPos = WarMapManager.Instanse.Grid.Grid.WorldToCell(planePosition);
                if (WarMapManager.Instanse.Grid.IsGridPositionOverWarBase(new Vector2Int(gridPos.x, gridPos.y)))
                {
                    found = true;
                    WarMapManager.Instanse.Grid.SelectWarBase(new Vector2Int(gridPos.x, gridPos.y));
                }

                if (!found)
                {
                    if (WarMapManager.Instanse.selectedWarBase != null)
                    {
                        WarMapManager.Instanse.selectedWarBase.Deselected();
                    }
                }
            }
            else
            {
                if (WarMapManager.Instanse.selectedWarBase != null)
                {
                    var handled = false;
                    foreach (var r in results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")))
                    {
                        handled = true;
                    }

                    if (!handled)
                    {
                        WarMapManager.Instanse.selectedWarBase.Deselected();
                    }
                }
            }
        }

        private void MoveStarted()
        {
            _moveRootBasePosition = _root.position;
            _moveInputBaseScreenPosition = _input.Main.PointerPosition.ReadValue<Vector2>();
            _moveBaseDirection = new Vector2((_camera.orthographicSize * _camera.aspect * 2f) / Screen.width, (_camera.orthographicSize * 2f) / Screen.height);
            _moving = true;
        }

        private void MoveCanceled()
        {
            _moving = false;
        }
        
        private void ZoomStarted()
        {
            _moveRootBasePosition = _root.position;
            var touch0 = _input.Main.TouchPosition0.ReadValue<Vector2>();
            var touch1 = _input.Main.TouchPosition1.ReadValue<Vector2>();
            _zoomPositionOnScreen = Vector2.Lerp(touch0, touch1, 0.5f);
            _zoomPositionInWorld = CameraUtils.CameraScreenPositionToWorldPosition(_zoomPositionOnScreen);
            _zoomBaseValue = _zoom;

            touch0.x /= Screen.width;
            touch1.x /= Screen.width;
            touch0.y /= Screen.height;
            touch1.y /= Screen.height;

            _zoomBaseDistance = Vector2.Distance(touch0, touch1);
            _zooming = true;
            _moving = false;
        }

        private void ZoomCanceled()
        {
            _zooming = false;
        }

        private void Update()
        {
#if UNITY_WEBGL || UNITY_WEBPLAYER
            _input ??= new Controls();
#endif
            
            if (!Input.touchSupported)
            {
                var mouseScroll = _input.Main.MouseScroll.ReadValue<float>();
                switch (mouseScroll)
                {
                    case > 0:
                        _zoom -= _zoomSpeed * Time.deltaTime;
                        break;
                    case < 0:
                        _zoom += _zoomSpeed * Time.deltaTime;
                        break;
                }
            }

            if (_zooming)
            {
                var touch0 = _input.Main.TouchPosition0.ReadValue<Vector2>();
                var touch1 = _input.Main.TouchPosition1.ReadValue<Vector2>();

                touch0.x /= Screen.width;
                touch1.x /= Screen.width;
                touch0.y /= Screen.height;
                touch1.y /= Screen.height;

                var currentDistance = Vector2.Distance(touch0, touch1);
                var deltaDistance = currentDistance - _zoomBaseDistance;
                _zoom = _zoomBaseValue - (deltaDistance * _zoomSpeed);
                
                var zoomCenter = CameraUtils.CameraScreenPositionToWorldPosition(_zoomPositionOnScreen);
                _root.position = _moveRootBasePosition  + (_zoomPositionInWorld - zoomCenter);
            }
            else if (_moving)
            {
                var delta = _input.Main.PointerPosition.ReadValue<Vector2>() - _moveInputBaseScreenPosition;
                _root.position = _moveRootBasePosition - new Vector3(delta.x * _moveBaseDirection.x, delta.y * _moveBaseDirection.y, 0);
            }
            
            AdjustBounds();

            if (_camera.orthographicSize != _zoom)
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoom, _zoomSmooth * Time.deltaTime);
                zoomScale = _camera.orthographicSize / _zoomMax;
            }

            if (_camera.transform.position != _target.position)
            {
                var velocity = Vector3.zero;
                _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _target.position, ref velocity,_moveSmooth * Time.deltaTime);
            }

            if (_camera.transform.rotation != _target.rotation)
            {
                _camera.transform.rotation = _target.rotation;
            }
            CameraUtils.UpdatePlainAnchors();
        }

        private void AdjustBounds()
        {
            if (_zoom < _zoomMin)
            {
                _zoom = _zoomMin;
            }
            if (_zoom > _zoomMax)
            {
                _zoom = _zoomMax;
            }

            var h = (WarMapManager.Instanse.Grid.Up + WarMapManager.Instanse.Grid.Down);
            var w = (WarMapManager.Instanse.Grid.Right + WarMapManager.Instanse.Grid.Left);

            var ch = _zoom * 2f;
            var cw = ch * _camera.aspect;

            if (ch > h)
            {
                _zoom = h / 2f;
            }
            if (cw > w)
            {
                _zoom = (w / _camera.aspect) / 2f;
            }

            ch = _zoom;
            cw = ch * _camera.aspect;

            var position = _root.position;
            if (position.x > WarMapManager.Instanse.Grid.transform.position.x + WarMapManager.Instanse.Grid.Right - cw)
            {
                position.x = WarMapManager.Instanse.Grid.transform.position.x + WarMapManager.Instanse.Grid.Right - cw;
            }
            if (position.x < WarMapManager.Instanse.Grid.transform.position.x - WarMapManager.Instanse.Grid.Left + cw)
            {
                position.x = WarMapManager.Instanse.Grid.transform.position.x - WarMapManager.Instanse.Grid.Left + cw;
            }
            if (position.y > WarMapManager.Instanse.Grid.transform.position.y + WarMapManager.Instanse.Grid.Up - ch)
            {
                position.y = WarMapManager.Instanse.Grid.transform.position.y + WarMapManager.Instanse.Grid.Up - ch;
            }
            if (position.y < WarMapManager.Instanse.Grid.transform.position.y - WarMapManager.Instanse.Grid.Down + ch)
            {
                position.y = WarMapManager.Instanse.Grid.transform.position.y - WarMapManager.Instanse.Grid.Down + ch;
            }
            _root.position = position;
        }
    }
}