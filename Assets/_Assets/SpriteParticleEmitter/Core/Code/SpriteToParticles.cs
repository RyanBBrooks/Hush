using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Serialization;
using UnityEngine.UI;
using SpriteToParticlesAsset;

namespace SpriteToParticlesAsset
{
[ExecuteInEditMode]
public class SpriteToParticles : MonoBehaviour
{
    #region Definitions
    [Tooltip("Weather the system is being used for Sprite or Image component. ")]
    //! Weather the system is being used for Sprite or Image component.
    public RenderSystemUsing renderSystemType = RenderSystemUsing.SpriteRenderer;

    [Tooltip("Weather the system is using static or dynamic mode.")]
    //! Weather the system is using static or dynamic mode.
    public SpriteMode mode = SpriteMode.Static;

    [Tooltip("Should log warnings and errors?")]
    //! Should log warnings and errors?
    public bool verboseDebug = false;

    [Tooltip("If none is provided the script will look for one in this game object.")]
    //! If none is provided the script will look for one in this game object.
    public SpriteRenderer spriteRenderer;

    [Tooltip("Must be provided by other GameObject's ImageRenderer.")]
    //! Must be provided by other GameObject's ImageRenderer.
    public Image imageRenderer;

    [Tooltip("If none is provided the script will look for one in this game object.")]
    //! If none is provided the script will look for one in this game object.
    public ParticleSystem particlesSystem;

    [Tooltip("Start emitting as soon as able. (On static emission activating this will force CacheOnAwake)")]
    //! Start emitting as soon as able. (On static emission activating this will force CacheOnAwake)
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
    [Tooltip("Emit from sprite border. Fast will work on the x axis only. Precise works on both x and y axis but is more performance heavy. (Border emission only works in dynamic mode currently)")]
    //! Emit from sprite border. Fast will work on the x axis only. Precise works on both x and y axis but is more performance heavy. (Border emission only works in dynamic mode currently)
    public BorderEmission borderEmission;

    [Tooltip("Activating this will make the Emitter only emit from selected color")]
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

    [Tooltip("This will save memory size when dealing with same sprite being loaded repeatedly by different GameObjects.")]
    //! This will save memory size when dealing with same sprite being loaded repeatedly by different GameObjects.
    public bool useSpritesSharingPool;

    [Tooltip("Weather use BetweenFrames precision or not. (Refer to manual for further explanation)")]
    //! Weather use Between Frames precision or not.
    public bool useBetweenFramesPrecision;

    [Tooltip("Should the system cache sprites data? (Refer to manual for further explanation)")]
    //! Should the system cache sprites data? (Refer to manual for further explanation)
    public bool CacheSprites = true;

    [Tooltip("Should the transform match target Renderer GameObject Position? (For Image Component(UI) StP Object must have same parent as the Renderer Image component Transform)")]
    //! Should the transform match target Renderer GameObject Position? (For Image Component(UI) StP Object must have same parent as the Renderer Image component Transform)
    [FormerlySerializedAs("matchImageRendererPostionData")]
    public bool matchTargetGOPostionData;

    [Tooltip("Should the transform match target Renderer Renderer Scale? (For Image Component(UI) StP Object must have same parent as the Image component Transform. For Sprite Component it will match local scale data)")]
    //! Should the transform match target Renderer Renderer Scale? (For Image Component(UI) StP Object must have same parent as the Image component Transform. For Sprite Component it will match local scale data)
    [FormerlySerializedAs("matchImageRendererScale")]
    public bool matchTargetGOScale;


    //! Must match Particle System's same option
    private ParticleSystemSimulationSpace SimulationSpace;
    //! is the system playing
    private bool isPlaying;
    //! The component in charge of rendering the particles in the UI.
    public UIParticleRenderer uiParticleSystem;
    #if UNITY_5_5_OR_NEWER
    //! Particle System's main module reference.
    private ParticleSystem.MainModule mainModule;
    #endif

#if UNITY_EDITOR
    //! Edit variable for checking when sprite has changed in editor. Only used in static emission mode.
    private Sprite cachedSprite;
#endif
    //! Save time to know how many particles to show per frame
    private float ParticlesToEmitThisFrame;
    //! Save last position to be able to interpolate particle emission position between last and current frame. (Refer to manual for further explanation)
    private Vector3 lastTransformPosition;

    //! Save Sprite component transform reference in case it's in a different GO than the StP component.
    private Transform spriteTransformReference;

    #region Dynamic mode Specifics
    //! Emiting from color needs to cycle all pixels in the sprite to know where the color is and later emition needs to randomize the emitting position so a look up table cache is needed
    //! Made it private but not local to the scope for reusing in next frames. (Only used in dynamic mode)
    private Color[] colorCache = new Color[1];
    //! Emiting from color needs to cycle all pixels in the sprite to know where the color is and later emition needs to randomize the emitting position so a look up table cache is needed
    //! Made it private but not local to the scope for reusing in next frames. (Only used in dynamic mode)
    private int[] indexCache = new int[1];
    //! Dictionary containing all sprites data so far for not asking texture.GetPixels() every frame, which would be slow. (Only used in dynamic mode)
    private Dictionary<Sprite, Color[]> spritesSoFar = new Dictionary<Sprite, Color[]>();
    #endregion

    #region UI Specifics
    //! The target Image Renderer's RectTransform. (Only used in UI render mode)
    private RectTransform targetRectTransform;
    //! This RectTransform.  (Only used in UI render mode)
    private RectTransform currentRectTransform;
    //! Used to calculate position multipliers based on pixels per unit. (Only used in UI render mode)
    private Vector2 offsetXY;
    //! Multiplier used with texture's Pixels per unit. (Only used in UI render mode)
    private float wMult = 100;
    //! Multiplier used with texture's Pixels per unit. (Only used in UI render mode)
    private float hMult = 100;
    #endregion

