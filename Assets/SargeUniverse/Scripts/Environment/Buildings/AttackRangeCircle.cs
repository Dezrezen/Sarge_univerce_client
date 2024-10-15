using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    [RequireComponent(typeof(LineRenderer))]
    public class AttackRangeCircle : MonoBehaviour
    {
        [Range(0,100)]
        public int segments = 50;
        [Range(0,5)]
        public float xradius = 5;
        [Range(0,5)]
        public float yradius = 5;
        public float size = 1f;
        
        LineRenderer line;

        void Start ()
        {
            line = gameObject.GetComponent<LineRenderer>();

            line.SetWidth(0.4f, 0.4f);
            line.SetVertexCount (segments + 1);
            line.useWorldSpace = false;
            CreatePoints ();
            line.startWidth = size;
            line.endWidth = size;
        }

        void CreatePoints ()
        {
            float x;
            float y;
            float z;

            float angle = 20f;

            for (int i = 0; i < (segments + 1); i++)
            {
                x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
                y = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;

                line.SetPosition (i,new Vector3(x,y,0) );

                angle += (360f / segments);
            }
        }
    }
}