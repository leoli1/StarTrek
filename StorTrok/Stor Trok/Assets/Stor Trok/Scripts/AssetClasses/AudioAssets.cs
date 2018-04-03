using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAssets : MonoBehaviour {

	public static AudioAssets audio_assets;

	public AudioClip[] explosions;

	public AudioClip[] phasers;

	void Awake () {
		AudioAssets.audio_assets = this;
	}
}
