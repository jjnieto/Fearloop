using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{

	/*
	x= -2.59 0, 90, 0
	z=4.413 0, 0, 0
	x=2.59 0, -90, 0
	x=9.584 0, 180, 0
	*/
	public bool closed = true;
	public AudioSource audioSource;
	public AudioClip open,close;
	// Start is called before the first frame update

	private void Start()
	{
		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();
	}
	public void OpenDoor()
    {
        if(closed)
        {
            closed = false;
            GetComponent<Animator>().SetTrigger("Open");
			if (audioSource)
				audioSource.PlayOneShot(open);
        }
    }

	public void ClosedDoor()
	{
		if (!closed)
		{
			closed = true;
			GetComponent<Animator>().SetTrigger("Close");
			if (audioSource)
				audioSource.PlayOneShot(close);
		}
	}

	public void SwitchDoor()
	{
		if (closed)
		{
			OpenDoor();
		}
		else
		{
			ClosedDoor();
		}
	}
}
