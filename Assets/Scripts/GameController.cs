using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	private int userCurrency;
	
	#region Script References
	private MenuScript hMenuScript;
	private PatchController hPatchController;
	private InGameController hInGameController;
	private PlayerController hPlayerController;
	private EnemyController hEnemyController;
	private PrimaryColliderController hPrimaryColliderController;
	private SecondaryColliderController hSecondaryColliderController;
	#endregion
	
	void Start () 
	{
		Application.targetFrameRate = 60;		//ceiling the frame rate on 60 (debug only)
		//PlayerPrefs.DeleteAll();
		
		hMenuScript = (MenuScript)GameObject.Find("GUIGroup/MenuGroup").GetComponent(typeof(MenuScript));
		hPatchController = (PatchController)this.GetComponent(typeof(PatchController));
		hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
		hInGameController = (InGameController)this.GetComponent(typeof(InGameController));
		hPlayerController = (PlayerController)this.GetComponent(typeof(PlayerController));
		hPrimaryColliderController = (PrimaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/PrimaryCollider").GetComponent(typeof(PrimaryColliderController));
		hSecondaryColliderController = (SecondaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/SecondaryCollider").GetComponent(typeof(SecondaryColliderController));
	}
		
	public void relaunchGame()
	{	
		StopAllCoroutines();
		
		hPatchController.Restart();
		hInGameController.Restart();
		hPlayerController.Restart();
		hEnemyController.Restart();
		hMenuScript.Restart();
		hPrimaryColliderController.Restart();
		
		System.GC.Collect();
	}
	
	public int getUserCurrency()
	{
		return userCurrency;
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