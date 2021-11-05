using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIExampleController : MonoBehaviour
{
    public Toggle maskToggle;
    public Image maskBorder;

    public List<Image> maskImages;
    public Sprite[] windowSprites;
    public Image window;

    public Image health;
    public Image mana;

    public Slider healthSlider;
    public Slider manaSlider;

	// Use this for initialization
	void Start () 
    {
	    maskImages = new List<Image>();

	    Mask[] masks = FindObjectsOfType<Mask>();

	    foreach (Mask mask1 in masks)
	    {
	        maskImages.Add(mask1.GetComponent<Image>());
	    }

    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void ToggleMask()
    {
        foreach (Image maskImage in maskImages)
        {
            maskImage.enabled = maskToggle.isOn;    
        }
        maskBorder.enabled = maskToggle.isOn;
    }

    public void ChangeWindowType(int i)
    {
        window.sprite = windowSprites[i];
    }

    public void OnSlidersChanged()
    {
        mana.rectTransform.sizeDelta = new Vector2(mana.rectTransform.sizeDelta.x, manaSlider.value * 240);
        health.rectTransform.sizeDelta = new Vector2(health.rectTransform.sizeDelta.x, healthSlider.value * 240);
    }
}
