using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DisplayControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Text display;
    public GameObject displayBkg;
    public string initValue = "*****";
    public int displaySize = 5;
    public string targetValue = "65598";
    public bool hasEnergy = true;

    public AudioClip setOn, setOff, clickOn, clickOff;

    public UnityEvent onMatchText;

    public AudioSource audioSource;

    void Start()
    {
        display.text = initValue;
        if(audioSource==null)
            audioSource = GetComponent<AudioSource>();

        if (hasEnergy)
        {
            displayBkg.SetActive(true);
            display.gameObject.SetActive(true);
        }
        else if (!hasEnergy)
        {
            displayBkg.SetActive(false);
            display.gameObject.SetActive(false);
        }
    }

    public void EnterValue(string value)
	{
        if (hasEnergy)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(clickOn);
        }
        else
        {
            if (audioSource != null)
                audioSource.PlayOneShot(clickOff);
        }

        string newVal = display.text + value;
        display.text = newVal.Substring(Math.Max(0, newVal.Length - displaySize));
        if (display.text == targetValue && onMatchText != null)
            onMatchText.Invoke();
    }
    
    public void SwitchEnergy()
	{
        SetEnergy(!hasEnergy);
	}

    public void SetEnergy(bool energy)
	{
        
        if (!hasEnergy && energy)
        {
            displayBkg.SetActive(true);
            display.gameObject.SetActive(true);
            if (audioSource != null)
                audioSource.PlayOneShot(setOn);

        }
        else if(hasEnergy && !energy)
        {
            displayBkg.SetActive(false);
            display.gameObject.SetActive(false);
            audioSource.PlayOneShot(setOff);
        }
        hasEnergy = energy;
    }
}
