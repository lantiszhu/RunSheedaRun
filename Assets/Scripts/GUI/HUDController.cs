using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
	#region Constants
	private const float scoreCoreMultiplier = 0.3f;//multiplicative factor of distance covered in delta time
	
	private const float utilityIconDisplacementY = 22;//distance between two utility icons
	private const float utilityIconDisplayDuration = 5;//time duration to display utility icons
	#endregion
	
	private RaycastHit hit;
	private int iTapState;
	private Camera HUDCamera;
	private float currentDeltaTimeScore;//score earned in delta time
	private float accumulatedScore;//total score in the current run
	private float missionRecordScore;//used for counting score for missions
	
	private TextMesh tmScore;
	private TextMesh tmCurrency;
	private TextMesh tmMultiplier;
	
	#region Utility Variables
	private int utilityIconState;
	private Transform[] tUtilityButtons;
	private float utilityIconPositionY;//where to place the utility icon
	private float utilityIconDisplayTimeStart;
	#endregion
	
	private Transform tHUDScoreContainer;
	private Transform tCurrencyIcon;
	private Transform tHUDCurrencyContainer;	
	private Transform tHUDMultiplierContainer;
	//default size/ position of the containers
	private Vector3 scoreContainerScale;
	private Vector3 currencyContainerScale;
	private Vector3 multiplierContainerScale;
	private Vector3 currencyIconPosition;
	
	//HUD element Container sizes
	private int iDivisorScore;
	private int iDivisorCurrency;
	private int iDivisorMultiplier;
	
	#region Script References
	private GameController hGameController;
	private InGameController hInGameController;
	private PlayerController hPlayerController;
	private PowerupController hPowerupController;
	private MissionsController hMissionsController;
	#endregion
	
	void Start () 
	{
		hGameController = (GameController)GameObject.Find("Player").GetComponent(typeof(GameController));
		hPlayerController = (PlayerController)GameObject.Find("Player").GetComponent(typeof(PlayerController));
		hInGameController = GameObject.Find("Player").GetComponent<InGameController>();
		hPowerupController = (PowerupController)GameObject.Find("Player").GetComponent(typeof(PowerupController));
		hMissionsController = GameObject.Find("Player").GetComponent<MissionsController>();
		HUDCamera = GameObject.Find("GUIGroup/Camera").camera;
		
		tUtilityButtons = new Transform[Utilities.GetValues(typeof(Utilities)).Length];
		tUtilityButtons[(int)Utilities.Headstart] = this.transform.Find("UtilityButtonsGroup/Headstart");
		tUtilityButtons[(int)Utilities.MegaHeadstart] = this.transform.Find("UtilityButtonsGroup/MegaHeadstart");
		tUtilityButtons[(int)Utilities.ScoreBooster] = this.transform.Find("UtilityButtonsGroup/ScoreBooster");
		tUtilityButtons[(int)Utilities.MegaScoreBooster] = this.transform.Find("UtilityButtonsGroup/MegaScoreBooster");
		
		tmScore = this.transform.Find("HUDScoreGroup/HUD_Score_Text").GetComponent("TextMesh") as TextMesh;
		tmCurrency = this.transform.Find("HUDCurrencyGroup/HUD_Currency_Text").GetComponent("TextMesh") as TextMesh;		
		tmMultiplier = this.transform.Find("MultiplierGroup/Text_Multiplier").GetComponent("TextMesh") as TextMesh;
		updateMultiplierHUDValue();//update the value of the multiplier on HUD
		
		tHUDScoreContainer = this.transform.Find("HUDScoreGroup/HUD_Score_BG").transform;//HUD Score Container
		tHUDCurrencyContainer = this.transform.Find("HUDCurrencyGroup/HUD_Currency_BG").transform;
		tCurrencyIcon = this.transform.Find("HUDCurrencyGroup/Icon_Coin").transform;
		tHUDMultiplierContainer = this.transform.Find("MultiplierGroup/Background").transform;
		scoreContainerScale = tHUDScoreContainer.localScale;
		currencyIconPosition = tCurrencyIcon.localPosition;
		currencyContainerScale = tHUDCurrencyContainer.localScale;
		multiplierContainerScale = tHUDMultiplierContainer.localScale;
				
		Init();
	}
	
	private void Init()
	{
		iTapState = 0;
		utilityIconState = 0;
		utilityIconPositionY = -22;
		
		currentDeltaTimeScore = 0;
		accumulatedScore = 0;
		missionRecordScore = 0;
		
		iDivisorScore = 10;
		iDivisorCurrency = 10;
		iDivisorMultiplier = 10;
		tHUDScoreContainer.localScale = scoreContainerScale;
		tCurrencyIcon.localPosition = currencyIconPosition;
		tHUDCurrencyContainer.localScale = currencyContainerScale;
		tHUDMultiplierContainer.localScale = multiplierContainerScale;		
				
		//InvokeRepeating("resizeDigitContainer", 1, 1);//momentarily call the resize function
	}
	
	public void Restart()
	{
		CancelInvoke();
		Init();
	}
	
	public void launchGame()
	{
		StartCoroutine(displayUtilityIcons());
	}
	
	void LateUpdate () 
	{
		if (hInGameController.isGamePaused())
			return;
		
		currentDeltaTimeScore = hPlayerController.getDistanceCoveredInDeltaTime() 
			* scoreCoreMultiplier * hGameController.getCurrentScoreMultiplier();	//calculate score
		accumulatedScore += currentDeltaTimeScore;//the score since the game started		
		tmScore.text =  Mathf.RoundToInt(accumulatedScore).ToString();//show the earned score on HUD
		
		//show the earned currency on HUD
		tmCurrency.text = hPowerupController.getCollectedStandardCurrency().ToString();
		
		//send the score earned to Missions Controller
		missionRecordScore += currentDeltaTimeScore;
		if (missionRecordScore >= 1)
		{
			hMissionsController.incrementMissionCount(MissionTypes.Score, 
				Mathf.RoundToInt(missionRecordScore));
			missionRecordScore = 0;
		}
	}//end of late update
	
	private IEnumerator displayUtilityIcons()
	{	
		while (true)
		{
			yield return new WaitForFixedUpdate();
			
			if (hInGameController.isGamePaused())
				continue;
						
			if (utilityIconState == 0)//display owned utilities
			{
				for (int i=0; i<hPowerupController.getUtilityCount(); i++)
				{
					if (hPowerupController.getUtilityData( (Utilities)i ).ownedCount > 0)
					{
						tUtilityButtons[i].localPosition = new Vector3(tUtilityButtons[i].localPosition.x,
							utilityIconPositionY, tUtilityButtons[i].localPosition.z);
						utilityIconPositionY += utilityIconDisplacementY;
					}
				}//end of for
				
				utilityIconDisplayTimeStart = Time.time;
				utilityIconState = 1;
			}
			else if (utilityIconState == 1)//display utility buttons and wait
			{
				if ( (Time.time - utilityIconDisplayTimeStart) >= utilityIconDisplayDuration )
					utilityIconState = 2;
			}
			else if (utilityIconState == 2)//make the utility buttons disappear
			{
				for (int i=0; i<hPowerupController.getUtilityCount(); i++)
				{
					tUtilityButtons[i].localPosition = new Vector3(tUtilityButtons[i].localPosition.x,
						1000, tUtilityButtons[i].localPosition.z);
				}//end of for
				break;
			}//end of state 2
		
			listenerUtilityButtons();//listen for clicks on the utility buttons
		}//end of while
		
		StopCoroutine("diplayUtilityIcons");
	}//end of display utility icons coroutine
		
	private void listenerUtilityButtons()
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
				//call the listner function of the active menu
				if (hit.transform == tUtilityButtons[(int)Utilities.Headstart])
					handlerUtilityButtons(Utilities.Headstart);
				else if (hit.transform == tUtilityButtons[(int)Utilities.MegaHeadstart])
					handlerUtilityButtons(Utilities.MegaHeadstart);
				else if (hit.transform == tUtilityButtons[(int)Utilities.ScoreBooster])
					handlerUtilityButtons(Utilities.ScoreBooster);
				else if (hit.transform == tUtilityButtons[(int)Utilities.MegaScoreBooster])
					handlerUtilityButtons(Utilities.MegaScoreBooster);
			}//end of if raycast
						
			iTapState = 0;		
		}
	}
	
	private void handlerUtilityButtons(Utilities type)
	{
		if (type == Utilities.Headstart)
		{
			StartCoroutine(hPlayerController.headstartRoutine(Utilities.Headstart));
			hPowerupController.updateUtilityOwnedCount(type, -1);
		}
		else if (type == Utilities.MegaHeadstart)
		{
			StartCoroutine(hPlayerController.headstartRoutine(Utilities.MegaHeadstart));
			hPowerupController.updateUtilityOwnedCount(type, -1);
		}
		else if (type == Utilities.ScoreBooster)
		{
			hGameController.updateCurrentScoreMultiplier((int)
				hPowerupController.getUtilityData(type).upgradeValue);
			hPowerupController.updateUtilityOwnedCount(type, -1);
			updateMultiplierHUDValue();
		}
		else if (type == Utilities.MegaScoreBooster)
		{
			hGameController.updateCurrentScoreMultiplier((int)
				hPowerupController.getUtilityData(type).upgradeValue);
			hPowerupController.updateUtilityOwnedCount(type, -1);
			updateMultiplierHUDValue();
		}
		
		utilityIconState = 2;//make the icon disappear
	}
		
	/// <summary>
	/// Resize HUD Score and Currency containers according to digit count.
	/// </summary>
	/*private void resizeDigitContainer()
	{		
		if ( (accumulatedScore / iDivisorScore) >= 1 )
		{			
			tHUDScoreContainer.localScale = new Vector3(tHUDScoreContainer.localScale.x, tHUDScoreContainer.localScale.y,
				tHUDScoreContainer.localScale.z+0.4f);//expand the background of the score count background
			iDivisorScore *= 10;
		}
		
		if ( (hPowerupController.getCollectedStandardCurrency() / iDivisorCurrency) >= 1 )
		{
			tHUDCurrencyContainer.localScale = new Vector3(tHUDCurrencyContainer.localScale.x,
				tHUDCurrencyContainer.localScale.y, tHUDCurrencyContainer.localScale.z+0.4f);//expand the background of the coin count background
			tCurrencyIcon.transform.localPosition = new Vector3(tCurrencyIcon.transform.localPosition.x-6, tCurrencyIcon.transform.localPosition.y,
				tCurrencyIcon.transform.localPosition.z);//displace the coin icon on HUD
			iDivisorCurrency *= 10;
		}
				
		if ( (hGameController.getCurrentScoreMultiplier() / iDivisorMultiplier) >= 1 )
		{			
			tHUDMultiplierContainer.localScale = new Vector3(tHUDMultiplierContainer.localScale.x+0.4f,
				tHUDMultiplierContainer.localScale.y, tHUDMultiplierContainer.localScale.z);//expand the background of the multiplier background
			iDivisorMultiplier *= 10;
		}
	}//end of resize digit container function*/
	
	public void updateMultiplierHUDValue() { tmMultiplier.text = hGameController.getCurrentScoreMultiplier() + "x"; }
}
