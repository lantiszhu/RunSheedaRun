using UnityEngine;
using System.Collections;

public class PowerupElement : MonoBehaviour {
	
	#region Exposed Variables
	public Powerups powerupType;
<<<<<<< HEAD
	public int spoolSize = 1;
	 
=======
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
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
<<<<<<< HEAD
	private float MinPlayerDistance = 100;

	private InGameController hInGameController;
	private PowerupController hPowerupController;	
	private ElementsGenerator hElementsGenerator;

=======
	
	private InGameController hInGameController;
	private PowerupController hPowerupController;	
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
	#endregion
		
	void Start () 
	{
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
		hPowerupController = (PowerupController)GameObject.Find("Player").GetComponent(typeof(PowerupController));
<<<<<<< HEAD
		hElementsGenerator = GameObject.Find("Controllers").GetComponent<ElementsGenerator>();		
=======
				
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
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
<<<<<<< HEAD
				if (powerupType == Powerups.StandardCurrency)
				{
					hElementsGenerator.updateCoinPos(transform);
				}
				//TODO: Place this element back in the queue
				//this.gameObject.SetActive(false);//disable this element
				
				CollectionState = 0;//escape the loop
=======
								
				//TODO: Place this element back in the queue
				this.gameObject.SetActive(false);//disable this element
				
				CollectionState = 2;//escape the loop
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
			}
		}
		
		tElementMesh.localEulerAngles = new Vector3(tElementMesh.localEulerAngles.x,
			(Time.deltaTime*120)+tElementMesh.localEulerAngles.y, tElementMesh.localEulerAngles.z);
	}//end of fixed update
}