    #region Static Mode Specifics
    [Tooltip("Should the system cache on Awake method? - Static emission needs to be cached first, if this property is not checked the CacheSprite() method should be called by code. (Refer to manual for further explanation)")]
    //! Should the system cache on Awake method? - Static emission needs to be cached first, if this property is not checked the CacheSprite() method should be called by code. (Refer to manual for further explanation)
    public bool CacheOnAwake = true;
    //! is cached ready to start emission? (Only used in static mode)
    private bool hasCachingEnded;
    //! Processed particles count. (Only used in static mode)
    private int particlesCacheCount;
    //! The size the particles will start with. (Only used in static mode)
    private float particleStartSize;
    //! Save data as an array for better access performance. (Only used in static mode)
    private Vector3[] particleInitPositionsCache;
    //! Save data as an array for better access performance. (Only used in static mode)
    private Color[] particleInitColorCache;
    //! Event will be called when Sprite Cache as ended. (Static mode only)
    public event SimpleEvent OnCacheEnded;
    //! Event will be called when the system is available to be played. (Static mode only)
    public event SimpleEvent OnAvailableToPlay;
    #endregion

    #endregion

    #region Awake
    protected virtual void Awake()
    {
        //reset values just to be sure editor hasn't save data when switching modes (dynamic and static)
        spritesSoFar = new Dictionary<Sprite, Color[]>();
        colorCache = new Color[1];
        indexCache = new int[1];
        particleInitPositionsCache = null;
        particleInitColorCache = null;

        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            //Find Renderer in current gameObject if non is draggued
            if (!spriteRenderer)
            {
                if (verboseDebug)
                    Debug.LogWarning("Sprite Renderer not defined, trying to find in same GameObject. ");
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (!spriteRenderer)
                {
                    if (verboseDebug)
                        Debug.LogWarning("Sprite Renderer not found");
                }
            }
            if (spriteRenderer)
            {
                spriteTransformReference = spriteRenderer.gameObject.transform;
                lastTransformPosition = spriteTransformReference.position;
            }
        }
        else
        {
            uiParticleSystem = GetComponent<UIParticleRenderer>();
            if (!uiParticleSystem)
            {
                if (verboseDebug)
                    Debug.LogWarning("UIParticleRenderer couldn't be found, component must be added in order for the system to work. ");
                isPlaying = false;
                return;
            }

            //Find Renderer in current gameObject if non is draggued
            if (!imageRenderer)
            {
                if (verboseDebug)
                    Debug.LogWarning("Image Renderer not defined, must be defined in order for the system to work. ");
                isPlaying = false;
                return;
            }
        }

        //Find Particle System in current gameObject if non is draggued
        if (!particlesSystem)
        {
            particlesSystem = GetComponent<ParticleSystem>();
            if (!particlesSystem)
            {
                if (verboseDebug)
                    Debug.LogError("No particle system found. Static Sprite Emission won't work. ");
                isPlaying = false;
                return;
            }
        }

        //Set base varibles in the system for this emitter work as expected
        #if UNITY_5_5_OR_NEWER
            mainModule = particlesSystem.main;
            #if UNITY_2017_2_OR_NEWER
                //Hack needed beyond Unity 2017.2 to be sure all things are properly initialized
                mainModule.loop = true;
                mainModule.playOnAwake = true;
            #else
                mainModule.loop = false;
                mainModule.playOnAwake = false;
            #endif
            particlesSystem.Stop();
            //validate simulation Space
            SimulationSpace = mainModule.simulationSpace;
        #else
            SimulationSpace = particlesSystem.simulationSpace;
        #endif

        if (PlayOnAwake)
        {
            isPlaying = true;
            particlesSystem.Emit(1);
            particlesSystem.Clear();
#if UNITY_2017_2_OR_NEWER
            if (Application.isPlaying)
                particlesSystem.Play();
#endif
        }

        if (renderSystemType == RenderSystemUsing.ImageRenderer)
        {
            currentRectTransform = GetComponent<RectTransform>();
            targetRectTransform = imageRenderer.GetComponent<RectTransform>();
        }

        #if UNITY_5_5_OR_NEWER
            // Set particle system's emission to at least the amount expected by the system
            if (mainModule.maxParticles < EmissionRate)
                mainModule.maxParticles = Mathf.CeilToInt(EmissionRate);
        #else
            if (particlesSystem.maxParticles < EmissionRate)
                particlesSystem.maxParticles = Mathf.CeilToInt(EmissionRate);
        #endif

        // Make sure to cache sprite data before emitting (Only in static mode)
        if (mode == SpriteMode.Static)
        {
            if (PlayOnAwake)
                CacheOnAwake = true;

            if (CacheOnAwake)
                CacheSprite();
        }
    }
    #endregion
    
