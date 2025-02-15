using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AudioClipEvent : UnityEvent<AudioClip> { }

public class AudioRecorder : MonoBehaviour
{
	public AudioClipEvent onRecordingComplete;

	private AudioClip recordedClip;
	private bool isRecording = false;

	// Llama este m�todo al presionar el bot�n
	public void StartRecording()
	{
		if (!isRecording)
		{
			isRecording = true;
			recordedClip = Microphone.Start(null, false, 10, 44100); // Ajusta la duraci�n y sample rate seg�n necesites
		}
	}

	// Llama este m�todo al soltar el bot�n
	public void StopRecording()
	{
		if (isRecording)
		{
			isRecording = false;
			Microphone.End(null);

			// Invoca el evento al terminar de grabar y pasa el AudioClip
			onRecordingComplete.Invoke(recordedClip);
		}
	}
}
