using UnityEngine;
using System.Collections;

public class PowerupElement : MonoBehaviour {
	
	#region Exposed Variables
	public Powerups powerupType;
	#endregion
	
	#region Constants
	private const float pullSpeed = 1;
	private const float elementContactDistance = 0.5f;
	#endregion
	
	#region Variables
	private Transform tPlayer;	//player's transform
	private Transform tPowerupElement;//the currently active element
	
	private int CollectionState;
	
	private InGameController hInGameController;
	private PowerupController hPowerupController;	
	#endregion
		
	void Start () 
	{
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
		hPowerupController = (PowerupController)GameObject.Find("Player").GetComponent(typeof(PowerupController));
				
		tPowerupElement = this.transform;
		tPlayer = GameObject.Find("Player").transform;		
	}
	
	public void Init()
	{
		CollectionState = 0;
	}
		
	void FixedUpdate () 
	{
		if (CollectionState == 0)//wait to get close enough
		{
			if (Vector3.Distance(tPlayer.position, tPowerupElement.position)
				<= hPowerupController.getElementPullDistance())
			{
				CollectionState = 1;				
			}
		}//end of state 0
		else if (CollectionState == 1)
		{
			tPowerupElement.position = Vector3.Lerp(tPowerupElement.position, tPlayer.position, Time.deltaTime*pullSpeed);
			
			if (Vector3.Distance(tPlayer.position, tPowerupElement.position) == elementContactDistance)
			{
				hPowerupController.handlerPowerupCollection(powerupType);
				//disable this element
				CollectionState = 2;//escape the loop
			}
		}
		
	}//end of fixed update
}