    /// <summary>
    /// When playing it emits particles based on EmissionRate.
    /// </summary>
    public void Update()
    {
        bool mustEmit = isPlaying;
        if (mode == SpriteMode.Static)
            mustEmit = isPlaying && hasCachingEnded;

        if (!mustEmit)
            return;

#if UNITY_EDITOR
        //if cached sprite is different than the one beign used, remake all initial definitions
        if (mode == SpriteMode.Static && cachedSprite != GetSprite())
            EditorInvalidate();
#endif

        // handle further calculations when dealing with UI render mode.
        if (renderSystemType == RenderSystemUsing.ImageRenderer)
        {
            HandlePositionAndScaleForUI();
            // stop here if something has gone wrong
            if (!isPlaying)
                return;
        }
        else
        {
            HandlePosition();
        }

        ParticlesToEmitThisFrame += EmissionRate * Time.deltaTime;
        int EmissionCount = (int)ParticlesToEmitThisFrame;
        //don't even call the method if no particle would be emitted
        if (EmissionCount > 0)
            Emit(EmissionCount);
        //sustract integer particles emitted and leave the float bit
        ParticlesToEmitThisFrame -= EmissionCount;

        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            if (!spriteTransformReference)
                spriteTransformReference = spriteRenderer.transform;
            lastTransformPosition = spriteTransformReference.position;
        }
    }

    #region Public methods

    /// <summary>
    /// Emit the amount of particles this frame
    /// </summary>
    /// <param name="emitCount">Amount of particles to emit</param>
    public void Emit(int emitCount)
    {
        HackUnityCrash2017();

        if (mode == SpriteMode.Dynamic)
            EmitDynamic(emitCount);
        else
            EmitStatic(emitCount);
    }

    /// <summary>
    /// Will emit one particle from every pixel in the sprite, or from every pixel in the found color if UseEmissionFromColor is set to true
    /// </summary>
    /// <param name="hideSprite">Must it disable referenced spriteRenderer</param>
    public void EmitAll(bool hideSprite = true)
    {
        if (hideSprite)
        {
            if (renderSystemType == RenderSystemUsing.SpriteRenderer)
                spriteRenderer.enabled = false;
            else
                imageRenderer.enabled = false;
        }

        if (mode == SpriteMode.Dynamic)
            EmitAllDynamic();
        else
            EmitAllStatic();
    }

    /// <summary>
    /// Enable spriteRenderer if it was disabled.
    /// </summary>
    public void RestoreSprite()
    {
        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
            spriteRenderer.enabled = true;
    }

    /// <summary>
    /// Works as Shuryken Particle System's Play() method
    /// </summary>
    public void Play()
    {
        if (!isPlaying)
            particlesSystem.Play();
        isPlaying = true;
    }

    /// <summary>
    /// Works as Shuryken Particle System's Pause() method
    /// </summary>
    public void Pause()
    {
        if (isPlaying)
            particlesSystem.Pause();
        isPlaying = false;
    }

    /// <summary>
    /// Works as Shuryken Particle System's Stop() method
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
    }

    /// <summary>
    /// Is the system being played?
    /// </summary>
    public bool IsPlaying()
    {
        return isPlaying;
    }

    /// <summary>
    /// Is the system available to be played? Different emitters will have different conditions.
    /// </summary>
    public bool IsAvailableToPlay()
    {
        return mode != SpriteMode.Static || hasCachingEnded;
    }

    /// <summary>
    /// Clears the sprites cache, useful for releasing memory if needed.
    /// </summary>
    public void ClearCachedSprites()
    {
        spritesSoFar = new Dictionary<Sprite, Color[]>();
    }

    #endregion

    /// <summary>
    /// Handle further calculations before emission when dealing with UI render mode.
    /// </summary>
    void HandlePositionAndScaleForUI()
    {
        if (mode == SpriteMode.Dynamic)
            ProcessPositionAndScaleDynamic();
        else
            ProcessPositionAndScaleStatic();
    }

    /// <summary>
    /// Set expected position based on options.
    /// </summary>
    void HandlePosition()
    {
        if (matchTargetGOPostionData && spriteRenderer!=null)
            transform.position = spriteRenderer.transform.position;
        if (matchTargetGOScale && spriteRenderer != null)
            transform.localScale = spriteRenderer.transform.lossyScale;
    }

    #region Dynamic

    /// <summary>
    /// Handle further calculations before emission when dealing with UI render mode.
    /// </summary>
    void ProcessPositionAndScaleDynamic()
    {
        if (imageRenderer == null)
        {
            if (verboseDebug)
                Debug.LogError("Image Renderer component not referenced in DynamicEmitterUI component");
            isPlaying = false;
            return;
        }

#if UNITY_EDITOR
        if (currentRectTransform == null)
        {
            Awake();
            return;
        }
#endif

        //match current RectTransform's data with target RectTransform
        if (matchTargetGOPostionData)
            currentRectTransform.position = new Vector3(targetRectTransform.position.x,
                targetRectTransform.position.y, targetRectTransform.position.z);
        currentRectTransform.pivot = targetRectTransform.pivot;
        if (matchTargetGOPostionData)
        {
            currentRectTransform.anchoredPosition = targetRectTransform.anchoredPosition;
            currentRectTransform.anchorMin = targetRectTransform.anchorMin;
            currentRectTransform.anchorMax = targetRectTransform.anchorMax;
            currentRectTransform.offsetMin = targetRectTransform.offsetMin;
            currentRectTransform.offsetMax = targetRectTransform.offsetMax;
        }
        if (matchTargetGOScale)
            currentRectTransform.localScale = targetRectTransform.localScale;
        currentRectTransform.rotation = targetRectTransform.rotation;
        currentRectTransform.sizeDelta = new Vector2(targetRectTransform.rect.width, targetRectTransform.rect.height);

        //Calculate position multipliers based on pixels per unit
        float offsetX = (1 - targetRectTransform.pivot.x)*(targetRectTransform.rect.width) -
                        targetRectTransform.rect.width/2;
        float offsetY = (1 - targetRectTransform.pivot.y)*(-targetRectTransform.rect.height) +
                        targetRectTransform.rect.height/2;
        offsetXY = new Vector2(offsetX, offsetY);
        Sprite sprite = GetSprite();

        //This code was part was added thanks to Zachary Petriw who came across the aspect ratio bug and kindly send me the fix:)
        // If the target image has "PreserveAspect" enabled then we need to figure out which dimension got shorter. 
        if (imageRenderer.preserveAspect)
        {
            // Get the aspect ratio of the sprite here as defined by aspect = height / width. Keep this definition consistent for now. 
            // Remember that if the sprite aspect is larger than the target rect, the sprite was shrunken horizontally and the 'wMult' must be made smaller.
            // If the sprite aspect is smaller than the target rect, the sprite was shrunken vertically and the 'hMult' must be made smaller. 
            // This is based on the above definition that a = h/w. 
            float spriteAspect = sprite.rect.size.y / sprite.rect.size.x;
            float targetAspect = targetRectTransform.rect.height / targetRectTransform.rect.width;
        
            // If this sprite aspect is larger than the target RectTransform then we must scale the x-scale accordingly or the particles will be emitted too far out horizontally. 
            if (spriteAspect > targetAspect)
            {
                //Debug.Log("[DynamicEmitterUI] Sprite aspect larger - targetRectTransform aspect: " + targetAspect + ", sprite aspect: " + spriteAspect);
                wMult = sprite.pixelsPerUnit * (targetRectTransform.rect.width / sprite.rect.size.x) * (targetAspect / spriteAspect);
                hMult = sprite.pixelsPerUnit * (targetRectTransform.rect.height / sprite.rect.size.y);
            }
            else
            {
                //Debug.Log("[DynamicEmitterUI] targetRectTransform aspect larger - targetRectTransform aspect: " + targetAspect + ", sprite aspect: " + spriteAspect);
                wMult = sprite.pixelsPerUnit * (targetRectTransform.rect.width / sprite.rect.size.x);
                hMult = sprite.pixelsPerUnit * (targetRectTransform.rect.height / sprite.rect.size.y) * (spriteAspect / targetAspect);
            }
        }
        else
        {
            // calculate multiplier offsets based on pixels per unit.
            wMult = sprite.pixelsPerUnit * (targetRectTransform.rect.width / sprite.rect.size.x);
            hMult = sprite.pixelsPerUnit * (targetRectTransform.rect.height / sprite.rect.size.y);
        }
    }

    /// <summary>
    /// Randomly emit the amount of particles for this sprite.
    /// Variables ref or copied as local for fast access inside the emision loop.
    /// </summary>
    /// <param name="emitCount">Number of particles to emit</param>
    void EmitDynamic(int emitCount)
    {
        Sprite sprite = GetSprite();

        if (!sprite)
            return;

        float colorR = EmitFromColor.r;
        float colorG = EmitFromColor.g;
        float colorB = EmitFromColor.b;

        float PixelsPerUnit = sprite.pixelsPerUnit;

        float width = (int)sprite.rect.size.x;
        float height = (int)sprite.rect.size.y;
        float startSize = GetParticleStartSize(PixelsPerUnit);

        //calculate sprite offset position in texture
        float offsetX = sprite.pivot.x / PixelsPerUnit;
        float offsetY = sprite.pivot.y / PixelsPerUnit;

        Color[] pix = GetSpriteColorsData(sprite);

        float toleranceR = RedTolerance;
        float toleranceG = GreenTolerance;
        float toleranceB = BlueTolerance;

        float widthByHeight = width * height;

        Color[] cCache = colorCache;
        int[] iCache = indexCache;

        // enlarge cache data if needed.
        if (cCache.Length < widthByHeight)
        {
            colorCache = new Color[(int)widthByHeight];
            indexCache = new int[(int)widthByHeight];
            cCache = colorCache;
            iCache = indexCache;
        }

        int intWidth = (int)width;

        float halfPixelSize = 1 / PixelsPerUnit / 2;

        Vector3 transformPos = Vector3.zero;
        Quaternion transformRot = Quaternion.identity;
        Vector3 transformScale = Vector3.one;
        bool flipX = false;
        bool flipY = false;

        //if Particle system is using World Space save transform modifiers. (Sprite Rendering mode only)
        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            if (SimulationSpace != ParticleSystemSimulationSpace.Local)
            {
                transformPos = spriteTransformReference.position;
                transformRot = spriteTransformReference.rotation;
                transformScale = spriteTransformReference.lossyScale;
            }
            flipX = spriteRenderer.flipX;
            flipY = spriteRenderer.flipY;
        }

        // This int saves the amount of particles posible emit position limited by the effect params, color, border, etc.
        // (ie: let suppose we only want to emit from red pixels, if the amount of red pixels in the image is 10, matchesCount will be 10)
        int matchesCount = 0;

        bool UseEmissionFromColorLocal = UseEmissionFromColor;
        bool borderEmissionLocal = borderEmission == BorderEmission.Fast || borderEmission == BorderEmission.Precise;

        #region matching
        // breaking apart 'border emission' color matching from 'normal' color matching as border emission has a small performance cost, nonetheless.
        if (borderEmissionLocal)
        {
            // border emission work as comparing the last pixel with the current one
            // borderEmission set to Fast will only check on X axis as is the fastest, while set to Precise will work in both X and Y axis.
            // A lot of repeated code here, if only c# had the inline compiler!

            bool lastVisible = false;
            Color lastColor = pix[0];
            int widthInt = (int)width;

            bool borderEmissionPreciseLocal = borderEmission == BorderEmission.Precise;
            //find available pixels to emit from
            for (int i = 0; i < widthByHeight; i++)
            {
                Color c = pix[i];
                bool currentVisible = c.a > 0;

                // Check borders in the Y axis.
                if (borderEmissionPreciseLocal)
                {
                    int prevYindex = i - widthInt;
                    if (prevYindex > 0)
                    {
                        Color cPrev = pix[prevYindex];
                        bool prevVisibleInY = cPrev.a > 0;
                        if (currentVisible)
                        {
                            if (!prevVisibleInY)
                            {
                                //Skip unwanted colors
                                if (UseEmissionFromColorLocal)
                                    if (!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                                     !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                                     !FloatComparer.AreEqual(colorB, c.b, toleranceB))
                                        continue;

                                //current pixel is visible but the previous is not, must save pixel data.
                                cCache[matchesCount] = c;
                                iCache[matchesCount] = i;
                                matchesCount++;
                                lastColor = c;
                                lastVisible = true;
                                continue;
                            }
                        }
                        else
                        {
                            if (prevVisibleInY)
                            {
                                //Skip unwanted colors
                                if (UseEmissionFromColorLocal)
                                    if (!FloatComparer.AreEqual(colorR, cPrev.r, toleranceR) ||
                                     !FloatComparer.AreEqual(colorG, cPrev.g, toleranceG) ||
                                     !FloatComparer.AreEqual(colorB, cPrev.b, toleranceB))
                                        continue;

                                //current pixel is invisible but the previous is visible, must save pixel data.
                                cCache[matchesCount] = cPrev;
                                iCache[matchesCount] = prevYindex;
                                matchesCount++;
                            }
                        }
                    }
                }

                // Check borders in the X axis.
                if (!currentVisible && lastVisible)
                {
                    //Skip unwanted colors
                    if (UseEmissionFromColorLocal)
                        if (!FloatComparer.AreEqual(colorR, lastColor.r, toleranceR) ||
                         !FloatComparer.AreEqual(colorG, lastColor.g, toleranceG) ||
                         !FloatComparer.AreEqual(colorB, lastColor.b, toleranceB))
                            continue;

                    cCache[matchesCount] = lastColor;
                    iCache[matchesCount] = i - 1;
                    matchesCount++;
                    lastVisible = true;
                }

                lastColor = c;
                if (!currentVisible)
                {
                    lastVisible = false;
                    continue;
                }

                if (!borderEmissionLocal || (currentVisible && !lastVisible))
                {
                    //Skip unwanted colors
                    if (UseEmissionFromColorLocal)
                        if (!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                         !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                         !FloatComparer.AreEqual(colorB, c.b, toleranceB))
                            continue;

                    cCache[matchesCount] = c;
                    iCache[matchesCount] = i;
                    matchesCount++;
                    lastVisible = true;
                }
            }
        }
        #endregion
        else
        {
            //find available pixels to emit from
            for (int i = 0; i < widthByHeight; i++)
            {
                Color c = pix[i];
                //skip pixels with alpha 0
                if (c.a <= 0)
                    continue;

                //Skip unwanted colors when using Emission from color.
                if (UseEmissionFromColorLocal)
                    if (!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                        !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                        !FloatComparer.AreEqual(colorB, c.b, toleranceB))
                        continue;

                cCache[matchesCount] = c;
                iCache[matchesCount] = i;
                matchesCount++;
            }
        }

        // if no pixel were matched, stop
        if (matchesCount <= 0)
            return;

        //Debug.Log("emitCount: " + emitCount);
        Vector3 tempV = Vector3.zero;
        Vector3 betweenFramesPrecisionPos = transformPos;
        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            //emit needed particles count
            for (int k = 0; k < emitCount; k++)
            {
                int index = Random.Range(0, matchesCount - 1);
                int i = iCache[index];

                if (useBetweenFramesPrecision)
                {
                    float randomDelta = Random.Range(0, 1f);
                    betweenFramesPrecisionPos = Vector3.Lerp(lastTransformPosition, transformPos, randomDelta);
                }

                //get pixel position in texture
                float posX = ((i % width) / PixelsPerUnit) - offsetX + halfPixelSize;
                float posY = ((i / intWidth) / PixelsPerUnit) - offsetY + halfPixelSize;
                //handle sprite renderer fliping
                if (flipX)
                    posX = width/PixelsPerUnit - posX - offsetX*2;
                if (flipY)
                    posY = height/PixelsPerUnit - posY - offsetY*2;

                tempV.x = posX*transformScale.x;
                tempV.y = posY*transformScale.y;

                ParticleSystem.EmitParams em = new ParticleSystem.EmitParams();
                // define new particle start position based on Sprite pixel position in texture, this game object's rotation and position.
                em.position = transformRot * tempV + betweenFramesPrecisionPos;
                if (UsePixelSourceColor)
                    em.startColor = cCache[index];

                em.startSize = startSize;
                particlesSystem.Emit(em, 1);
            }
        }
        else
        {
            for (int k = 0; k < emitCount; k++)
            {
                int index = Random.Range(0, matchesCount - 1);
                int i = iCache[index];

                //get pixel position in texture
                float posX = ((i % width) / PixelsPerUnit) - offsetX + halfPixelSize;
                float posY = ((i / intWidth) / PixelsPerUnit) - offsetY + halfPixelSize;

                ParticleSystem.EmitParams em = new ParticleSystem.EmitParams();
                // define new particle start position based on Sprite pixel position in texture, this game object's rotation and position.
                tempV.x = posX * wMult + offsetXY.x;
                tempV.y = posY * hMult - offsetXY.y;
                em.position = tempV;

                if (UsePixelSourceColor)
                    em.startColor = cCache[index];

                em.startSize = startSize;
                particlesSystem.Emit(em, 1);
            }
        }
    }

    /// <summary>
    /// Will emit one particle from every pixel in the sprite
    /// Variables ref or copied as local for fast access inside the emision loop.
    /// borderEmission not working at the moment for EmitAll.
    /// </summary>
    void EmitAllDynamic()
    {
        Sprite sprite = GetSprite();

        if (!sprite)
            return;

        float colorR = EmitFromColor.r;
        float colorG = EmitFromColor.g;
        float colorB = EmitFromColor.b;

        float PixelsPerUnit = sprite.pixelsPerUnit;

        float width = (int) sprite.rect.size.x;
        float height = (int) sprite.rect.size.y;

        float startSize = GetParticleStartSize(PixelsPerUnit);

        //calculate sprite offset position in texture
        float offsetX = sprite.pivot.x/PixelsPerUnit;
        float offsetY = sprite.pivot.y/PixelsPerUnit;

        Color[] pix = GetSpriteColorsData(sprite);

        float toleranceR = RedTolerance;
        float toleranceG = GreenTolerance;
        float toleranceB = BlueTolerance;

        float widthByHeight = width*height;

        int intWidth = (int)width;

        //set particle size based on sprite Pixels per unit and particle system prefered size
        float halfPixelSize = 1 / PixelsPerUnit / 2;

        Vector3 transformPos = Vector3.zero;
        Quaternion transformRot = Quaternion.identity;
        Vector3 transformScale = Vector3.one;
        bool flipX = false;
        bool flipY = false;

        //if Particle system is using World Space save transform modifiers.
        if (renderSystemType == RenderSystemUsing.SpriteRenderer && SimulationSpace != ParticleSystemSimulationSpace.Local)
        {
            transformPos = spriteTransformReference.position;
            transformRot = spriteTransformReference.rotation;
            transformScale = spriteTransformReference.lossyScale;
            flipX = spriteRenderer.flipX;
            flipY = spriteRenderer.flipY;
        }
        
        Vector3 tempV = Vector3.zero;
        Vector3 betweenFramesPrecisionPos = transformPos;
        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            for (int i = 0; i < widthByHeight; i++)
            {
                Color c = pix[i];
                //skip pixels with alpha 0
                if (c.a <= 0)
                    continue;

                //Skip unwanted colors when using Emission from color.
                if (UseEmissionFromColor)
                    if (!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                        !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                        !FloatComparer.AreEqual(colorB, c.b, toleranceB))
                        continue;

                //get pixel position in texture
                float posX = ((i%width)/PixelsPerUnit) - offsetX;
                float posY = ((i/intWidth)/PixelsPerUnit) - offsetY;

                //handle sprite renderer fliping
                if (flipX)
                    posX = width/PixelsPerUnit - posX - offsetX*2;
                if (flipY)
                    posY = height/PixelsPerUnit - posY - offsetY*2;

                tempV.x = posX*transformScale.x + halfPixelSize;
                tempV.y = posY*transformScale.y + halfPixelSize;

                ParticleSystem.EmitParams em = new ParticleSystem.EmitParams();
                // define new particle start position based on Sprite pixel position in texture, this game object's rotation and position.
                em.position = transformRot * tempV + betweenFramesPrecisionPos;
                if (UsePixelSourceColor)
                    em.startColor = c;

                em.startSize = startSize;
                particlesSystem.Emit(em, 1);
            }
        }
        else
        {
            for (int i = 0; i < widthByHeight; i++)
            {
                Color c = pix[i];
                //skip pixels with alpha 0
                if (c.a <= 0)
                    continue;

                //Skip unwanted colors when using Emission from color.
                if (UseEmissionFromColor)
                    if (!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                        !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                        !FloatComparer.AreEqual(colorB, c.b, toleranceB))
                        continue;

                //get pixel position in texture
                float posX = ((i%width)/PixelsPerUnit) - offsetX;
                float posY = ((i/intWidth)/PixelsPerUnit) - offsetY;

                ParticleSystem.EmitParams em = new ParticleSystem.EmitParams();
                // define new particle start position based on Sprite pixel position in texture, this game object's rotation and position.
                tempV.x = posX*wMult + offsetXY.x + halfPixelSize;
                tempV.y = posY*hMult - offsetXY.y + halfPixelSize;
                em.position = tempV;

                if (UsePixelSourceColor)
                    em.startColor = c;

                em.startSize = startSize;
                particlesSystem.Emit(em, 1);
            }
        }
    }
    #endregion

    #region Static

    /// <summary>
    /// Will cache sprite data needed to emit later.
    /// Static emitter needs to cache sprite coords data first before emitting.
    /// A lot of variables are saved as local for fast access.
    /// </summary>
    public void CacheSprite(bool relativeToParent = false)
    {
        //safe check
        if (!particlesSystem)
            return;

        hasCachingEnded = false;
        particlesCacheCount = 0;

        Sprite sprite = GetSprite();

        if (!sprite)
            return;

        float colorR = EmitFromColor.r;
        float colorG = EmitFromColor.g;
        float colorB = EmitFromColor.b;
        float PixelsPerUnit = sprite.pixelsPerUnit;
        float width = (int)sprite.rect.size.x;
        float height = (int)sprite.rect.size.y;
        int intWidth = (int)width;
        float halfPixelSize = 1 / PixelsPerUnit / 2;
        //set particle size based on sprite Pixels per unit and particle system prefered size
        particleStartSize = GetParticleStartSize(PixelsPerUnit);

        //calculate sprite offset position in texture
        float offsetX = sprite.pivot.x / PixelsPerUnit;
        float offsetY = sprite.pivot.y / PixelsPerUnit;

        //ask texture for wanted sprite
        Color[] pix = GetSpriteColorsData(sprite);

        float toleranceR = RedTolerance;
        float toleranceG = GreenTolerance;
        float toleranceB = BlueTolerance;

        float widthByHeight = width * height;

        Vector3 transformPos = Vector3.zero;
        Quaternion transformRot = Quaternion.identity;
        Vector3 transformScale = Vector3.one;
        bool flipX = false;
        bool flipY = false;

        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            transformPos = spriteTransformReference.position;
            transformRot = spriteTransformReference.rotation;
            transformScale = spriteTransformReference.lossyScale;
            flipX = spriteRenderer.flipX;
            flipY = spriteRenderer.flipY;
        }

        // Create lists for fast insertion
        List<Color> colorsCache = new List<Color>();
        List<Vector3> positionsCache = new List<Vector3>();
        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            for (int i = 0; i < widthByHeight; i++)
            {
                Color c = pix[i];
                //skip pixels with alpha 0
                if (c.a <= 0)
                    continue;

                //Skip unwanted colors when using Emission from color.
                if (UseEmissionFromColor &&
                    (!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                     !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                     !FloatComparer.AreEqual(colorB, c.b, toleranceB)))
                    continue;

                //get pixel position in texture
                float posX = ((i % width) / PixelsPerUnit) - offsetX + halfPixelSize;
                float posY = ((i / intWidth) / PixelsPerUnit) - offsetY + halfPixelSize;

                //handle sprite renderer fliping
                if (flipX)
                    posX = width / PixelsPerUnit - posX - offsetX * 2;
                if (flipY)
                    posY = height / PixelsPerUnit - posY - offsetY * 2;

                Vector3 vTemp;
                // define new particle start position based on Sprite pixel position in texture, this game object's rotation and position.
                if (relativeToParent)
                    vTemp = transformRot * new Vector3(posX * transformScale.x, posY * transformScale.y, 0) + transformPos;
                else
                    vTemp = new Vector3(posX, posY, 0);

                positionsCache.Add(vTemp);
                colorsCache.Add(c);
                particlesCacheCount++;
            }
        }
        else
        {
            for (int i = 0; i < widthByHeight; i++)
            {
                Color c = pix[i];
                //skip pixels with alpha 0
                if (c.a <= 0)
                    continue;

                //Skip unwanted colors when using Emission from color.
                if (UseEmissionFromColor &&
                    (!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                        !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                        !FloatComparer.AreEqual(colorB, c.b, toleranceB)))
                    continue;

                //get pixel position in texture
                float posX = ((i % width) / PixelsPerUnit) - offsetX + halfPixelSize;
                float posY = ((i / intWidth) / PixelsPerUnit) - offsetY + halfPixelSize;

                Vector3 vTemp;
                // define new particle start position based on Sprite pixel position in texture, this game object's rotation and position.
                //if (relativeToParent)
                //    vTemp = transformRot * new Vector3(posX * transformScale.x, posY * transformScale.y, 0) + transformPos;
                //else
                vTemp = new Vector3(posX, posY, 0);

                positionsCache.Add(vTemp);
                colorsCache.Add(c);
                particlesCacheCount++;
            }
        }

        //Duplicate data as an array for better performance when accessing later
        particleInitPositionsCache = positionsCache.ToArray();
        particleInitColorCache = colorsCache.ToArray();
        if (particlesCacheCount <= 0)
        {
            if (verboseDebug)
                Debug.LogWarning("Caching particle emission went wrong. This is most probably because couldn't find wanted color in sprite");
            return;
        }

        //clear unwanted allocation
        pix = null;
        positionsCache.Clear();
        positionsCache = null;
        colorsCache.Clear();
        colorsCache = null;
        System.GC.Collect();

        hasCachingEnded = true;

