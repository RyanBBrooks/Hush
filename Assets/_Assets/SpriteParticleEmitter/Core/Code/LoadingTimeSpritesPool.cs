using System.Collections.Generic;
using UnityEngine;

namespace SpriteToParticlesAsset
{
/// <summary>
/// Use this component if you want to load the sprites at a particular moment in the game. 
/// For the StP component to work in conjunction with this the Use Sprites Sharing Pool option must be enabled.
/// </summary>
public class LoadingTimeSpritesPool : MonoBehaviour
{
    [Tooltip("Drag here all the sprites to be loaded in the pool.")]
    //! Drag here all the sprites to be loaded in the pool.
    public List<Sprite> spritesToLoad;

    [Tooltip("If enabled the load will be called on this GameObject’s Awake method. Otherwise it can be called by the method LoadAll() ")]
    //! If enabled the load will be called on this GameObject’s Awake method. Otherwise it can be called by the method LoadAll() 
    public bool loadAllOnAwake;

    /// <summary>
    /// Define whether the Loading of all Sprites must be done on Awake
    /// </summary>
    private void Awake()
    {
        if (loadAllOnAwake)
            LoadAll();
    }

    /// <summary>
    /// Load all referenced Sprites to the SpritesDataPool.
    /// </summary>
    public void LoadAll()
    {
        //Go over all referenced sprites
        foreach (Sprite sprite in spritesToLoad)
        {
            Rect spriteRect = sprite.rect;
            //Add current sprite to the SpritesDataPool 
            SpritesDataPool.GetSpriteColors(sprite,
                (int) spriteRect.position.x,
                (int) spriteRect.position.y,
                (int) spriteRect.size.x,
                (int) spriteRect.size.y);
        }
    }
}
}