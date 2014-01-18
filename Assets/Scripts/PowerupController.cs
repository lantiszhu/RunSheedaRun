using UnityEngine;
using System.Collections;

public enum Powerups 
{ 
	Magnetism = 0, 
	StandardCurrency = 1,
	PremiumCurrency = 2
}

public enum Utilities
{
	Headstart = 0,
	MegaHeadstart = 1,
	ScoreBooster = 2,
	MegaScoreBooster = 3
}
public struct Utility
{
	public int cost;
	public float duration;	
	public int ownedCount;
}

public class PowerupController : MonoBehaviour {
	
	#region Contants
	private const float defaultElementPullDistance = 1;//default pull value
	private const float magnetismPullDistance = 5;//pull value when magnetism is active	
	#endregion
	
	#region Variables
	private int powerupCount;//total types of powerups
	private int utilityCount;//total types of utilities
	private float elementPullDistance;	//when to pull currency towards the player
	private float powerupStartTime;
	
	private Utility[] utilityData;
	private float[] powerupActiveDuration;//number of seconds to keep a powerup active
	
	private int collectedStandardCurrency;//number of standard currency collected in the current run
	private int collectedPremiumCurrency;//number of premium currency collected in the current run
	#endregion
	
	#region Script References
	private MissionsController hMissionsController;
	private SoundController hSoundController;
	#endregion
	
	void Start () 
	{
		hMissionsController = this.GetComponent<MissionsController>();
		hSoundController = GameObject.Find("SoundManager").GetComponent<SoundController>();
<<<<<<< HEAD
=======
		
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
		powerupCount = Powerups.GetValues(typeof(Powerups)).Length-2;//get the number of powerup types
						
		powerupActiveDuration = new float[powerupCount];
		for (int i=0; i<powerupCount; i++)//TODO: proper implementation for powerup upgrades
			powerupActiveDuration[i] = 5;
		
		populateUtilityDataStruct();
		Init();
	}
<<<<<<< HEAD
	
	void Init()
	{
		collectedStandardCurrency = 0;
		collectedPremiumCurrency = 0;
		elementPullDistance = defaultElementPullDistance;
	}
	
	public void Restart()
	{
		Init();
	}
	
	/// <summary>
	/// Handles what to do when a pickable object is collected
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	public void handleElementCollection(Powerups type)
	{
		if (type == Powerups.StandardCurrency)
		{
			collectedStandardCurrency ++;
			//tell the Missions Controller that a standard currency unit has been collected
			hMissionsController.incrementMissionCount(MissionTypes.StandardCurrency);
			
			hSoundController.playPowerupSound(PowerupSounds.CurrencyCollection);


		}
		else if (type == Powerups.PremiumCurrency)
			collectedPremiumCurrency ++;
		else//if a power-up has been collected
		{
			activatePowerup(type);
			
			if (type == Powerups.Magnetism)//tell Mission Controller if a magnetism power-up is collected
				hMissionsController.incrementMissionCount(MissionTypes.MagnetismPowerup);
			
			//tell the Missions Controller that a power-up has been picked up
			hMissionsController.incrementMissionCount(MissionTypes.Powerups);
		}		
	}//end of powerup collection handler
	
	/// <summary>
	/// Activates a powerup.
	/// </summary>
	/// <param name='powerup'>
	/// Powerup.
	/// </param>
	public void activatePowerup(Powerups powerup)
	{
		if (powerup == Powerups.Magnetism)
		{
			elementPullDistance = magnetismPullDistance;
		}
		
		powerupStartTime = Time.time;
		StartCoroutine(countdownPowerupDeactivation(powerup));
	}
	
=======
	
	void Init()
	{
		collectedStandardCurrency = 0;
		collectedPremiumCurrency = 0;
		elementPullDistance = defaultElementPullDistance;
	}
	
	public void Restart()
	{
		Init();
	}
	
	/// <summary>
	/// Handles what to do when a pickable object is collected
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	public void handleElementCollection(Powerups type)
	{
		if (type == Powerups.StandardCurrency)
		{
			collectedStandardCurrency ++;
			//tell the Missions Controller that a standard currency unit has been collected
			hMissionsController.incrementMissionCount(MissionTypes.StandardCurrency);
			
			hSoundController.playPowerupSound(PowerupSounds.CurrencyCollection);
		}
		else if (type == Powerups.PremiumCurrency)
			collectedPremiumCurrency ++;
		else//if a power-up has been collected
		{
			activatePowerup(type);
			
			if (type == Powerups.Magnetism)//tell Mission Controller if a magnetism power-up is collected
				hMissionsController.incrementMissionCount(MissionTypes.MagnetismPowerup);
			
			//tell the Missions Controller that a power-up has been picked up
			hMissionsController.incrementMissionCount(MissionTypes.Powerups);
		}		
	}//end of powerup collection handler
	