#if UNITY_EDITOR
        cachedSprite = sprite;
#endif
        //finally call event to warn we've finished
        if (OnCacheEnded != null)
            OnCacheEnded();

        if (OnAvailableToPlay != null)
            OnAvailableToPlay();
    }

    /// <summary>
    /// Handle further calculations before emission when dealing with UI render mode.
    /// </summary>
    void ProcessPositionAndScaleStatic()
    {
        //match current RectTransform's data with target RectTransform
        if (matchTargetGOPostionData)
            currentRectTransform.position = new Vector3(targetRectTransform.position.x,
                targetRectTransform.position.y, targetRectTransform.position.z);
        currentRectTransform.pivot = targetRectTransform.pivot;
        if (matchTargetGOPostionData)
        {
            currentRectTransform.anchoredPosition = targetRectTransform.anchoredPosition;
            currentRectTransform.anchorMin = targetRectTransform.anchorMin;
            currentRectTransform.anchorMax = targetRectTransform.anchorMax;
            currentRectTransform.offsetMin = targetRectTransform.offsetMin;
            currentRectTransform.offsetMax = targetRectTransform.offsetMax;
        }
        if (matchTargetGOScale)
            currentRectTransform.localScale = targetRectTransform.localScale;
        currentRectTransform.rotation = targetRectTransform.rotation;

        currentRectTransform.sizeDelta = new Vector2(targetRectTransform.rect.width, targetRectTransform.rect.height);

        //Calculate position multipliers based on pixels per unit
        float offsetX = (1 - currentRectTransform.pivot.x) * (currentRectTransform.rect.width) - currentRectTransform.rect.width / 2;
        float offsetY = (1 - currentRectTransform.pivot.y) * (-currentRectTransform.rect.height) + currentRectTransform.rect.height / 2;
        offsetXY = new Vector2(offsetX, offsetY);
        Sprite sprite = GetSprite();
        if (!sprite)
            return;
        // calculate multiplier offsets based on pixels per unit.
        wMult = sprite.pixelsPerUnit * (currentRectTransform.rect.width / sprite.rect.size.x);
        hMult = sprite.pixelsPerUnit * (currentRectTransform.rect.height / sprite.rect.size.y);
    }

    /// <summary>
    /// Randomly emit the amount of particles for this sprite.
    /// Variables ref or copied as local for fast access inside the emision loop.
    /// </summary>
    /// <param name="emitCount">Number of particles to emit</param>
    void EmitStatic(int emitCount)
    {
        //safe check
        if (!hasCachingEnded)
            return;

        int pCount = particlesCacheCount;
        float pStartSize = particleStartSize;

        bool spriteRenderMode = renderSystemType == RenderSystemUsing.SpriteRenderer;
        if (particlesCacheCount <= 0)
            return;

        //getting sprite source as gameobject for position rotation and scale
        Vector3 transformPos;
        Quaternion transformRot;
        Vector3 transformScale;

        if (spriteRenderMode)
        {
            transformPos = spriteTransformReference.position;
            transformRot = spriteTransformReference.rotation;
            transformScale = spriteTransformReference.lossyScale;
        }
        else
        {
            transformPos = currentRectTransform.position;
            transformRot = currentRectTransform.rotation;
            transformScale = currentRectTransform.lossyScale;
        }

        Vector3 betweenFramesPrecisionPos = transformPos;
        ParticleSystemSimulationSpace currentSimulationSpace = SimulationSpace;

        //ref as local for faster access
        Color[] _colorCache = particleInitColorCache;
        Vector3[] posCache = particleInitPositionsCache;
        Vector3 tempV = Vector3.zero;
        for (int i = 0; i < emitCount; i++)
        {
            int rnd = Random.Range(0, pCount);
            if (useBetweenFramesPrecision)
            {
                float randomDelta = Random.Range(0, 1f);
                betweenFramesPrecisionPos = Vector3.Lerp(lastTransformPosition, transformPos, randomDelta);
            }

            ParticleSystem.EmitParams em = new ParticleSystem.EmitParams();
            if (UsePixelSourceColor)
                em.startColor = _colorCache[rnd];
            em.startSize = pStartSize;

            Vector3 origPos = posCache[rnd];

            //if particles are set to World we must remove original particle calculation and apply the new transform modifiers.
            if (currentSimulationSpace == ParticleSystemSimulationSpace.World)
            {
                if (spriteRenderMode)
                {
                    tempV.x = origPos.x*transformScale.x;
                    tempV.y = origPos.y*transformScale.y;
                }
                else
                {
                    tempV.x = (origPos.x * wMult) * transformScale.x + offsetXY.x;
                    tempV.y = (origPos.y * hMult) * transformScale.y - offsetXY.y;
                }
                em.position = transformRot * tempV + betweenFramesPrecisionPos;
                particlesSystem.Emit(em, 1);
            }
            else
            {
                if (spriteRenderMode)
                {
                    em.position = posCache[rnd];
                }
                else
                {
                    tempV.x = (origPos.x * wMult) + offsetXY.x;
                    tempV.y = (origPos.y * hMult) - offsetXY.y;
                    em.position = tempV;
                }
                particlesSystem.Emit(em, 1);
            }
        }
        
    }

    /// <summary>
    /// Will emit one particle from every pixel in the sprite
    /// Variables ref or copied as local for fast access inside the emision loop.
    /// borderEmission not working at the moment for EmitAll.
    /// </summary>
    void EmitAllStatic()
    {
        //safe check
        if (!hasCachingEnded)
            return;

        int pCount = particlesCacheCount;
        float pStartSize = particleStartSize;

        bool spriteRenderMode = renderSystemType == RenderSystemUsing.SpriteRenderer;
        if (particlesCacheCount <= 0)
            return;

        //getting sprite source as gameobject for position rotation and scale
        Vector3 transformPos;
        Quaternion transformRot;
        Vector3 transformScale;

        if (spriteRenderMode)
        {
            transformPos = spriteTransformReference.position;
            transformRot = spriteTransformReference.rotation;
            transformScale = spriteTransformReference.lossyScale;
        }
        else
        {
            transformPos = currentRectTransform.position;
            transformRot = currentRectTransform.rotation;
            transformScale = currentRectTransform.lossyScale;
        }

        Vector3 betweenFramesPrecisionPos = transformPos;
        ParticleSystemSimulationSpace currentSimulationSpace = SimulationSpace;

        //ref as local for faster access
        Color[] _colorCache = particleInitColorCache;
        Vector3[] posCache = particleInitPositionsCache;
        Vector3 tempV = Vector3.zero;
        for (int i = 0; i < pCount; i++)
        {
            if (useBetweenFramesPrecision)
            {
                float randomDelta = Random.Range(0, 1f);
                betweenFramesPrecisionPos = Vector3.Lerp(lastTransformPosition, transformPos, randomDelta);
            }

            ParticleSystem.EmitParams em = new ParticleSystem.EmitParams();
            if (UsePixelSourceColor)
                em.startColor = _colorCache[i];
            em.startSize = pStartSize;

            Vector3 origPos = posCache[i];

            //if particles are set to World we must remove original particle calculation and apply the new transform modifiers.
            if (currentSimulationSpace == ParticleSystemSimulationSpace.World)
            {
                if (spriteRenderMode)
                {
                    tempV.x = origPos.x * transformScale.x;
                    tempV.y = origPos.y * transformScale.y;
                }
                else
                {
                    tempV.x = (origPos.x * wMult) * transformScale.x + offsetXY.x;
                    tempV.y = (origPos.y * hMult) * transformScale.y - offsetXY.y;
                }
                em.position = transformRot * tempV + betweenFramesPrecisionPos;
                particlesSystem.Emit(em, 1);
            }
            else
            {
                if (spriteRenderMode)
                {
                    em.position = posCache[i];
                }
                else
                {
                    tempV.x = (origPos.x * wMult) + offsetXY.x;
                    tempV.y = (origPos.y * hMult) - offsetXY.y;
                    em.position = tempV;
                }
                particlesSystem.Emit(em, 1);
            }
        }
    }

    #endregion

    /// <summary>
    /// Get the current sprite using, whether is part of Image Renderer or SpriteRenderer
    /// </summary>
    /// <returns></returns>
    public Sprite GetSprite()
    {
        Sprite sprite;

        if (renderSystemType == RenderSystemUsing.ImageRenderer)
        {
            if (!imageRenderer)
            {
                if (verboseDebug)
                    Debug.LogError("imageRenderer is null in game object " + name);
                return null;
            }
            sprite = imageRenderer.sprite;

            if (imageRenderer.overrideSprite)
            {
                sprite = imageRenderer.overrideSprite;
            }
        }
        else
        {
            if (!spriteRenderer)
            {
                if (verboseDebug)
                    Debug.LogError("spriteRenderer is null in game object " + name);
                return null;
            }
            sprite = spriteRenderer.sprite;
        }

        if (!sprite)
        {
            if (verboseDebug)
                Debug.LogError("Sprite is null in game object " + name);
            isPlaying = false;
            return null;
        }

        return sprite;
    }

    /// <summary>
    /// Calculate the particle main size based on the sprite pixel ratio.
    /// </summary>
    /// <param name="PixelsPerUnit"></param>
    /// <returns></returns>
    float GetParticleStartSize(float PixelsPerUnit)
    {
        float startSize;
        if (renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            startSize = 1 / (PixelsPerUnit);
            #if UNITY_5_5_OR_NEWER
            startSize *= mainModule.startSize.constant; //TODO ability to process different sizes
            #else
            startSize *= particlesSystem.startSize;
            #endif
        }
        else
        {
            #if UNITY_5_5_OR_NEWER
            startSize = mainModule.startSize.constant;
            #else
            startSize = particlesSystem.startSize;
            #endif
        }

        return startSize;
    }

    /// <summary>
    /// If the sprite raw data is cached in any way return it, if not ask for it to the texture.
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    Color[] GetSpriteColorsData(Sprite sprite)
    {
        Color[] pix;
        Rect rect = sprite.rect;
        if (useSpritesSharingPool && Application.isPlaying)
            pix = SpritesDataPool.GetSpriteColors(sprite, (int)rect.position.x, (int)rect.position.y, (int)rect.size.x, (int)rect.size.y);
        else if (CacheSprites && mode == SpriteMode.Dynamic)
        {
            if (spritesSoFar.ContainsKey(sprite))
                pix = spritesSoFar[sprite];
            else
            {
                pix = sprite.texture.GetPixels((int)rect.position.x, (int)rect.position.y, (int)rect.size.x, (int)rect.size.y);
                spritesSoFar.Add(sprite, pix);
            }
        }
        else
            pix = sprite.texture.GetPixels((int)rect.position.x, (int)rect.position.y, (int)rect.size.x, (int)rect.size.y);

        return pix;
    }

