using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SpriteToParticlesAsset;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This mini panel controls the demo scenes
/// </summary>
public class MiniPanel : MonoBehaviour
{
    //reference to emitters
    public List<SpriteToParticles> PlayableFXs;
    public Button PlayButton;
    public Button PauseButton;
    public Toggle WindButton;
    private int SceneCount = 11;

    public WindZone wind;

    /// <summary>
    /// Check sanity of wanted references and attach BecameAvailableToPlay method to all emitters' OnAvailableToPlay event
    /// </summary>
	void Start ()
	{
        if (PlayableFXs == null || PlayableFXs.Count <= 0)
            PlayableFXs = FindObjectsOfType<SpriteToParticles>().ToList();

        if (PlayableFXs == null || PlayableFXs.Count <= 0)
	    {
	        Destroy(gameObject);
            return;
	    }

        if (!wind)
            wind = FindObjectOfType<WindZone>();

        if (!wind)
            WindButton.gameObject.SetActive(false);

        foreach (SpriteToParticles fx in PlayableFXs)
	    {
            if (fx)
	            fx.OnAvailableToPlay += BecameAvailableToPlay;
	    }

        RefreshButtons();
	}

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// If all emitters are playing, pause all, and viceversa.
    /// </summary>
    public void TogglePlay()
    {
        bool isPlaying = PlayableFXs.TrueForAll(x => x.IsPlaying());
        if (isPlaying)
        {
            foreach (SpriteToParticles fx in PlayableFXs)
                fx.Pause();
        }
        else
        {
            foreach (SpriteToParticles fx in PlayableFXs)
                fx.Play();
        }

        RefreshButtons();
    }

    /// <summary>
    /// Stop all referenced emitters 
    /// </summary>
    public void Stop()
    {
        foreach (SpriteToParticles fx in PlayableFXs)
            fx.Stop();

        RefreshButtons();
    }

    public void BecameAvailableToPlay()
    {
        RefreshButtons();
    }

    /// <summary>
    /// Set button states based on emitters situation
    /// </summary>
    public void RefreshButtons()
    {
        bool isPlaying = PlayableFXs.TrueForAll(x => x.IsPlaying());
        PlayButton.gameObject.SetActive(!isPlaying);
        PauseButton.gameObject.SetActive(isPlaying);

        bool available = PlayableFXs.TrueForAll(x => x.IsAvailableToPlay());
        PlayButton.interactable = available;
    }

    /// <summary>
    /// Turns on/off scene wind
    /// </summary>
    public void ToggleWind()
    {
        if (wind)
        {
            wind.gameObject.SetActive(!wind.gameObject.activeInHierarchy);
        }
    }

    private int currentScene = 0;
    public void NextScene()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        currentScene = (currentScene + 1) % SceneCount;
        UnloadCurrentScene();
        //SceneManager.LoadScene(currentScene);
        Invoke("LoadNextScene", 0.1f);

    }
    public void PreviousScene()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        currentScene = (currentScene - 1 + SceneCount) % SceneCount;
        UnloadCurrentScene();
        //SceneManager.LoadScene(currentScene);
        Invoke("LoadNextScene", 0.1f);
    }

    void UnloadCurrentScene()
    {
        foreach (SpriteToParticles fx in PlayableFXs)
        {
            DestroyImmediate(fx.gameObject);
        }
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    void LoadNextScene()
    {
        System.GC.Collect();
        SceneManager.LoadScene(currentScene);
    }
}
