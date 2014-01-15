using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	private int userCurrency;
	private int defaultScoreMultiplier;//user's score multiplier
	private int currentScoreMultiplier;//current multiplier
	
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
		if (PlayerPrefs.HasKey("UserCurrency"))
			userCurrency = PlayerPrefs.GetInt("UserCurrency");
		else
		{
			PlayerPrefs.SetInt("UserCurrency", 0);
			userCurrency = 0;
		}
		
		if (PlayerPrefs.HasKey("ScoreMultiplier"))
			defaultScoreMultiplier = PlayerPrefs.GetInt("ScoreMultiplier");
		else
		{
			defaultScoreMultiplier = 1;
			PlayerPrefs.SetInt("ScoreMultiplier", defaultScoreMultiplier);
		}
		
		PlayerPrefs.Save();
	}
	
	void Init()
	{
		currentScoreMultiplier = defaultScoreMultiplier;
	}
	
	public void relaunchGame()
	{	
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
	
	public int getUserCurrency() { return userCurrency; }
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
	
	public bool updateUserCurrency(int updateAmount)
	{
		if (updateAmount < 0 
			&& userCurrency < Mathf.Abs(updateAmount) )
			return false;
		
		userCurrency += updateAmount;
		PlayerPrefs.SetInt("UserCurrency",userCurrency);
		return true;
	}
}