	/// <summary>
	/// Activates a powerup.
	/// </summary>
	/// <param name='powerup'>
	/// Powerup.
	/// </param>
	public void activatePowerup(Powerups powerup)
	{
		if (powerup == Powerups.Magnetism)
		{
			elementPullDistance = magnetismPullDistance;
		}
		
		powerupStartTime = Time.time;
		StartCoroutine(countdownPowerupDeactivation(powerup));
	}
	
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
	/// <summary>
	/// Countdowns the powerup deactivation.
	/// </summary>
	/// <returns>
	/// The powerup deactivation.
	/// </returns>
	/// <param name='powerup'>
	/// Powerup.
	/// </param>
	private IEnumerator countdownPowerupDeactivation(Powerups powerup)
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();
			
			if ( (Time.time-powerupStartTime) >= powerupActiveDuration[(int)powerup])
			{
				deactivatePowerup(powerup);
				break;
			}
		}//end of while
<<<<<<< HEAD
=======
		
		StopCoroutine("countdownPowerupDeactivation");
	}
	
	/// <summary>
	/// Deactivates the powerup.
	/// </summary>
	/// <param name='powerup'>
	/// Powerup.
	/// </param>
	public void deactivatePowerup(Powerups powerup)
	{
		if (powerup == Powerups.Magnetism)
		{
			elementPullDistance = defaultElementPullDistance;
		}
	}
	public void deactivateAllPowerups()
	{
		for (int i=0; i<powerupCount; i++)
			deactivatePowerup( (Powerups)i );
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
		
		StopCoroutine("countdownPowerupDeactivation");
	}
	
<<<<<<< HEAD
	/// <summary>
	/// Deactivates the powerup.
	/// </summary>
	/// <param name='powerup'>
	/// Powerup.
	/// </param>
	public void deactivatePowerup(Powerups powerup)
	{
		if (powerup == Powerups.Magnetism)
		{
			elementPullDistance = defaultElementPullDistance;
		}
	}
	public void deactivateAllPowerups()
	{
		for (int i=0; i<powerupCount; i++)
			deactivatePowerup( (Powerups)i );
		
		StopCoroutine("countdownPowerupDeactivation");
	}
	
	public float getElementPullDistance() { return elementPullDistance; }
	public int getCollectedStandardCurrency() { return collectedStandardCurrency; }
	public int getCollectedPremiumCurrency() { return collectedPremiumCurrency; }
	
	public Utility getUtilityData(Utilities type) { return utilityData[(int)type]; }	
	public int getUtilityCount() { return utilityCount; }
	private void populateUtilityDataStruct()
	{
=======
	public float getElementPullDistance() { return elementPullDistance; }
	public int getCollectedStandardCurrency() { return collectedStandardCurrency; }
	public int getCollectedPremiumCurrency() { return collectedPremiumCurrency; }
	
	public Utility getUtilityData(Utilities type) { return utilityData[(int)type]; }	
	public int getUtilityCount() { return utilityCount; }
	private void populateUtilityDataStruct()
	{
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
		utilityCount = Utilities.GetValues(typeof(Utilities)).Length;
		utilityData = new Utility[utilityCount];
				
		for (int i=0; i<utilityCount; i++)
		{						
			if (PlayerPrefs.HasKey("Utility_"+i.ToString()))
				utilityData[i].ownedCount = PlayerPrefs.GetInt("Utility_"+i.ToString());
			else
			{
				utilityData[i].ownedCount = 0;
				PlayerPrefs.SetInt("Utility_"+i.ToString(), utilityData[i].ownedCount);
			}
		}//end of for
		
		//set the cost of each utility
		utilityData[(int)Utilities.Headstart].cost = 100;
		utilityData[(int)Utilities.MegaHeadstart].cost = 100;
		utilityData[(int)Utilities.ScoreBooster].cost = 100;
		utilityData[(int)Utilities.MegaScoreBooster].cost = 100;
		
		//set the duration of each utility
		utilityData[(int)Utilities.Headstart].duration = 10;
		utilityData[(int)Utilities.MegaHeadstart].duration = 15;
		
	}//end of populate Utility Data Struct function
	
	public void updateUtilityOwnedCount(Utilities type, int count) 
	{ 
		utilityData[(int)type].ownedCount += count;
		PlayerPrefs.SetInt("Utility_"+ ((int)type).ToString(),
			utilityData[(int)type].ownedCount);
	}
}
