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
	private const float defaultElementPullDistance = 3;
	#endregion
	
	#region Variables
	private float elementPullDistance;
	
	private int collectedStandardCurrency;
	#endregion
	
	void Start () 
	{
		elementPullDistance = defaultElementPullDistance;
		
		Init();
	}
	
	void Init()
	{
		collectedStandardCurrency = 0;
	}
	
	public void Restart()
	{
		Init();
	}
	
	public void handlerPowerupCollection(Powerups type)
	{
		if (type == Powerups.StandardCurrency)
		{
			collectedStandardCurrency ++;
		}
	}
	
	public float getElementPullDistance() { return elementPullDistance; }
	public int getCollectedStandardCurrency() { return collectedStandardCurrency; }
}
