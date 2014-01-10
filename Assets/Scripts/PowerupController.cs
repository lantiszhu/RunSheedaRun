using UnityEngine;
using System.Collections;

public enum Powerups 
{ 
	Magnetism = 0, 
	StandardCurrency = 1,
	PremiumCurrency = 2
}

public class PowerupController : MonoBehaviour {
	
	#region Contants
	private const float defaultElementPullDistance = 1;//default pull value
	private const float magnetismPullDistance = 5;//pull value when magnetism is active
	#endregion
	
	#region Variables
	private int powerupCount;//total types of powerups
	private float elementPullDistance;	//when to pull currency towards the player
	private float powerupStartTime;
	
	private float[] powerupActiveDuration;//number of seconds to keep a powerup active
	
	private int collectedStandardCurrency;//number of standard currency collected in the current run
	private int collectedPremiumCurrency;//number of premium currency collected in the current run
	#endregion
	
	void Start () 
	{
		powerupCount = Powerups.GetValues(typeof(Powerups)).Length-2;//get the number of powerup types
		elementPullDistance = defaultElementPullDistance;
		
		powerupActiveDuration = new float[powerupCount];
		for (int i=0; i<powerupCount; i++)//TODO: discuss a proper implementation for powerup upgrades
			powerupActiveDuration[i] = 5;
		
		Init();
	}
	
	void Init()
	{
		collectedStandardCurrency = 0;
		collectedPremiumCurrency = 0;
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
			collectedStandardCurrency ++;		
		else if (type == Powerups.PremiumCurrency)
			collectedPremiumCurrency ++;
		else
		{
			activatePowerup(type);
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
	
	public float getElementPullDistance() { return elementPullDistance; }
	public int getCollectedStandardCurrency() { return collectedStandardCurrency; }
}
