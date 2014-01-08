using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour {
	
	#region Constants
	private const float scoreCoreMultiplier = 0.3f;
	#endregion
	
	private float accumulatedScore;
	
	//HUD element Container sizes
	private int iDivisorScore;
	private int iDivisorCurrency;
	private int iDivisorMultiplier;
	
	private TextMesh tmScore;
	private TextMesh tmCurrency;
	
	private TextMesh tmHUDCurrencyText;
	private TextMesh tmHUDScoreText;
	private Transform tHUDMultiplierContainerMid;	
	private Transform tHUDScoreContainerMid;
	private Transform tHUDCurrencyContainerMid;
	
	#region Script References
	private GameController hGameController;
	private PlayerController hPlayerController;
	#endregion
	GameObject player;
	void Start () 
	{
		player = GameObject.Find("GameObject");
		
		//hGameController = player.GetComponent<GameController>();print(hGameController.transform.name);
		hGameController = (GameController)GameObject.Find("Player").GetComponent(typeof(GameController));
		hPlayerController = (PlayerController)GameObject.Find("Player").GetComponent(typeof(PlayerController));
		
		tmScore = this.transform.Find("HUDScoreGroup/HUD_Score_Text").GetComponent("TextMesh") as TextMesh;
		tmHUDCurrencyText = this.transform.Find("HUDCurrencyGroup/HUD_Currency_Text").GetComponent("TextMesh") as TextMesh;
		tmHUDScoreText = this.transform.Find("HUDScoreGroup/HUD_Score_Text").GetComponent("TextMesh") as TextMesh;
		tHUDScoreContainerMid = this.transform.Find("HUDScoreGroup/HUD_Score_BG").transform;//HUD Score Container	
				
		Init();
	}
	
	private void Init()
	{
		accumulatedScore = 0;
		
		iDivisorScore = 10;
		iDivisorCurrency = 10;
		iDivisorMultiplier = 10;
		
		InvokeRepeating("resizeDigitContainer", 1, 1);
	}
	
	private void Restart()
	{
		Init();
	}
	
	void LateUpdate () 
	{
		/*accumulatedScore += hPlayerController.getDistanceCoveredInDeltaTime() 
			* scoreCoreMultiplier * hGameController.getScoreMultiplier();
		tmScore.text =  Mathf.RoundToInt(accumulatedScore).ToString();*/
	}
	
	//FUNCTION: Resize HUD Score and Currency containers according to digit count
	private void resizeDigitContainer()
	{		
		//int fCurrency = hPowerupsMainController.getCurrencyUnits();
			
		if ( (accumulatedScore / iDivisorScore) >= 1 )
		{			
			tHUDScoreContainerMid.localScale = new Vector3(tHUDScoreContainerMid.localScale.x, tHUDScoreContainerMid.localScale.y, tHUDScoreContainerMid.localScale.z+0.4f);
			iDivisorScore *= 10;
		}
		
		/*if ( (fCurrency / iDivisorCurrency) >= 1 )
		{
			tHUDCurrencyContainerMid.localScale = new Vector3(tHUDCurrencyContainerMid.localScale.x, tHUDCurrencyContainerMid.localScale.y, tHUDCurrencyContainerMid.localScale.z+0.4f);
			tHUDCurrencyIcon.transform.localPosition = new Vector3(tHUDCurrencyIcon.transform.localPosition.x-6, tHUDCurrencyIcon.transform.localPosition.y, tHUDCurrencyIcon.transform.localPosition.z);
			iDivisorCurrency *= 10;
		}
		
		int iMultiplier = hGameController.getMultiplierValue();
		if ( (iMultiplier / iDivisorMultiplier) >= 1 )
		{			
			tHUDMultiplierContainerMid.localScale = new Vector3(tHUDMultiplierContainerMid.localScale.x, tHUDMultiplierContainerMid.localScale.y, tHUDMultiplierContainerMid.localScale.z+0.4f);
			iDivisorMultiplier *= 10;
		}*/
	}
}
