using System;
using UnityEngine;

namespace SpriteParticleEmitter
{
    public delegate void SimpleEvent();

    /// <summary>
    /// Obsolete: Use SpriteToParticles component instead - Works as a Base For all emitters defining all common methods and variables
    /// </summary>
    //[Obsolete("Use SpriteToParticles component instead")]
    [SerializeField]
    public abstract class EmitterBase : MonoBehaviour
    {
        //! Should show warnings and errors?
        public bool verboseDebug = false;

        [Header("References")]
        [Tooltip("If none is provided the script will look for one in this game object.")]
        //! If none is provided the script will look for one in this game object.
        public SpriteRenderer spriteRenderer;
        [Tooltip("If none is provided the script will look for one in this game object.")]
        //! If none is provided the script will look for one in this game object.
        public ParticleSystem particlesSystem;

        [Header("Emission Options")]
        [Tooltip("Start emitting as soon as able. (On static emission activating this will force CacheOnAwake)")]
        //! Activating this will force CacheOnAwake
        public bool PlayOnAwake = true;
        [Tooltip("Particles to emit per second")]
        //! Particles to emit per second
        public float EmissionRate = 1000;
        [Tooltip("Should new particles override ParticleSystem's startColor and use the color in the pixel they're emitting from?")]
        //! Should new particles override ParticleSystem's startColor and use the color in the pixel they're emitting from?
        public bool UsePixelSourceColor;
        public enum BorderEmission
        {
            Off,
            Fast,
            Precise
        }
        public BorderEmission borderEmission;

        [Space(10)]
        //! Activating this will make the Emitter only emit from selected color.
        public bool UseEmissionFromColor = false;
        [Tooltip("Emission will take this color as only source position")]
        //! Emission will take this color as only source position.
        public Color EmitFromColor;
        [Range(0.01f, 1)]
        [Tooltip("In conjunction with EmitFromColor. Defines how much can it deviate from red spectrum for selected color.")]
        //! In conjunction with EmitFromColor. Defines how much can it deviate from red spectrum for selected color.
        public float RedTolerance = 0.05f;
        [Range(0f, 1f)]
        [Tooltip("In conjunction with EmitFromColor. Defines how much can it deviate from green spectrum for selected color.")]
        //! In conjunction with EmitFromColor. Defines how much can it deviate from green spectrum for selected color.
        public float GreenTolerance = 0.05f;
        [Range(0f, 1f)]
        [Tooltip("In conjunction with EmitFromColor. Defines how much can it deviate from blue spectrum for selected color.")]
        //! In conjunction with EmitFromColor. Defines how much can it deviate from blue spectrum for selected color.
        public float BlueTolerance = 0.05f;

        //! Must match Particle System's same option
        protected ParticleSystemSimulationSpace SimulationSpace;

        //! is the system playing
        protected bool isPlaying;

#if UNITY_5_5_OR_NEWER
        protected ParticleSystem.MainModule mainModule;
#endif

#if UNITY_EDITOR
        protected Sprite cachedSprite;
#endif
        [Header("Advanced")]
        [Tooltip("This will save memory size when dealing with same sprite being loaded repeatedly by different GameObjects.")]
        //! This will save memory size when dealing with same sprite being loaded repeatedly by different GameObjects.
        public bool useSpritesSharingCache;

        public bool useBetweenFramesPrecision;
        protected Vector3 lastTransformPosition;

        //! Save time to know how many particles to show per frame
        protected float ParticlesToEmitThisFrame;

        //public enum RenderSystemUsing
        //{
        //    SpriteRenderer,
        //    ImageRenderer,
        //}
        //
        //protected RenderSystemUsing renderSystemUsing;
        /// <summary>
        /// Obtain needed references and define base variables.
        /// </summary>
        protected virtual void Awake()
        {
            //Find Renderer in current gameObject if non is draggued
            if (!spriteRenderer)
            {
                if (verboseDebug)
                    Debug.LogWarning("Sprite Renderer not defined, trying to find in same GameObject");
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (!spriteRenderer)
                {
                    if (verboseDebug)
                        Debug.LogWarning("Sprite Renderer not found");
                }
            }

            //Find Particle System in current gameObject if non is draggued
            if (!particlesSystem)
            {
                particlesSystem = GetComponent<ParticleSystem>();
                if (!particlesSystem)
                {
                    if (verboseDebug)
                        Debug.LogError("No particle system found. Static Sprite Emission won't work");
                    return;
                }
            }

            //Set base varibles in the system for this emitter work as expected
            #if UNITY_5_5_OR_NEWER
            mainModule = particlesSystem.main;
            mainModule.loop = false;
            mainModule.playOnAwake = false;
            particlesSystem.Stop();
            //validate simulation Space
            SimulationSpace = mainModule.simulationSpace;
            #else
                particlesSystem.loop = false;
                particlesSystem.playOnAwake = false;
                particlesSystem.Stop();
                //validate simulation Space
                SimulationSpace = particlesSystem.simulationSpace;
            #endif
        }

        #region Abstract Methods
        /// <summary>
        /// Works as Shuryken Particle System's Play() method
        /// </summary>
        public abstract void Play();
        /// <summary>
        /// Works as Shuryken Particle System's Pause() method
        /// </summary>
        public abstract void Pause();
        /// <summary>
        /// Works as Shuryken Particle System's Stop() method
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// Is the system being played?
        /// </summary>
        public abstract bool IsPlaying();
        /// <summary>
        /// Is the system available to be played? Different emitters will have different conditions.
        /// </summary>
        public abstract bool IsAvailableToPlay();
        #endregion

        //! Event will be called when Sprite Cache as ended
        public virtual event SimpleEvent OnCacheEnded;
        //! Event will be called when the system is available to be played
        public virtual event SimpleEvent OnAvailableToPlay;

        private void DummyMethod()
        {
            if (OnAvailableToPlay != null)
                OnAvailableToPlay();

            if (OnCacheEnded != null)
                OnCacheEnded();
        }
    }
}
