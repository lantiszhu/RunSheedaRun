using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	#region Variables
	private int userStandardCurrency;
	private int userPremiumCurrency;
	private int defaultScoreMultiplier;//user's score multiplier
	private int currentScoreMultiplier;//current multiplier
	#endregion
	
	#region Utilities
	/*private readonly int[] utilityPrice = {
		100,	//Headstart
		200,	//Mega Headstart
		100,	//Score Booster
		200		//Mega Score Booster
	};
	private readonly float[] utilityDuration = {
		10,
		15,
		0,
		0
	};*/
	#endregion
	
	#region Script References
	private MenuScript hMenuScript;
	private HUDController hHUDController;
	private PatchController hPatchController;
	private InGameController hInGameController;
	private PlayerController hPlayerController;
	private EnemyController hEnemyController;
	private PowerupController hPowerupController;
	private CameraController hCameraController;	
	private PrimaryColliderController hPrimaryColliderController;
	private SecondaryColliderController hSecondaryColliderController;
	#endregion
	
	void Start () 
	{
		Application.targetFrameRate = 60;		//ceiling the frame rate on 60 (debug only)
		//PlayerPrefs.DeleteAll();
		
		hMenuScript = (MenuScript)GameObject.Find("GUIGroup/MenuGroup").GetComponent(typeof(MenuScript));
		hHUDController = GameObject.Find("GUIGroup/HUDGroup").GetComponent<HUDController>();
		hPatchController = (PatchController)this.GetComponent(typeof(PatchController));
		hPowerupController = this.GetComponent<PowerupController>();
		hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
		hInGameController = (InGameController)this.GetComponent(typeof(InGameController));
		hPlayerController = (PlayerController)this.GetComponent(typeof(PlayerController));
		hCameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
		hPrimaryColliderController = (PrimaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/PrimaryCollider").GetComponent(typeof(PrimaryColliderController));
		hSecondaryColliderController = (SecondaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/SecondaryCollider").GetComponent(typeof(SecondaryColliderController));
				
		getUserData();
		Init();
	}
	
	void getUserData()
	{
		if (PlayerPrefs.HasKey("UserStandardCurrency"))
			userStandardCurrency = PlayerPrefs.GetInt("UserStandardCurrency");
		else
		{
			PlayerPrefs.SetInt("UserStandardCurrency", 0);
			userStandardCurrency = 0;
		}
		
		if (PlayerPrefs.HasKey("UserPremiumCurrency"))
			userPremiumCurrency = PlayerPrefs.GetInt("UserPremiumCurrency");
		else
		{
			PlayerPrefs.SetInt("UserPremiumCurrency", 0);
			userPremiumCurrency = 0;
		}
		
		if (PlayerPrefs.HasKey("ScoreMultiplier"))
			defaultScoreMultiplier = PlayerPrefs.GetInt("ScoreMultiplier");
		else
		{
			defaultScoreMultiplier = 1;
			PlayerPrefs.SetInt("ScoreMultiplier", defaultScoreMultiplier);
		}
		
		PlayerPrefs.Save();
	}//end of get user data function
	
	void Init()
	{
		currentScoreMultiplier = defaultScoreMultiplier;
	}
	
	public void relaunchGame()
	{	
		PlayerPrefs.Save();
		StopAllCoroutines();
		
		hPatchController.Restart();
		hInGameController.Restart();
		hPlayerController.Restart();
		hEnemyController.Restart();
		hMenuScript.Restart();
		hCameraController.Restart();
		hHUDController.Restart();
		hPowerupController.Restart();
		hPrimaryColliderController.Restart();
		hSecondaryColliderController.Restart();
		
		System.GC.Collect();
	}
	
	public int getUserStandardCurrency() { return userStandardCurrency; }
	public int getUserPremiumCurrency() { return userPremiumCurrency; }
	public int getCurrentScoreMultiplier() { return currentScoreMultiplier; }
	
	/// <summary>
	/// Updates the default score multiplier. This is not the multiplier currently
	/// active.
	/// </summary>
	public void updateDefaultScoreMultiplier()
	{
		defaultScoreMultiplier += 1;
		currentScoreMultiplier += 1;
		PlayerPrefs.SetInt("ScoreMultiplier", defaultScoreMultiplier);		
	}
	
	/// <summary>
	/// Updates the current score multiplier in case of multiplier power-up or utility collection/ usage.
	/// </summary>
	/// <param name='val'>
	/// Value.
	/// </param>
	public void updateCurrentScoreMultiplier(int val)
	{
		currentScoreMultiplier += val;
	}
	
	public bool updateUserStandardCurrency(int updateAmount)
	{
		//on deduction check if user has enough currency
		if (updateAmount < 0 
			&& userStandardCurrency < Mathf.Abs(updateAmount) )
			return false;
		
		userStandardCurrency += updateAmount;
		PlayerPrefs.SetInt("UserStandardCurrency",userStandardCurrency);
		return true;
	}
	
	public bool updateUserPremiumCurrency(int updateAmount)
	{
		//on deduction check if user has enough currency
		if (updateAmount < 0
			&& userPremiumCurrency < Mathf.Abs(updateAmount))
			return false;
		
		userPremiumCurrency += updateAmount;
		PlayerPrefs.SetInt("UserPremiumCurrency", userPremiumCurrency);
		return true;
	}
	
	#region Utility Functions
	/*public int getUtilityPrice(Utilities type) { return utilityPrice[(int)type]; }
	public float getUtilityDuration(Utilities type) { return utilityDuration[(int)type]; }
		
	public void updateUtilityOwned(Utilities type)
	{
		utilityOwned[(int)type] ++;
		PlayerPrefs.SetInt("Utility_"+(int)type, utilityOwned[(int)type]);		
	}
	
	public int getUtilityOwnedCount(Utilities type) { return utilityOwned[(int)type]; }*/
	#endregion
}