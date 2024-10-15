using UnityEngine;

namespace Utils
{
    public static class CameraUtils
    {
        private static Camera _camera;
        public static Vector3 plainDownLeft = Vector3.zero;
        public static Vector3 plainTopRight = Vector3.zero;

        public static void Init(Camera camera)
        {
            _camera = camera;
        }
        
        public static Vector3 CameraScreenPositionToWorldPosition(Vector2 position)
        {
            var h = _camera.orthographicSize * 2f;
            var w = _camera.aspect * h;

            var anchor = _camera.transform.position - 
                         _camera.transform.right.normalized * w / 2f -
                         _camera.transform.up.normalized * h / 2f;

            var world = anchor + 
                        _camera.transform.right.normalized * position.x / Screen.width * w + 
                        _camera.transform.up.normalized * position.y / Screen.height * h;
            world.z = 0;
            return world;
        }

        public static void UpdatePlainAnchors()
        {
            plainDownLeft = CameraScreenPositionToWorldPosition(Vector2.zero);
            plainTopRight = CameraScreenPositionToWorldPosition(new Vector2(Screen.width, Screen.height));
        }
    }
}