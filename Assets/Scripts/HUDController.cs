using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
	#region Constants
	private const float scoreCoreMultiplier = 0.3f;
	#endregion
	
	private float accumulatedScore;
	
	private TextMesh tmScore;
	private TextMesh tmCurrency;
	private TextMesh tmMultiplier;
	
	private Transform tHUDScoreContainer;
	private Transform tHUDCurrencyContainer;
	private Transform tCurrencyIcon;
	private Transform tHUDMultiplierContainer;
	
	//HUD element Container sizes
	private int iDivisorScore;
	private int iDivisorCurrency;
	private int iDivisorMultiplier;
	
	#region Script References
	private GameController hGameController;
	private PlayerController hPlayerController;
	private PowerupController hPowerupController;
	#endregion
	
	void Start () 
	{
		hGameController = (GameController)GameObject.Find("Player").GetComponent(typeof(GameController));
		hPlayerController = (PlayerController)GameObject.Find("Player").GetComponent(typeof(PlayerController));		
		hPowerupController = (PowerupController)GameObject.Find("Player").GetComponent(typeof(PowerupController));
		
		tmScore = this.transform.Find("HUDScoreGroup/HUD_Score_Text").GetComponent("TextMesh") as TextMesh;
		tmCurrency = this.transform.Find("HUDCurrencyGroup/HUD_Currency_Text").GetComponent("TextMesh") as TextMesh;		
		tmMultiplier = this.transform.Find("MultiplierGroup/Text_Multiplier").GetComponent("TextMesh") as TextMesh;
		tmMultiplier.text = hGameController.getScoreMultiplier() + "x";
		
		tHUDScoreContainer = this.transform.Find("HUDScoreGroup/HUD_Score_BG").transform;//HUD Score Container
		tHUDCurrencyContainer = this.transform.Find("HUDCurrencyGroup/HUD_Currency_BG").transform;
		tCurrencyIcon = this.transform.Find("HUDCurrencyGroup/Icon_Coin").transform;
		tHUDMultiplierContainer = this.transform.Find("MultiplierGroup/Background").transform;
								
		Init();
	}
	
	private void Init()
	{
		accumulatedScore = 0;
		
		iDivisorScore = 10;
		iDivisorCurrency = 10;
		iDivisorMultiplier = 10;
				
		InvokeRepeating("resizeDigitContainer", 1, 1);//momentarily call the resize function
	}
	
	private void Restart()
	{
		Init();
	}
	
	void LateUpdate () 
	{
		accumulatedScore += hPlayerController.getDistanceCoveredInDeltaTime() 
			* scoreCoreMultiplier * hGameController.getScoreMultiplier();	//calculate score
		tmScore.text =  Mathf.RoundToInt(accumulatedScore).ToString();//show the earned score on HUD
		
		//show the earned currency on HUD
		tmCurrency.text = hPowerupController.getCollectedStandardCurrency().ToString();
	}
		
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
				
		if ( (hGameController.getScoreMultiplier() / iDivisorMultiplier) >= 1 )
		{			
			tHUDMultiplierContainer.localScale = new Vector3(tHUDMultiplierContainer.localScale.x+0.4f,
				tHUDMultiplierContainer.localScale.y, tHUDMultiplierContainer.localScale.z);//expand the background of the multiplier background
			iDivisorMultiplier *= 10;
		}
	}//end of resize digit container function
}
