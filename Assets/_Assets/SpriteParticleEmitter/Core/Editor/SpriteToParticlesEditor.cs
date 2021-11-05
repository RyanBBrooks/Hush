using SpriteParticleEmitter;
using UnityEditor;
using UnityEngine;
using SpriteToParticlesAsset;
using UnityEditor.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// This editor class will handle eveything editor related to StP, menu options, proper variable display for the component, etc.
/// </summary>
[CustomEditor(typeof(SpriteToParticles))]
public class SpriteToParticlesEditor : Editor
{
    [Tooltip("Weather the inspector shold show help tips or not.")]
    //! Weather the inspector shold show help tips or not.
    public bool showHelp;
    //public bool showWarnings;
    //protected static bool advancedOptions = true;

    SerializedProperty verboseDebug;
    SerializedProperty spriteMode;
    SerializedProperty renderSystemType;
    SerializedProperty spriteRenderer;
    SerializedProperty imageRenderer;
    SerializedProperty particlesSystem;
    SerializedProperty PlayOnAwake;
    SerializedProperty EmissionRate;
    SerializedProperty UsePixelSourceColor;
    SerializedProperty borderEmission;
    SerializedProperty UseEmissionFromColor;

    SerializedProperty EmitFromColor;
    SerializedProperty RedTolerance;
    SerializedProperty GreenTolerance;
    SerializedProperty BlueTolerance;
    SerializedProperty useSpritesSharingCache;

    SerializedProperty useBetweenFramesPrecision;
    SerializedProperty CacheSprites;
    SerializedProperty CacheOnAwake;
    SerializedProperty matchTargetGOPostionData;
    SerializedProperty matchTargetGOScale;

    /// <summary>
    /// Find all properties in selected StP
    /// </summary>
    void OnEnable()
    {
        // Setup the SerializedProperties
        verboseDebug = serializedObject.FindProperty("verboseDebug");
        spriteMode = serializedObject.FindProperty("mode");
        renderSystemType = serializedObject.FindProperty("renderSystemType");
        spriteRenderer = serializedObject.FindProperty("spriteRenderer");
        imageRenderer = serializedObject.FindProperty("imageRenderer");
        particlesSystem = serializedObject.FindProperty("particlesSystem");
        PlayOnAwake = serializedObject.FindProperty("PlayOnAwake");
        EmissionRate = serializedObject.FindProperty("EmissionRate");
        UsePixelSourceColor = serializedObject.FindProperty("UsePixelSourceColor");
        borderEmission = serializedObject.FindProperty("borderEmission");
        UseEmissionFromColor = serializedObject.FindProperty("UseEmissionFromColor");

        EmitFromColor = serializedObject.FindProperty("EmitFromColor");
        RedTolerance = serializedObject.FindProperty("RedTolerance");
        GreenTolerance = serializedObject.FindProperty("GreenTolerance");
        BlueTolerance = serializedObject.FindProperty("BlueTolerance");
        useSpritesSharingCache = serializedObject.FindProperty("useSpritesSharingPool");

        useBetweenFramesPrecision = serializedObject.FindProperty("useBetweenFramesPrecision");
        CacheSprites = serializedObject.FindProperty("CacheSprites");
        CacheOnAwake = serializedObject.FindProperty("CacheOnAwake");
        matchTargetGOPostionData = serializedObject.FindProperty("matchTargetGOPostionData");
        matchTargetGOScale = serializedObject.FindProperty("matchTargetGOScale");
    }

