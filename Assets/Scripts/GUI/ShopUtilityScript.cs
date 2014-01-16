/// <summary>
/// Enables user to purchase a utility item.
/// </summary>
using UnityEngine;
using System.Collections;

public class ShopUtilityScript : MonoBehaviour {
	
	public Utilities utility;
	private int itemCost;//exposed variable to store the item cost

	private int iTapState = 0;//state of tap on screen
	private RaycastHit hit;//used for detecting taps
	private Camera HUDCamera;//the HUD/Menu orthographic camera
	
	private Transform tBuyButton;
	private TextMesh tmCost;//cost of the utility displayed in shop
	private TextMesh tmOwned;//number of units of the utility currently owned
	
	#region Script References
	private ShopScript hShopScript;
	private InGameController hInGameController;
	private GameController hGameController;
	#endregion
	
	void Start ()
	{
		HUDCamera = (Camera)GameObject.Find("GUIGroup/Camera").GetComponent(typeof(Camera));
		hShopScript = (ShopScript)GameObject.Find("GUIGroup/MenuGroup/Shop").GetComponent(typeof(ShopScript));
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
		hGameController = GameObject.Find("Player").GetComponent<GameController>();
				
		tBuyButton = (Transform)this.transform.Find("Buttons/Button_Buy").GetComponent(typeof(Transform));
		tmCost = (TextMesh)this.transform.Find("CostGroup/Text_Currency").GetComponent(typeof(TextMesh));
		tmOwned = this.transform.Find("Text_Owned").GetComponent<TextMesh>();
		
		itemCost = hGameController.getUtilityPrice(utility);
		tmCost.text = itemCost.ToString();//set the cost of the item as specified
		updateOwnedUtilityText();
		
		setShopUtilityScriptEnabled(false);//turn off current script
	}
	
	void OnGUI ()
	{
		listenerClicks();//listen for clicks on utility shop menu
	}
	
	/*
	*	FUNCTION:	Listen for clicks on the menus and call the relevant handler function on click.
	*	CALLED BY:	FixedUpdate()
	*/
	private void listenerClicks()
	{	
		if (Input.GetMouseButtonDown(0) && iTapState == 0)//detect taps
		{	
			iTapState = 1;		
		}//end of if get mouse button
		else if (iTapState == 1)//call relevent handler
		{
			if (Input.GetMouseButtonUp(0))
				iTapState = 2;
		}
		else if (iTapState == 2)//wait for user to release before detcting next tap
		{
			if (Physics.Raycast(HUDCamera.ScreenPointToRay(Input.mousePosition), out hit))//if a button has been tapped
			{			
				handlerUtilityItem(hit.transform);//call the listner function
			}//end of if raycast
			
			iTapState = 0;
		}
	}//end of listener clicks function
	
	/*
	*	FUNCTION:	Perform function according to the clicked button.
	*	CALLED BY:	listenerClicks()
	*/
	private void handlerUtilityItem(Transform buttonTransform)
	{
		if (buttonTransform == tBuyButton)
		{
			//give the utility to user and deduct the item cost
			if (hGameController.getUserStandardCurrency() >= itemCost)//check if user has enough currency
			{					
				hGameController.updateUserStandardCurrency(-itemCost);//deduct the cost of utility
				hShopScript.updateCurrencyOnHeader();//update the currency on the header bar				
				
				hGameController.updateUtilityOwned(utility);//add a untility item in user's inventory
				updateOwnedUtilityText();
				PlayerPrefs.Save();
			}
		}//end of if
	}
	
	/*
	*	FUNCITON:	Enable or disable the current script.
	*/
	public void setShopUtilityScriptEnabled(bool state)	
	{ 
		this.enabled = state;
		if (state)
			updateOwnedUtilityText();
	}
	
	/// <summary>
	/// Updates the owned utility text on the item element.
	/// </summary>
	private void updateOwnedUtilityText() 
	{ 
		tmOwned.text = "Owned: "+hGameController.getUtilityOwnedCount(utility).ToString(); 
	}
}
