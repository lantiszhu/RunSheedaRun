using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
	#region Constants
	private const float scoreCoreMultiplier = 0.3f;
	#endregion
	
	private int utilityIconState;
	private float currentDeltaTimeScore;//score earned in delta time
	private float accumulatedScore;//total score in the current run
	private float missionRecordScore;//used for counting score for missions
		
	private TextMesh tmScore;
	private TextMesh tmCurrency;
	private TextMesh tmMultiplier;
	
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
		
		tmScore = this.transform.Find("HUDScoreGroup/HUD_Score_Text").GetComponent("TextMesh") as TextMesh;
		tmCurrency = this.transform.Find("HUDCurrencyGroup/HUD_Currency_Text").GetComponent("TextMesh") as TextMesh;		
		tmMultiplier = this.transform.Find("MultiplierGroup/Text_Multiplier").GetComponent("TextMesh") as TextMesh;
		tmMultiplier.text = hGameController.getCurrentScoreMultiplier() + "x";
		
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
		utilityIconState = 0;
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
		
		CancelInvoke();
		InvokeRepeating("resizeDigitContainer", 1, 1);//momentarily call the resize function
	}
	
	public void Restart()
	{
		Init();
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
	
	private IEnumerator diplayUtilityIcons()
	{
		yield return new WaitForFixedUpdate();
		
		if (utilityIconState == 0)
		{
			
		}
	}//end of display utility icons coroutine
		
	/// <summary>
	/// Resize HUD Score and Currency containers according to digit count.
	/// </summary>
	private void resizeDigitContainer()
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
	}//end of resize digit container function
}