    /// <summary>
    /// Display precise variables for StP different modes
    /// </summary>
    public override void OnInspectorGUI()
    {
        SpriteToParticles StP = (SpriteToParticles)target;
        showHelp = EditorPrefs.GetBool("SpriteToParticles.ShowTools");
        //showWarnings = EditorPrefs.GetBool("SpriteToParticles.ShowWarnings");

        GUIStyle Title = new GUIStyle(EditorStyles.textArea);
        Title.normal.textColor = Color.white;
        Title.fontStyle = FontStyle.Bold;

        serializedObject.Update();

        RenderSystemUsing renderType = (RenderSystemUsing)renderSystemType.intValue;
        SpriteMode _emissionMode = (SpriteMode)spriteMode.intValue;

        GUILayout.Space(5f);
        EditorGUILayout.LabelField("Main Config", Title);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(spriteMode);
        
        EditorGUILayout.PropertyField(renderSystemType);
        EditorGUI.indentLevel--;
        GUILayout.Space(5f);

        EditorGUILayout.LabelField("References", Title);
        EditorGUI.indentLevel++;
        
        if (renderType == RenderSystemUsing.SpriteRenderer)
            EditorGUILayout.PropertyField(spriteRenderer);  
        if (renderType == RenderSystemUsing.ImageRenderer)
            EditorGUILayout.PropertyField(imageRenderer);
        EditorGUILayout.PropertyField(particlesSystem);
        if (showHelp)
            CheckValidations();
        EditorGUI.indentLevel--;
        GUILayout.Space(5f);

        EditorGUILayout.LabelField("Emission Options", Title);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(PlayOnAwake);
        EditorGUILayout.PropertyField(EmissionRate);
        
        if (_emissionMode != SpriteMode.Static)
            EditorGUILayout.PropertyField(borderEmission);

        EditorGUILayout.PropertyField(UsePixelSourceColor);
        GUILayout.Space(5f);
        EditorGUILayout.PropertyField(UseEmissionFromColor);
        if (UseEmissionFromColor.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(EmitFromColor);
            EditorGUILayout.PropertyField(RedTolerance);
            EditorGUILayout.PropertyField(GreenTolerance);
            EditorGUILayout.PropertyField(BlueTolerance);
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
        GUILayout.Space(5f);

        if (IsRendererInAnotherGameObject(StP))
        {

            EditorGUILayout.LabelField("Renderer in different GameObject specifics", Title);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(matchTargetGOPostionData);
            EditorGUILayout.PropertyField(matchTargetGOScale);
            EditorGUI.indentLevel--;
            GUILayout.Space(5f);
        }

        EditorGUILayout.LabelField("Advanced Options", Title);
        EditorGUI.indentLevel++;
        //advancedOptions = EditorGUILayout.Foldout(advancedOptions, "Show");
        //if (advancedOptions)
        {
            //EditorGUI.indentLevel++;
            if (_emissionMode == SpriteMode.Dynamic)
                EditorGUILayout.PropertyField(CacheSprites);
            else
                EditorGUILayout.PropertyField(CacheOnAwake);

            EditorGUILayout.PropertyField(useSpritesSharingCache);
            EditorGUILayout.PropertyField(useBetweenFramesPrecision);
            if (showHelp && useBetweenFramesPrecision.boolValue)
            {
                #if UNITY_5_5_OR_NEWER
                if (StP.particlesSystem && StP.particlesSystem.main.simulationSpace != ParticleSystemSimulationSpace.World)
                {
                    EditorGUILayout.HelpBox("Between frames precision is only to be use with ParticleSystem's simulation space set to world.", MessageType.Warning);
                }
                #endif
            }

            GUILayout.Space(5f);

            EditorGUILayout.PropertyField(verboseDebug);

            showHelp = EditorGUILayout.Toggle("Show Inspector Help", showHelp);
            EditorPrefs.SetBool("SpriteToParticles.ShowTools", showHelp);

            //showWarnings = EditorGUILayout.Toggle("Show Inspector Warnings", showWarnings);
            //EditorPrefs.SetBool("SpriteToParticles.ShowWarnings", showWarnings);
            //EditorGUI.indentLevel--;
        }
        if (GUILayout.Button("Emit All"))
        {
            StP.particlesSystem.Play();
            StP.EmitAll(false);
        }

        if (GUILayout.Button("Reset Cache"))
        {
            StP.ResetAllCache();
        }

        EditorGUI.indentLevel--;
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Check posible missing references and other common errors.
    /// </summary>
    void CheckValidations()
    {
        SpriteToParticles StP = (SpriteToParticles)target;
        if (StP.renderSystemType == RenderSystemUsing.SpriteRenderer)
        {
            //missing sprite Renderer
            if (!StP.spriteRenderer)
                EditorGUILayout.HelpBox("Missing Sprite Renderer", MessageType.Warning);
            else
            {
                #if UNITY_5_5_OR_NEWER
                //sprite Draw mode not supported
                if (StP.spriteRenderer.drawMode != SpriteDrawMode.Simple)
                    EditorGUILayout.HelpBox("Sprite mode not supported", MessageType.Warning);
                #endif
                //sprite is null
                if (!StP.GetSprite())
                    EditorGUILayout.HelpBox("Sprite or Image not defined", MessageType.Warning);
            }
        }
        else
        {
            //missing Image Renderer
            if (!StP.imageRenderer)
                EditorGUILayout.HelpBox("Missing Image Renderer", MessageType.Warning);
            else
            {
                //sprite Draw mode not supported
                if (StP.imageRenderer.type != Image.Type.Simple)
                    EditorGUILayout.HelpBox("Image mode not supported", MessageType.Warning);
                //sprite is null
                if (!StP.GetSprite())
                    EditorGUILayout.HelpBox("Sprite or Image not defined", MessageType.Warning);
            }
            //missing UI Particle Renderer
            if (!StP.uiParticleSystem)
                EditorGUILayout.HelpBox("Missing UI Particle Renderer", MessageType.Warning);
        }

        //missing Particle System
        if (!StP.particlesSystem)
            EditorGUILayout.HelpBox("Missing Particle System", MessageType.Warning);

        //TODO sprite is not marked read/write
    }

    [MenuItem("GameObject/UI/SpriteToParticles/Image + StP", false, 10)]
    public static void CreateUIDynamic(MenuCommand menuCommand)
    {
        GameObject goImg = new GameObject("Image - StP");
        GameObjectUtility.SetParentAndAlign(goImg, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(goImg, "Create " + goImg.name);
        Selection.activeObject = goImg;
        goImg.AddComponent<RectTransform>();

        Image image = goImg.AddComponent<Image>();
        string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("numbloqLogo")[0]);
        image.sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
        image.SetNativeSize();

        GameObject goParticles = new GameObject("Particles - StP");
        GameObjectUtility.SetParentAndAlign(goParticles, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(goParticles, "Create " + goParticles.name);
        Selection.activeObject = goParticles;

        goParticles.AddComponent<RectTransform>();
        ParticleSystem ps = goParticles.AddComponent<ParticleSystem>();
        UIParticleRenderer uipr = goParticles.AddComponent<UIParticleRenderer>();
        SpriteToParticles stP = goParticles.AddComponent<SpriteToParticles>();

        #if UNITY_5_5_OR_NEWER
        ParticleSystem.MainModule mainModule = ps.main;
        mainModule.loop = false;
        mainModule.playOnAwake = false;
        mainModule.duration = 1;
        mainModule.startLifetime = 1;
        mainModule.gravityModifier = 20;
        mainModule.startSize = 10;
        #endif
        ParticleSystem.EmissionModule em = ps.emission;
        em.enabled = false;
        ParticleSystem.ShapeModule sm = ps.shape;
        sm.enabled = false;

        ParticleSystemRenderer psr = goParticles.GetComponent<ParticleSystemRenderer>();
        psr.enabled = false;

        string path2 = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("UI - Particle Add")[0]);
        uipr.material = (Material)AssetDatabase.LoadAssetAtPath(path2, typeof(Material));

        stP.renderSystemType = RenderSystemUsing.ImageRenderer;
        stP.imageRenderer = image;
        stP.UsePixelSourceColor = true;
        stP.ResetAllCache();
    }

    [MenuItem("GameObject/2D Object/SpriteToParticles/Sprite", false, 10)]
    public static void CreateSpriteDynamic(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Sprite - StP");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;

        SpriteRenderer sp = go.AddComponent<SpriteRenderer>();
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        SpriteToParticles stP = go.AddComponent<SpriteToParticles>();

        string path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("numbloqLogo")[0]);
        sp.sprite = (Sprite) AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));

