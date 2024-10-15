using UnityEngine;

public class GuidedProjectile : MonoBehaviour
{
    //colored projectiles coming from attacking units towards the center of the building

    //instead of processing who's firing them, 4 prefabs would be better- too many projectiles, wasted performance

    public int assignedToGroup; //keeps track of which unit group fires this
    public GameObject sparks;

    public float projectileSpeed;


    public Vector2 velocity;


    public Vector3 Target { get; set; } = new();

    private void Start()
    {
        transform.parent = GameObject.Find("GroupEffects").transform;
    }

    private void FixedUpdate()
    {
        FireSeq();
    }

    private void Explode()
    {
        var Sparks = Instantiate(sparks, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void FireSeq()
    {
        velocity += Steer(Target);
    }

    private Vector2 Steer(Vector2 target)
    {
        var pos = new Vector2(transform.position.x, transform.position.y);
        var desiredVelocity = target - pos;
        var dist = desiredVelocity.magnitude; //square root of (x*x+y*y+z*z)

        desiredVelocity.Normalize();

        if (dist < 10) Explode();

        desiredVelocity *= projectileSpeed;

        var acceleration = desiredVelocity - velocity;
        transform.position += new Vector3(velocity.x, velocity.y, 0);

        return acceleration;
    }
}