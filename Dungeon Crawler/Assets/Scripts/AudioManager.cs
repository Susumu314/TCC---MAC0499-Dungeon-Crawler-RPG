using UnityEngine.Audio;
using System;
using UnityEngine;
/**
* Classe baseada na aula de audio manager do canal de youtube "Brackeys"
*/
public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;
	private Sound currentMusic = null;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

	void Update()
    {
		if(currentMusic != null){
			if(currentMusic.isIntro && !currentMusic.source.isPlaying){
				AudioManager.instance.Play(currentMusic.mainMusic);
			}
		}
    }

	public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		if(s.isMusic){
			if (currentMusic != null)
				currentMusic.source.Stop();
			currentMusic = s;
		}
		s.source.Play();
	}

	public void Stop(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		s.source.Stop();
	}
	public void StopAllMusic()
	{
		foreach (Sound s in sounds)
		{
			if (s.isMusic)
				s.source.Stop();
		}
	}



}
