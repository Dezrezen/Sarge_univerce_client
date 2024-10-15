using UnityEngine;

public class Shooter : MonoBehaviour
{
    //soldiers shoot at targets using this

    public GameObject[] projectile;

    public int assignedToGroup;
    public float fireRate;
    public bool shoot;
    public string projectileType = "Star";
    private readonly int projectileZ = -11;


    private float fireCounter;

    private Component soundFx;

    public FighterController FighterController { get; set; }

    private void Start()
    {
        soundFx = GameObject.Find("SoundFX").GetComponent<SoundFX>();
        FighterController = GetComponent<FighterController>();
    }

    private void FixedUpdate()
    {
        if (shoot)
        {
            fireCounter += Time.deltaTime;

            if (fireCounter > fireRate)
            {
                if (projectileType == "Star") ((SoundFX)soundFx).SoldierFire();

                var guidedProjectile =
                    Instantiate(projectile[assignedToGroup],
                            new Vector3(transform.position.x, transform.position.y, projectileZ), Quaternion.identity)
                        .GetComponent<GuidedProjectile>();

                guidedProjectile.Target = new Vector3(FighterController.GrassTarget.transform.position.x,
                    FighterController.GrassTarget.transform.position.y, 2);

                fireCounter = 0;

                Helios.Instance.SetDamageForStructure(FighterController.GrassTarget.StructureIndex,
                    FighterController.DamagePoints, this);
            }
        }
    }
}