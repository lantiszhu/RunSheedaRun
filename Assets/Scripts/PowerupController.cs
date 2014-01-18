using UnityEngine;
using System.Collections;

public enum Powerups 
{ 
	Magnetism = 0, 
	StandardCurrency = 1,
	PremiumCurrency = 2
}
public struct Powerup
{
	public int levelCount;
	public int currentLevel;
	public float[] duration;
	public int[] upgradeCost;
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
	public int ownedCount;
	//any value associated with the utility
	//multiplier in case of score booster and time duration
	// in case of headstart
	public float upgradeValue;
}

public class PowerupController : MonoBehaviour {
	
	#region Contants
	private const float defaultElementPullDistance = 1;//default pull value
	private const float magnetismPullDistance = 5;//pull value when magnetism is active	
	#endregion
	
	#region Variables
	private int powerupCount;//total types of powerups
	private Powerup[] powerups;
	private float elementPullDistance;	//when to pull currency towards the player
	private float powerupStartTime;
	
	private int utilityCount;//total types of utilities
	private Utility[] utilityData;	
	
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
		
		populatePowerupDataStruct();
		populateUtilityDataStruct();
		Init();
	}
	
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
	public void activatePowerup(Powerups type)
	{
		if (type == Powerups.Magnetism)
		{
			elementPullDistance = magnetismPullDistance;
		}
		
		powerupStartTime = Time.time;
		StartCoroutine(countdownPowerupDeactivation(type));
	}
	
	/// <summary>
	/// Countdowns the powerup deactivation.
	/// </summary>
	/// <returns>
	/// The powerup deactivation.
	/// </returns>
	/// <param name='powerup'>
	/// Powerup.
	/// </param>
	private IEnumerator countdownPowerupDeactivation(Powerups type)
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();
			
			if ( (Time.time-powerupStartTime) >= 
				powerups[(int)type].duration[ powerups[(int)type].currentLevel ])
			{
				deactivatePowerup(type);
				break;
			}
		}//end of while
		
		StopCoroutine("countdownPowerupDeactivation");
	}
	
	/// <summary>
	/// Deactivates the powerup.
	/// </summary>
	/// <param name='powerup'>
	/// Powerup.
	/// </param>
	public void deactivatePowerup(Powerups type)
	{
		if (type == Powerups.Magnetism)
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
		
		//set the duration of headstart utility
		utilityData[(int)Utilities.Headstart].upgradeValue = 10;
		utilityData[(int)Utilities.MegaHeadstart].upgradeValue = 15;
		//set the multiplier bonus of the score booster utility
		utilityData[(int)Utilities.ScoreBooster].upgradeValue = 5;
		utilityData[(int)Utilities.MegaScoreBooster].upgradeValue = 10;
		
	}//end of populate Utility Data Struct function
	
	/// <summary>
	/// Updates the utility owned count.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	/// <param name='count'>
	/// Count.
	/// </param>
	public void updateUtilityOwnedCount(Utilities type, int count) 
	{ 
		utilityData[(int)type].ownedCount += count;
		PlayerPrefs.SetInt("Utility_"+ ((int)type).ToString(),
			utilityData[(int)type].ownedCount);
	}
	
	private void populatePowerupDataStruct()
	{
		powerupCount = Powerups.GetValues(typeof(Powerups)).Length-2;//get the number of powerup types
		powerups = new Powerup[powerupCount];//allocate memory according to the number of powerups
		
		for (int i=0; i<powerupCount; i++)
		{
			powerups[i].levelCount = 4;	//total upgrade levels
			powerups[i].duration = new float[powerups[i].levelCount];
			powerups[i].upgradeCost = new int[powerups[i].levelCount-1];
			
			if (PlayerPrefs.HasKey("Powerup_"+i.ToString()))
				powerups[i].currentLevel = PlayerPrefs.GetInt("Powerup_"+i.ToString());
			else
			{
				powerups[i].currentLevel = 0;
				PlayerPrefs.SetInt("Powerup_"+i.ToString(), powerups[i].currentLevel);
			}
		}//end of for
		
		//duration in seconds of each powerup
		powerups[(int)Powerups.Magnetism].duration[0] = 7;
		powerups[(int)Powerups.Magnetism].duration[1] = 10;
		powerups[(int)Powerups.Magnetism].duration[2] = 13;
		powerups[(int)Powerups.Magnetism].duration[3] = 16;
		//upgrade cost for each powerup level
		powerups[(int)Powerups.Magnetism].upgradeCost[0] = 100;
		powerups[(int)Powerups.Magnetism].upgradeCost[1] = 100;
		powerups[(int)Powerups.Magnetism].upgradeCost[2] = 100;
	}//end of populate powerup data struct	
	public Powerup getPowerupData(Powerups type)
	{
		return powerups[(int)type];
	}	
	public void upgradePowerupLevel(Powerups type)
	{
		powerups[(int)type].currentLevel ++;
		PlayerPrefs.SetInt("Powerup_"+((int)type).ToString(), 
			powerups[(int)type].currentLevel);
	}
}
