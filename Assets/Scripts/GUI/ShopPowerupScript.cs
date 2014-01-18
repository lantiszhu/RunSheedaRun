/*
*	FUNCTION:
*	- Enables user to upgrade a powerup.
*/
using UnityEngine;
using System.Collections;

public class ShopPowerupScript : MonoBehaviour {

	public Powerups powerup;
	private Powerup powerupData;
	
	private int iTapState = 0;//state of tap on screen
	private RaycastHit hit;//used for detecting taps
	private Camera HUDCamera;//the HUD/Menu orthographic camera
	
	private Transform tBuyButton;
	private TextMesh tmCost;
	
	#region Script References
	private ShopScript hShopScript;
	private GameController hGameController;
	private PowerupController hPowerupController;
	#endregion
	
	void Start ()
	{
		HUDCamera = (Camera)GameObject.Find("GUIGroup/Camera").GetComponent(typeof(Camera));
		hShopScript = GameObject.Find("GUIGroup/MenuGroup/Shop").GetComponent<ShopScript>();
		hGameController = GameObject.Find("Player").GetComponent<GameController>();
		hPowerupController = GameObject.Find("Player").GetComponent<PowerupController>();
				
		tBuyButton = (Transform)this.transform.Find("Buttons/Button_Buy").GetComponent(typeof(Transform));
		tmCost = (TextMesh)this.transform.Find("CostGroup/Text_Currency").GetComponent(typeof(TextMesh));
		
		powerupData = hPowerupController.getPowerupData(powerup);
		tmCost.text = powerupData.upgradeCost[powerupData.currentLevel].ToString();//set the cost of the item as specified by the user
				
		updatePowerupDescription();//Update the text on the power-up item in shop
		
		setShopPowerupScriptEnabled(false);//turn off current script
	}
	
	void OnGUI () 
	{
		listenerClicks();//listen for clicks on costume menu
	}
	
	/// <summary>
	/// Listen for clicks on the menus and call the relevant handler function on click.
	/// </summary>
	private void listenerClicks()
	{	
		if (Input.GetMouseButtonDown(0) && iTapState == 0)//detect taps
		{	
			iTapState = 1;		
		}//end of if get mouse button
		else if (iTapState == 1)//call relevent handler
		{
			if (Physics.Raycast(HUDCamera.ScreenPointToRay(Input.mousePosition), out hit))//if a button has been tapped
			{			
				handlerPowerupItem(hit.transform);//call the listner function
			}//end of if raycast
			
			iTapState = 2;
		}
		else if (iTapState == 2)//wait for user to release before detcting next tap
		{
			if (Input.GetMouseButtonUp(0))
				iTapState = 0;
		}
	}//end of listener clicks function
	
	/// <summary>
	/// Perform function according to the clicked button.
	/// </summary>
	/// <param name='buttonTransform'>
	/// Button transform.
	/// </param>
	private void handlerPowerupItem(Transform buttonTransform)
	{
		if (buttonTransform == tBuyButton)
		{
			//increase the powerup level
			if (powerupData.currentLevel < powerupData.levelCount //check if the max level has not been achieved
			&& hGameController.getUserStandardCurrency() >= powerupData.upgradeCost[powerupData.currentLevel])//check if user has enough currency
			{	
				hPowerupController.upgradePowerupLevel(powerup);//increase the power-up level
						
				hGameController.updateUserStandardCurrency(-powerupData.upgradeCost[powerupData.currentLevel]);//deduct the cost of power-up upgrade
				hShopScript.updateCurrencyOnHeader();//update the currency on the header bar
				
				powerupData = hPowerupController.getPowerupData(powerup);
				//Update the text on the power-up item in shop
				updatePowerupDescription();
				PlayerPrefs.Save();
			}
		}//end of if
	}
	
	private void updatePowerupDescription()
	{
		(this.transform.Find("Text_ItemLevel").GetComponent("TextMesh") as TextMesh).text = 
			"Level " + (powerupData.currentLevel+1).ToString();
	}
	
	/// <summary>
	/// Enable or disable the current script.
	/// </summary>
	/// <param name='state'>
	/// State.
	/// </param>
	public void setShopPowerupScriptEnabled(bool state)
	{	
		this.enabled = state;	
	}
}
