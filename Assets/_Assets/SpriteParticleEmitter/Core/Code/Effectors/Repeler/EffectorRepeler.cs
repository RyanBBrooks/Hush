using UnityEngine;

namespace SpriteToParticlesAsset
{
/// <summary>
/// A constant effect over the emitted particles repelling them from a wanted Transform.
/// </summary>
[ExecuteInEditMode]
public class EffectorRepeler : MonoBehaviour
{
    [Tooltip("Repeler force intensity. A negative strength will attract particles instead of repeling them.")]
    //! Repeler force intensity. A negative strength will attract particles instead of repeling them.
    public float strength = 1;

    [Tooltip("Transform at which the particles will repel from. If none is set it will use the current Sprite position.")]
    //! Transform at which the particles will repel from. If none is set it will use the current Sprite position.
    public Transform repelerCenter;

    //! Reference to current StP
    private SpriteToParticles emitter;
    //! Reference to current StP's Particle System
    private ParticleSystem ps;
    //! The array to be fed by Particle System's GetParticles() method
    private ParticleSystem.Particle[] particles;
    //! Stores Repeler position
    private Vector3 center;

    /// <summary>
    /// Get needed references and set the repeler center as the Sprite's position if none is suplied.
    /// </summary>
    private void Awake()
    {
        emitter = GetComponent<SpriteToParticles>();
        if (emitter && emitter.particlesSystem)
            ps = emitter.particlesSystem;
        if (!repelerCenter)
            repelerCenter = transform;
    }

    /// <summary>
    /// Method for changing the repeler Transform at runtime
    /// </summary>
    /// <param name="repeler">The repeler Transform</param>
    public void SetRepelCenterTransform(Transform repeler)
    {
        repelerCenter = repeler;
    }

    /// <summary>
    /// Main process for the repelling effect.
    /// </summary>
    private void LateUpdate()
    {
        //check that references are ok
        if (!ps)
        {
            if (!emitter || !emitter.particlesSystem)
                return;
            ps = emitter.particlesSystem;
        }
        //Define the array size to be fed with emitted particles.
        if (particles == null || particles.Length < ps.particleCount)
            particles = new ParticleSystem.Particle[ps.particleCount];

        //Feed particles array based on emitted particles.
        int particlesCount = ps.GetParticles(particles);

        // Set the repeler center based on ParticleSystems's simulation space.
        bool localOrWorld;
        #if UNITY_5_5_OR_NEWER
        localOrWorld = ps.main.simulationSpace == ParticleSystemSimulationSpace.Local;
        #else
        localOrWorld = ps.simulationSpace == ParticleSystemSimulationSpace.Local;
        #endif

        if (localOrWorld)
            center = repelerCenter.localPosition;
        else
            center = repelerCenter.position;

        Vector3 dir;

        //Go over all particles
        for (int i = 0; i < particlesCount; i++)
        {
            dir = particles[i].position - center;
            //Apply repel force
            particles[i].velocity = dir*strength;
        }

        //Finally set the processed particles back to the particle system.
        ps.SetParticles(particles, particlesCount);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorInvalidate();
    }

    /// <summary>
    /// Editor method for continuous modification.
    /// </summary>
    private void EditorInvalidate()
    {
        //Debug.Log("EditorInvalidate");
        Awake();
    }
#endif
}
}