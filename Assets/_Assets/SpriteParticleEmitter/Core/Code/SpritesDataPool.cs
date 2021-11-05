using System.Collections.Generic;
using UnityEngine;

namespace SpriteToParticlesAsset
{
/// <summary>
/// Static Pool for Sprite textures data.
/// </summary>
public class SpritesDataPool
{
    //! Dictionary containing all loaded sprites.
    private static Dictionary<Sprite, Color[]> spritesShared = new Dictionary<Sprite, Color[]>();

    /// <summary>
    /// Get the wanted Color[] for wanted sprite. If the Color[] information for that sprite doesn't exist yet texture's GetPixels() method will be called
    /// </summary>
    /// <param name="sprite">Wanted sprite</param>
    /// <param name="x">X position on texture for this Sprite (in pixels)</param>
    /// <param name="y">X position on texture for this Sprite (in pixels)</param>
    /// <param name="blockWidth">width size on texture for this Sprite (in pixels)</param>
    /// <param name="blockHeight">width size on texture for this Sprite (in pixels)</param>
    /// <returns>The Color[] data for wanted sprite.</returns>
    public static Color[] GetSpriteColors(Sprite sprite, int x, int y, int blockWidth, int blockHeight)
    {
        Color[] pix;
        //if pool doesn't exist, create it.
        if (spritesShared == null)
            spritesShared = new Dictionary<Sprite, Color[]>();

        //if the sprite doesn't is already in the pool
        if (!spritesShared.ContainsKey(sprite))
        {
            //Get pixels data for wanted sprite
            pix = sprite.texture.GetPixels(x, y, blockWidth, blockHeight);
            //add it to the pool
            spritesShared.Add(sprite, pix);
        }
        else
        {
            // get wanted Color[] data for this sprite.
            pix = spritesShared[sprite];
        }

        return pix;
    }

    /// <summary>
    /// Will release all memory for the pool.
    /// </summary>
    public static void ReleaseMemory()
    {
        spritesShared = null;
    }
}
}