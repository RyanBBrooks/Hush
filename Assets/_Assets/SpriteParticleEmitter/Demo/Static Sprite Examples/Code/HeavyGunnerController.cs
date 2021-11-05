using UnityEngine;
using System.Collections.Generic;
using SpriteToParticlesAsset;

public class HeavyGunnerController : MonoBehaviour
{
    public List<SpriteToParticles> ShadowFxs;
    public List<SpriteToParticles> WeirdFxs;

    public SpriteToParticles GunPrep;

    public float Speed = 20;
    public GameObject LookAtAim;
    public float RotationVelocity = 5;
    private float wantedRotation;
    public float angleDisplacement = 180;
    public Rigidbody2D rig;
    private Animator animator;
    private float ShootPrepTime;

    public GameObject BulletPrefab;
    public Transform BulletStartPos;
    public float bulletSpeed = 20;
    public float bulletRotationSpeed = 20;

	// Use this for initialization
	void Start () 
    {
        rig = GetComponent<Rigidbody2D>();
	    animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update() 
    {
        float ver = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        Vector2 move = new Vector2(hor, ver);

        if (move.magnitude > 1)
            move.Normalize();

        rig.velocity = new Vector3(move.x * Speed, move.y * Speed, 0);

        animator.SetFloat("Speed" , rig.velocity.magnitude);

        //Rotation
        float AngleRad = Mathf.Atan2(LookAtAim.transform.position.y - transform.position.y, LookAtAim.transform.position.x - transform.position.x);
        wantedRotation = (180 / Mathf.PI) * AngleRad + angleDisplacement;

        Quaternion wanted = Quaternion.Euler(0, 0, wantedRotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, wanted, RotationVelocity * Time.deltaTime);

        if (Input.GetMouseButton(0))
            ShootPrep();

        if (Input.GetMouseButtonUp(0))
            Shoot();

        if (Input.GetKeyDown(KeyCode.Z))
            ShadowFXToggle();

        if (Input.GetKeyDown(KeyCode.X))
            WeirdFXToggle();
	}

    public void ShootPrep()
    {
        ShootPrepTime += Time.deltaTime;
        if (ShootPrepTime > 0.1f)
        {
            if (!GunPrep.IsPlaying())
                GunPrep.Play();

            GunPrep.EmissionRate = ShootPrepTime*1000;
            if (GunPrep.EmissionRate > 10000)
                GunPrep.EmissionRate = 10000;
        }
    }

    public void Shoot()
    {
        animator.SetTrigger("Shoot");
        ShootPrepTime = 0;
        GunPrep.Stop();
        GameObject newBullet = Instantiate(BulletPrefab);
        newBullet.transform.position = BulletStartPos.position;
        newBullet.GetComponent<Rigidbody2D>().AddForce(transform.up * bulletSpeed);
        newBullet.GetComponent<Rigidbody2D>().AddTorque(bulletRotationSpeed);
        Destroy(newBullet, 10);
    }

    public void ShadowFXToggle()
    {
        if (ShadowFxs[0].IsPlaying())
        {
            foreach (SpriteToParticles emitter in ShadowFxs)
            {
                emitter.Stop();
            }
        }
        else
        {
            foreach (SpriteToParticles emitter in ShadowFxs)
            {
                emitter.Play();
            }
        }

    }

    public void WeirdFXToggle()
    {
        if (WeirdFxs[0].IsPlaying())
        {
            foreach (SpriteToParticles emitter in WeirdFxs)
            {
                emitter.Stop();
            }
        }
        else
        {
            foreach (SpriteToParticles emitter in WeirdFxs)
            {
                emitter.Play();
            }
        }
    }

}
