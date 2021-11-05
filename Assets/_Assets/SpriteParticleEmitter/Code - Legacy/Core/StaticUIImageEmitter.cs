//#define MEM_DEBUG
using System;
using UnityEngine;
using System.Collections.Generic;
using SpriteToParticlesAsset;
using UnityEngine.Assertions.Comparers;

namespace SpriteParticleEmitter
{
//[Obsolete("Use SpriteToParticles component instead")]
/// <summary>
/// Obsolete: Use SpriteToParticles component instead - Base component for StaticEmitterOneShot and StaticEmitterContinuous
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(UIParticleRenderer))]
public class StaticUIImageEmitter : EmitterBaseUI
{
    [Header("Awake Options")]
    [Tooltip("Activating this will force CacheOnAwake")]
    //! Activating this will force CacheOnAwake
    public bool PlayOnAwake = true;
    [Tooltip("Should the system cache on Awake method? - Static emission needs to be cached first, if this property is not checked the CacheSprite() method should be called by code. (Refer to manual for further explanation)")]
    //! Should the system cache on Awake method? - Static emission needs to be cached first, if this property is not checked the CacheSprite() method should be called by code. (Refer to manual for further explanation)
    public bool CacheOnAwake = true;

    //! is system ready to start emitting?
    protected bool hasCachingEnded;

    //! Processed particles count
    protected int particlesCacheCount;
    //! The size the particles will start with
    protected float particleStartSize;

    //! Save data as an array for better access performance, also not putting it on a struct for GC not to become crazy 
    protected Vector3[] particleInitPositionsCache;
    //! Save data as an array for better access performance, also not putting it on a struct for GC not to become crazy
    protected Color[] particleInitColorCache;

    public override event SimpleEvent OnCacheEnded;
    public override event SimpleEvent OnAvailableToPlay;

    protected override void Awake()
    {
        base.Awake();
        if (PlayOnAwake)
        {
            isPlaying = true;
            CacheOnAwake = true;
        }

        if (CacheOnAwake)
            CacheSprite();
    }

    /// <summary>
    /// Will cache sprite data needed to emit later.
    /// Static emitter needs to cache sprite coords data first before emitting.
    /// A lot of variables are saved as local for fast access.
    /// </summary>
    public virtual void CacheSprite(bool relativeToParent = false)
    {
#if MEM_DEBUG
        Debug.Log("<color=black>CacheSprite</color> " + gameObject.name);
        long mem = System.GC.GetTotalMemory(false);
        Debug.Log("<color=red>F00 = </color>" + mem / 1024 / 1024);
#endif

        hasCachingEnded = false;
        particlesCacheCount = 0;

        Sprite sprite = imageRenderer.sprite;

        if (!sprite)
        {
            if (verboseDebug)
                Debug.LogError("Unable to cache. Sprite is null in game object " + name);
            return;
        }

        float colorR = EmitFromColor.r;
        float colorG = EmitFromColor.g;
        float colorB = EmitFromColor.b;

        float PixelsPerUnit = sprite.pixelsPerUnit;
        if (imageRenderer == null || imageRenderer.sprite == null)
        {
            if (verboseDebug)
                Debug.LogError("Image UI reference missing");
        }

        float width = (int)sprite.rect.size.x;
        float height = (int)sprite.rect.size.y;

        //set particle size based on sprite Pixels per unit and particle system prefered size
        #if UNITY_5_5_OR_NEWER
        particleStartSize = mainModule.startSize.constant; //TODO ability to process different sizes coming in next update
        #else
            particleStartSize = particlesSystem.startSize;
        #endif

        //calculate sprite offset position in texture
        float offsetX = sprite.pivot.x / PixelsPerUnit;
        float offsetY = sprite.pivot.y / PixelsPerUnit;

        //ask texture for wanted sprite
        Color[] pix;
        if (useSpritesSharingCache && Application.isPlaying)
            pix = SpritesDataPool.GetSpriteColors(sprite, (int)sprite.rect.position.x, (int)sprite.rect.position.y, (int)width, (int)height);
        else
            pix = sprite.texture.GetPixels((int)sprite.rect.position.x, (int)sprite.rect.position.y, (int)width, (int)height);

        float toleranceR = RedTolerance;
        float toleranceG = GreenTolerance;
        float toleranceB = BlueTolerance;

        float widthByHeight = width * height;

        // Create lists for fast insertion
        List<Color> colorsCache = new List<Color>();
        List<Vector3> positionsCache = new List<Vector3>();
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
            float posX = ((i % width) / PixelsPerUnit) - offsetX;
            float posY = ((i / width) / PixelsPerUnit) - offsetY;

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

        //Duplicate data as an array for better performance when accessing later
        //particlesCacheArray = particlesCache.ToArray();
        particleInitPositionsCache = positionsCache.ToArray();
        particleInitColorCache = colorsCache.ToArray();
        if (particlesCacheCount <= 0)
        {
            if (verboseDebug)
                Debug.LogWarning("Caching particle emission went wrong. This is most probably because couldn't find wanted color in sprite");
            return;
        }

#if MEM_DEBUG
        Debug.Log("<color=green>F01 = </color>" + System.GC.GetTotalMemory(false) / 1024 / 1024 + " | delta: " + (System.GC.GetTotalMemory(false) - mem) / 1024 / 1024);
#endif
        //clear unwanted allocation
        pix = null;
        positionsCache.Clear();
        positionsCache = null;
        colorsCache.Clear();
        colorsCache = null;
        System.GC.Collect();

        hasCachingEnded = true;

#if MEM_DEBUG
        Debug.Log("<color=blue>F02 = </color> = " + System.GC.GetTotalMemory(false) / 1024 / 1024 + " | final delta: " + (System.GC.GetTotalMemory(false) - mem) / 1024 / 1024);
#endif
#if UNITY_EDITOR
        cachedSprite = sprite;
#endif
        //finally call event to warn we've finished
        if (OnCacheEnded != null)
            OnCacheEnded();
    }

    protected virtual void Update()
    {
#if MEM_DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            long mem = System.GC.GetTotalMemory(false);
            Debug.Log("Current = " + mem / 1024 / 1024);
        }
#endif

#if UNITY_EDITOR
        if (cachedSprite != imageRenderer.sprite)
            EditorInvalidate();
#endif
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        EditorInvalidate();
    }

    void EditorInvalidate()
    {
        if (particlesSystem)
            particlesSystem.Stop();
        isPlaying = false;
        Awake();
        CacheSprite();
    }
#endif

    public override void Play() { }
    public override void Stop() { }
    public override void Pause() { }

    public override bool IsPlaying()
    {
        return isPlaying;
    }

    public override bool IsAvailableToPlay()
    {
        return hasCachingEnded;
    }

    private void DummyMethod()
    {
        if (OnAvailableToPlay != null)
            OnAvailableToPlay();

        if (OnCacheEnded != null)
            OnCacheEnded();
    }
}
}