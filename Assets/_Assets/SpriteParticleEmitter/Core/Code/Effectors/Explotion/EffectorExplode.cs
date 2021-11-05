using UnityEngine;

namespace SpriteToParticlesAsset
{
/// <summary>
/// A one time effect that will make a Sprite explode based on some parameters
/// </summary>
public class EffectorExplode : MonoBehaviour
{
    [Tooltip("Weather the system is being used for Sprite or Image component. ")]
    //! Weather the system is being used for Sprite or Image component. 
    public float destroyObjectAfterExplosionIn = 10;

    //! Reference to current StP
    private SpriteToParticles emitter;
    //! Reference to current StP's Particle System
    private ParticleSystem ps;
    //! The array to be fed by Particle System's GetParticles() method
    private ParticleSystem.Particle[] particles;

    [HideInInspector]
    //! Whether the sprite has already exploded.
    public bool exploded = false;

    /// <summary>
    /// Get needed references
    /// </summary>
    private void Awake()
    {
        emitter = GetComponent<SpriteToParticles>();
        if (emitter && emitter.particlesSystem)
            ps = emitter.particlesSystem;

        //Invoke("ExplodeTest", 2);
    }

    /// <summary>
    /// Make the Sprite Explode using the StP component based on an arc as param. Check Effector - Explode scene.
    /// </summary>
    /// <param name="sourcePos"> Explotion source position</param>
    /// <param name="radius">Explosion radius</param>
    /// <param name="angle">Explosion angle</param>
    /// <param name="startRot">Explosion start rotation, work in conjuntion with angle.</param>
    /// <param name="strenght">Explosion strenght</param>
    public void ExplodeAt(Vector3 sourcePos, float radius, float angle, float startRot, float strenght)
    {
        //check that references are ok
        if (!ps)
        {
            if (!emitter || !emitter.particlesSystem)
                return;
            ps = emitter.particlesSystem;
        }

        //Use StP's EmitAll to create one particle per pixel in the sprite, also defining that the sprite disappear.
        emitter.EmitAll(true);

        //Define the array size to be fed with emitted particles.
        if (particles == null || particles.Length < ps.particleCount)
            particles = new ParticleSystem.Particle[ps.particleCount];

        //Feed particles array based on emitted particles.
        int particlesCount = ps.GetParticles(particles);

        float halfRadius = radius/2;

        //Get arcs right direction.
        Vector2 right = new Vector2(Mathf.Cos(Mathf.Deg2Rad*startRot), Mathf.Sin(Mathf.Deg2Rad*startRot));

        //Go over all particles
        for (int i = 0; i < particlesCount; i++)
        {
            ParticleSystem.Particle p = particles[i];

            float dist = Vector3.Distance(sourcePos, p.position);
            
            //if the particle position is inside the explosion radius.
            if (dist < halfRadius)
            {
                Vector3 dir = p.position - sourcePos;

                //Get the real angle between the particle position and the arc set as paramenter.
                float angleBtw = Vector3.Angle(right, dir);
                Vector3 cross = Vector3.Cross(right, dir);
                if (cross.z < 0)
                {
                    angleBtw = 360 - angleBtw;
                }

                //The particles is inside the radius of explosion, is it inside the arc of explosion? 
                if (angleBtw < angle)
                {
                    dir.Normalize();
                    float val = radius - dist;
                    //Add noise to explosion
                    float rand = Random.Range(val/2, val);
                    
                    //Apply explosion velocity
                    p.velocity += dir*(rand)*strenght;
                    particles[i] = p;
                }
            }
        }

        //Finally set the processed particles back to the particle system.
        ps.SetParticles(particles, particlesCount);
        exploded = true;

        //If this gameobject is set to autodestruct (destroyObjectAfterExplosionIn is negative) invoke Destroy.
        if (destroyObjectAfterExplosionIn >= 0)
            Destroy(gameObject, destroyObjectAfterExplosionIn);
    }

    public void ExplodeTest()
    {
        ExplodeAt(transform.position, 10, 360, 0, 2);
    }
}
}