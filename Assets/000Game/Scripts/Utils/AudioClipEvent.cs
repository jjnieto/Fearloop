using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AudioClipEvent : UnityEvent<AudioClip> { }

public class AudioRecorder : MonoBehaviour
{
	public AudioClipEvent onRecordingComplete;

	private AudioClip recordedClip;
	private bool isRecording = false;

	// Llama este método al presionar el botón
	public void StartRecording()
	{
		if (!isRecording)
		{
			isRecording = true;
			recordedClip = Microphone.Start(null, false, 10, 44100); // Ajusta la duración y sample rate según necesites
		}
	}

	// Llama este método al soltar el botón
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