         #if UNITY_5_5_OR_NEWER
        ParticleSystem.MainModule mainModule = ps.main;
        mainModule.loop = false;
        mainModule.playOnAwake = false;
        mainModule.duration = 1;
        mainModule.startLifetime = 1;
        mainModule.gravityModifier = 0.2f;
        #endif
        ParticleSystem.EmissionModule em = ps.emission;
        em.enabled = false;
        ParticleSystem.ShapeModule sm = ps.shape;
        sm.enabled = false;

        ParticleSystemRenderer psr = go.GetComponent<ParticleSystemRenderer>();
        string path2 = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Pixel Material")[0]);
        psr.sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath(path2, typeof(Material));
        psr.sortingOrder = 100;

        stP.UsePixelSourceColor = true;
        stP.ResetAllCache();
    }

    [MenuItem("Edit/SpriteToParticles/Update Components in scene (Experimental)", false, 10)]
    public static void UpdateStP(MenuCommand menuCommand)
    {
        bool sceneChanged = false;
        DynamicEmitter[] des = (DynamicEmitter[]) FindObjectsOfType(typeof(DynamicEmitter));
        foreach (DynamicEmitter de in des)
        {
            Debug.Log("Updating DynamicEmitter to StP for GameObject: " + de.name);
            GameObject g = de.gameObject;
            SpriteToParticles StP = g.AddComponent<SpriteToParticles>();

            StP.renderSystemType = RenderSystemUsing.SpriteRenderer;
            StP.mode = SpriteMode.Dynamic;

            UpdateBaseComponent(StP, de);
            StP.CacheSprites = de.CacheSprites;
            DestroyImmediate(de);
            sceneChanged = true;
        }

        StaticEmitterContinuous[] secs = (StaticEmitterContinuous[])FindObjectsOfType(typeof(StaticEmitterContinuous));
        foreach (StaticEmitterContinuous de in secs)
        {
            Debug.Log("Updating StaticEmitter to StP for GameObject: " + de.name);
            GameObject g = de.gameObject;
            SpriteToParticles StP = g.AddComponent<SpriteToParticles>();

            StP.renderSystemType = RenderSystemUsing.SpriteRenderer;
            StP.mode = SpriteMode.Static;

            UpdateBaseComponent(StP, de);
            StP.CacheOnAwake = de.CacheOnAwake;
            DestroyImmediate(de);
            sceneChanged = true;
        }

        DynamicEmitterUI[] desUI = (DynamicEmitterUI[])FindObjectsOfType(typeof(DynamicEmitterUI));
        foreach (DynamicEmitterUI de in desUI)
        {
            Debug.Log("Updating DynamicEmitterUI to StP for GameObject: " + de.name);
            GameObject g = de.gameObject;
            SpriteToParticles StP = g.AddComponent<SpriteToParticles>();

            StP.renderSystemType = RenderSystemUsing.ImageRenderer;
            StP.mode = SpriteMode.Dynamic;

            UpdateBaseComponentUI(StP, de);
            StP.PlayOnAwake = de.PlayOnAwake;
            StP.EmissionRate = de.EmissionRate;
            StP.CacheSprites = de.CacheSprites;
            DestroyImmediate(de);
            sceneChanged = true;
        }

        StaticEmitterContinuousUI[] secsUI = (StaticEmitterContinuousUI[])FindObjectsOfType(typeof(StaticEmitterContinuousUI));
        foreach (StaticEmitterContinuousUI de in secsUI)
        {
            Debug.Log("Updating DynamicEmitterUI to StP for GameObject: " + de.name);
            GameObject g = de.gameObject;
            SpriteToParticles StP = g.AddComponent<SpriteToParticles>();

            StP.renderSystemType = RenderSystemUsing.ImageRenderer;
            StP.mode = SpriteMode.Static;

            UpdateBaseComponentUI(StP, de);
            StP.PlayOnAwake = de.PlayOnAwake;
            StP.EmissionRate = de.EmissionRate;
            StP.CacheOnAwake = de.CacheOnAwake;
            DestroyImmediate(de);
            sceneChanged = true;
        }
        if (sceneChanged)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    static void UpdateBaseComponent(SpriteToParticles StP, EmitterBase eb)
    {
        StP.spriteRenderer = eb.spriteRenderer;
        StP.particlesSystem = eb.particlesSystem;
        
        StP.PlayOnAwake = eb.PlayOnAwake;
        StP.EmissionRate = eb.EmissionRate;
        StP.UsePixelSourceColor = eb.UsePixelSourceColor;
        StP.UseEmissionFromColor = eb.UseEmissionFromColor;
        
        StP.EmitFromColor = eb.EmitFromColor;
        StP.RedTolerance = eb.RedTolerance;
        StP.GreenTolerance = eb.GreenTolerance;
        StP.BlueTolerance = eb.BlueTolerance;

        StP.useSpritesSharingPool = eb.useSpritesSharingCache;
    }

    static void UpdateBaseComponentUI(SpriteToParticles StP, EmitterBaseUI eb)
    {
        StP.imageRenderer = eb.imageRenderer;
        StP.particlesSystem = eb.particlesSystem;

        StP.UsePixelSourceColor = eb.UsePixelSourceColor;
        StP.UseEmissionFromColor = eb.UseEmissionFromColor;

        StP.EmitFromColor = eb.EmitFromColor; 
        StP.RedTolerance = eb.RedTolerance;
        StP.GreenTolerance = eb.GreenTolerance;
        StP.BlueTolerance = eb.BlueTolerance;

        StP.useSpritesSharingPool = eb.useSpritesSharingCache;
        StP.matchTargetGOPostionData = eb.matchImageRendererPostionData;
        StP.matchTargetGOScale = eb.matchImageRendererScale;
    }

    bool IsRendererInAnotherGameObject(SpriteToParticles stP)
    {
        RenderSystemUsing renderType = (RenderSystemUsing)renderSystemType.intValue;

        if (renderType == RenderSystemUsing.SpriteRenderer && stP.spriteRenderer!=null)
        {
            return stP.spriteRenderer.gameObject != stP.particlesSystem.gameObject;
        }
        if (renderType == RenderSystemUsing.ImageRenderer && stP.imageRenderer != null)
        {
            return stP.imageRenderer.gameObject != stP.particlesSystem.gameObject;
        }

        return true;
    }
}
