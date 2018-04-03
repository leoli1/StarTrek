using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectTypes{
	Explosion,
	Phaser
}

public static class AudioMaster {

	public static void play_random_sound_effect(SoundEffectTypes type, Vector3 position){
		play_random_sound_effect (type, position, 1);
	}
	public static void play_random_sound_effect(SoundEffectTypes type, Vector3 position, float volume){
		AudioClip[] clips = null;
		switch (type) {
		case SoundEffectTypes.Explosion:
			clips = AudioAssets.audio_assets.explosions;
			break;
		case SoundEffectTypes.Phaser:
			clips = AudioAssets.audio_assets.phasers;
			break;
		}
		if (clips.Length == 0)
			return;
		int i = Random.Range (0, clips.Length - 1);
		AudioClip clip = clips [i];
		AudioSource.PlayClipAtPoint (clip, position, volume);
	}
}
