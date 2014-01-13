using UnityEngine;
using System.Collections;

public enum PlayerSounds
{
	Run = 0,
	Jump = 1,
	Duck = 2,
	JumpLand = 3
}

public enum PowerupSounds
{
	CurrencyCollection = 0,
	MagnetismCollection = 1
}

public enum GUISounds
{
	ButtonTap = 0
}

public class SoundController : MonoBehaviour {
	
	#region Variables
	private AudioSource[] asPlayer;
	private AudioSource[] asPowerups;
	private AudioSource[] asGUI;
	#endregion
	
	void Start ()
	{
		asPlayer = new AudioSource[PlayerSounds.GetValues(typeof(PlayerSounds)).Length];
		asPowerups = new AudioSource[PowerupSounds.GetValues(typeof(PowerupSounds)).Length];
		asGUI = new AudioSource[GUISounds.GetValues(typeof(GUISounds)).Length];
		
		asPlayer[0] = this.transform.Find("Player/Run").GetComponent<AudioSource>();
		asPlayer[1] = this.transform.Find("Player/Jump").GetComponent<AudioSource>();
		asPlayer[2] = this.transform.Find("Player/Duck").GetComponent<AudioSource>();
		asPlayer[3] = this.transform.Find("Player/JumpLand").GetComponent<AudioSource>();
		
		asPowerups[0] = this.transform.Find("Powerups/CurrencyCollection").GetComponent<AudioSource>();
		
		asGUI[0] = this.transform.Find("GUI/ButtonTap").GetComponent<AudioSource>();
	}
	
	/// <summary>
	/// Plays the player sound.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	public void playPlayerSound(PlayerSounds type)
	{	
		if (!asPlayer[(int)type].isPlaying)
			asPlayer[(int)type].Play();
	}	
	public void pausePlayerSound(PlayerSounds type)
	{
		if (asPlayer[(int)type].isPlaying)
			asPlayer[(int)type].Pause();
	}
	
	public void playPowerupSound(PowerupSounds type)
	{		
		asPowerups[(int)type].Play();
	}
	public void pausePowerupSound(PowerupSounds type)
	{
		if (asPowerups[(int)type].isPlaying)
			asPowerups[(int)type].Pause();
	}
	
	public void playGUISound(GUISounds type)
	{
		if (!asGUI[(int)type].isPlaying)
			asGUI[(int)type].Play();
	}
	public void pauseGUISound(GUISounds type)
	{
		if (asGUI[(int)type].isPlaying)
			asGUI[(int)type].Pause();
	}
}
