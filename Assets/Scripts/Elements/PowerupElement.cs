using UnityEngine;
using System.Collections;

public class PowerupElement : MonoBehaviour {
	
	#region Exposed Variables
	public Powerups powerupType;
	#endregion
	
	#region Constants
	private const float pullSpeed = 5;
	private const float elementContactDistance = 0.75f;
	#endregion
	
	#region Variables
	private Transform tPlayerCharacter;	//player's transform
	private Transform tPowerupElement;//the currently active element
	private Transform tElementMesh;
	
	private int CollectionState;
	
	private InGameController hInGameController;
	private PowerupController hPowerupController;	
	#endregion
		
	void Start () 
	{
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
		hPowerupController = (PowerupController)GameObject.Find("Player").GetComponent(typeof(PowerupController));
				
		tPowerupElement = this.transform;		
		tPlayerCharacter = GameObject.Find("Player/CharacterGroup").transform;
		tElementMesh = this.transform.Find("Mesh").transform;
	}
	
	public void Init()
	{
		CollectionState = 0;
	}
		
	void FixedUpdate () 
	{
		if (CollectionState == 0)//wait to get close enough
		{
			if (Vector3.Distance(tPlayerCharacter.position, tPowerupElement.position)
				<= hPowerupController.getElementPullDistance())
			{
				CollectionState = 1;				
			}
		}//end of state 0
		else if (CollectionState == 1)
		{
			tPowerupElement.position = Vector3.Lerp(tPowerupElement.position, tPlayerCharacter.position, Time.deltaTime*pullSpeed);
			tPowerupElement.localScale = Vector3.Lerp(tPowerupElement.localScale, new Vector3(0,0,0), Time.deltaTime);
			
			if (Vector3.Distance(tPlayerCharacter.position, tPowerupElement.position) <= elementContactDistance)
			{
				hPowerupController.handleElementCollection(powerupType);//handle what to do with the collected item
								
				//TODO: Place this element back in the queue
				this.gameObject.SetActive(false);//disable this element
				
				CollectionState = 2;//escape the loop
			}
		}
		
		tElementMesh.localEulerAngles = new Vector3(tElementMesh.localEulerAngles.x,
			(Time.deltaTime*120)+tElementMesh.localEulerAngles.y, tElementMesh.localEulerAngles.z);
	}//end of fixed update
}
