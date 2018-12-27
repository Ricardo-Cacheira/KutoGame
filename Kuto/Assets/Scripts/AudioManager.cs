using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour {
	public static AudioManager i;
	public Sound[] sounds;

	private void Awake() {
		i = this;
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;

			s.source.volume = s.volume;
		}
	}

	public void Play (string name, bool looping = false)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		s.source.Play();
		s.source.loop = looping;
	}

	public void Stop (string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);
		s.source.Stop();
	}

}