#if UNITY_EDITOR
    void OnEnable()
    {
        ForceNextUseOfHack();
    }
    void OnValidate()
    {
        EditorInvalidate();
    }

    /// <summary>
    /// Editor method for continuous modification.
    /// </summary>
    void EditorInvalidate()
    {
        ForceNextUseOfHack();
        //Debug.Log("EditorInvalidate");
        if (particlesSystem)
            particlesSystem.Stop();
        isPlaying = false;
        Awake();
        if (mode == SpriteMode.Static)
            CacheSprite();
    }

    /// <summary>
    /// This will reset all information stored by the plugin (Only for Editor).
    /// </summary>
    public void ResetAllCache()
    {
        SpritesDataPool.ReleaseMemory();
        cachedSprite = null;
        spritesSoFar = new Dictionary<Sprite, Color[]>();
        Awake();
    }
#endif

    bool forceHack = true;
    /// <summary>
    /// This hack (provided with the unity dev team) fixes a bug which may crash the unity editor in some 2017.x Unity versions.
    /// For more info: https://issuetracker.unity3d.com/issues/unity-crashes-when-particles-are-awaken-by-script
    /// </summary>
    void HackUnityCrash2017()
    {
        if (forceHack && !particlesSystem.isStopped)
        {
            particlesSystem.Emit(1);
            particlesSystem.Clear();
            //Debug.Log("Try fix Crash");
            forceHack = false;
        }
    }

    void ForceNextUseOfHack()
    {
        forceHack = true;
    }
}

}