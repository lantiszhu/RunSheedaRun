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

public enum Music
{
	Gameplay = 0
}

public class SoundController : MonoBehaviour {
	
	#region Variables
	private AudioSource[] asPlayer;
	private AudioSource[] asPowerups;
	private AudioSource[] asGUI;
	private AudioSource[] asMusic;
	
	private bool MusicEnabled;
	private bool SoundEnabled;
	#endregion
	
	void Start ()
	{
		asPlayer = new AudioSource[PlayerSounds.GetValues(typeof(PlayerSounds)).Length];
		asPowerups = new AudioSource[PowerupSounds.GetValues(typeof(PowerupSounds)).Length];
		asGUI = new AudioSource[GUISounds.GetValues(typeof(GUISounds)).Length];
		asMusic = new AudioSource[Music.GetValues(typeof(Music)).Length];
		
		asPlayer[0] = this.transform.Find("Player/Run").GetComponent<AudioSource>();
		asPlayer[1] = this.transform.Find("Player/Jump").GetComponent<AudioSource>();
		asPlayer[2] = this.transform.Find("Player/Duck").GetComponent<AudioSource>();
		asPlayer[3] = this.transform.Find("Player/JumpLand").GetComponent<AudioSource>();
		
		asPowerups[0] = this.transform.Find("Powerups/CurrencyCollection").GetComponent<AudioSource>();
		
		asGUI[0] = this.transform.Find("GUI/ButtonTap").GetComponent<AudioSource>();
		
		asMusic[0] = this.transform.Find("Music/Gameplay").GetComponent<AudioSource>();
		
		if (PlayerPrefs.HasKey("MusicEnabled"))		
			MusicEnabled = PlayerPrefs.GetInt("MusicEnabled") == 1 ? true : false;
		else
		{
			MusicEnabled = true;
			PlayerPrefs.SetInt("MusicEnabled", 1);			
		}
		
		if (PlayerPrefs.HasKey("SoundEnabled"))
			SoundEnabled = PlayerPrefs.GetInt("SoundEnabled") == 1 ? true : false;
		else
		{
			SoundEnabled = true;
			PlayerPrefs.SetInt("SoundEnabled", 1);
		}
		
		Init();
	}//end of Start function
	
	void Init()
	{
		setMusicState(MusicEnabled);
		setSoundState(SoundEnabled);
	}
	
	public void Restart()
	{
		Init();
	}
	
	/// <summary>
	/// Plays the player sound.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	public void playPlayerSound(PlayerSounds type)
	{	
		if (!asPlayer[(int)type].isPlaying
			&& SoundEnabled)
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
		if (asPowerups[(int)type].isPlaying
			&& SoundEnabled)
			asPowerups[(int)type].Pause();
	}
	
	public void playGUISound(GUISounds type)
	{
		if (!asGUI[(int)type].isPlaying
			&& SoundEnabled)
			asGUI[(int)type].Play();
	}
	public void pauseGUISound(GUISounds type)
	{
		if (asGUI[(int)type].isPlaying
			&& SoundEnabled)
			asGUI[(int)type].Pause();
	}
	
	public void playMusic(Music type)
	{
		if (!asMusic[(int)type].isPlaying)
			asMusic[(int)type].Play();
	}
	public void pauseMusic(Music type)
	{
		if (asMusic[(int)type].isPlaying)
			asMusic[(int)type].Pause();
	}
	
	/// <summary>
	/// Sets the state of the music (Enable or Disable).
	/// </summary>
	/// <param name='state'>
	/// State.
	/// </param>
	public void setMusicState(bool state)
	{	
		if (state)
			playMusic(Music.Gameplay);
		else
			pauseMusic(Music.Gameplay);
		
		MusicEnabled = state;
		PlayerPrefs.SetInt("MusicEnabled", state == true ? 1 : 0);
	}
	
	public bool isMusicEnabled() { return MusicEnabled; }
	
	public void setSoundState(bool state)
	{
		SoundEnabled = state;					
		PlayerPrefs.SetInt("SoundEnabled", state == true ? 1 : 0);
	}
	public bool isSoundEnabled() { return SoundEnabled; }
}
