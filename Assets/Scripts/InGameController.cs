using UnityEngine;
using System.Collections;

public class InGameController : MonoBehaviour {
	
	#region Constants
	private const float fGameOverSceneDuration = 1;
	private const float fReviveCountdownDuration = 3;
	private const float fReviveInvincibleDuration = 2;
	#endregion
	
	public int iGameOverState;
	private float fGameOverSceneStart;
	private float fReviveCountdownStart;
	private float fReviveInvincibleStart;
	
	private bool bGamePaused;
	
	private Transform tPauseButton;
	private Camera HUDCamera;
	
	#region Script References
	private MenuScript hMenuScript;
	private GameController hGameController;
	private PlayerController hPlayerController;
	private EnemyController hEnemyController;
	private MissionsController hMissionsController;
	private SoundController hSoundController;
	private PowerupController hPowerupController;
	private HUDController hHUDController;
	private PrimaryColliderController hPrimaryColliderController;
	private SecondaryColliderController hSecondaryColliderController;
	#endregion
	
	void Start ()
	{		
		hMenuScript = (MenuScript)GameObject.Find("GUIGroup/MenuGroup").GetComponent(typeof(MenuScript));
		hGameController = (GameController)this.GetComponent(typeof(GameController));
		hPlayerController = (PlayerController)this.GetComponent(typeof(PlayerController));
		hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
		hMissionsController = this.GetComponent<MissionsController>();
		hSoundController = GameObject.Find("SoundManager").GetComponent<SoundController>();
		hPowerupController = this.GetComponent<PowerupController>();
		hHUDController = GameObject.Find("GUIGroup/HUDGroup").GetComponent<HUDController>();
		hPrimaryColliderController = (PrimaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/PrimaryCollider").GetComponent(typeof(PrimaryColliderController));
		hSecondaryColliderController = (SecondaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/SecondaryCollider").GetComponent(typeof(SecondaryColliderController));
		
		tPauseButton = (Transform)GameObject.Find("GUIGroup/HUDGroup/HUDPause").GetComponent(typeof(Transform));
		HUDCamera = (Camera)GameObject.Find("GUIGroup/Camera").camera;
		
		Init();
	}
	
	void Init()
	{
		RenderSettings.fog = true;				//turn on fog on launch
		
		fReviveCountdownStart = 0;
		iGameOverState = 0;
		bGamePaused = true;
	}
	
	public void Restart()
	{
		StopAllCoroutines();
		Init();
	}
		
	void FixedUpdate ()
	{			
		getClicks();//check if pause button is clicked
	}//end of fixed update
	
	/*
	*	FUNCTION: start the gameplay and display all related elements
	*	CALLED BY: MenuScript.MainMenuGui()
	*			   MenuScript.MissionsGui()
	*/
	public void launchGame()
	{			
		bGamePaused = false;//tell all scripts to resume
		
		hMenuScript.toggleHUDState(true);
		hMenuScript.toggleMenuScriptStatus(false);
		hPlayerController.launchGame();//tell the PlayerController to start game
		hEnemyController.launchGame();//tell the EnemyController to start following the player
		hHUDController.launchGame();
		
		//tell the Missions Controller that a new game has been started
		hMissionsController.incrementMissionCount(MissionTypes.StartGame);
	}
	
	public void pauseGame()
	{
		bGamePaused = true;
		hMenuScript.toggleMenuScriptStatus(true);
		hMenuScript.pauseGame();
		
		hPlayerController.togglePlayerAnimation(false);
		hEnemyController.toggleEnemyAnimation(false);
		
		hSoundController.pausePlayerSound(PlayerSounds.Run);
		
		System.GC.Collect();//clear unused memory
		PlayerPrefs.Save();//save any unsaved data
	}
	
	public void resumeGame()
	{
		bGamePaused = false;
		hMenuScript.toggleHUDState(true);
		hMenuScript.toggleMenuScriptStatus(false);
		
		hSoundController.playPlayerSound(PlayerSounds.Run);
		
		hPlayerController.togglePlayerAnimation(true);
		hEnemyController.toggleEnemyAnimation(true);
	}
	
	public void handleStumble()
	{
		hPlayerController.handleStumble();
		hEnemyController.handleStumble();
		
		hPrimaryColliderController.togglePrimaryCollider(true);
		hSecondaryColliderController.toggleSecondaryCollider(true);
	}
	
	/// <summary>
	/// Routines the game over.
	/// </summary>
	/// <returns>
	/// The game over.
	/// </returns>
	public IEnumerator routineGameOver()
	{
		while(true)
		{
			if (iGameOverState == 0)//display and countdown revive menu
			{
				iGameOverState = 1;
				bGamePaused = true;							
				//turn off colliders
				hPrimaryColliderController.togglePrimaryCollider(false);
				hPrimaryColliderController.togglePrimaryCollider(false);
				
				hEnemyController.toggleEnemyAnimation(false);//stop enemy animation
				hPlayerController.toggleControlsState(false);
								
				hMenuScript.toggleMenuScriptStatus(true);//enable menu script
				hMenuScript.ShowMenu((int)Menus.Revive);
				
				hSoundController.pausePlayerSound(PlayerSounds.Run);
				fReviveCountdownStart = Time.time;				
				
				PlayerPrefs.Save();				
			}
			else if (iGameOverState == 1)//wait for user to revive
			{
				if ( (Time.time-fReviveCountdownStart) 
					>= fReviveCountdownDuration )//if user didnt revive
				{
					hMenuScript.CloseMenu((int)Menus.Revive);
					hPlayerController.routineGameOver();//signal player controller
					fGameOverSceneStart = Time.time;
					iGameOverState = 2;
				}				
			}
			else if (iGameOverState == 2)//wait for the death scene
			{
				if ( (Time.time-fGameOverSceneStart) >= fGameOverSceneDuration)
					iGameOverState = 3;
			}
			else if (iGameOverState == 3)//display the game over menu
			{	
				hGameController.updateUserStandardCurrency(hPowerupController.getCollectedStandardCurrency());
				hGameController.updateUserPremiumCurrency(hPowerupController.getCollectedPremiumCurrency());
				PlayerPrefs.Save();
				
				hMenuScript.ShowMenu((int)Menus.GameOverMenu);//show game over menu
				break;
			}
			
			else if (iGameOverState == 51)//revive button pressed
			{				
				hMenuScript.toggleMenuScriptStatus(false);//disable menu script
				bGamePaused = false;
				fReviveInvincibleStart = Time.time;
				
				hSoundController.playPlayerSound(PlayerSounds.Run);
				
				hPlayerController.revivePlayer();
				hEnemyController.revivePlayer();
				
				iGameOverState = 52;
			}
			else if (iGameOverState == 52)
			{
				if ( (Time.time - fReviveInvincibleStart) >= fReviveInvincibleDuration)
				{
					hPrimaryColliderController.togglePrimaryCollider(true);
					hPrimaryColliderController.togglePrimaryCollider(true);
					
					break;
				}
			}
			
			yield return new WaitForFixedUpdate();
		}//end of while
		
		iGameOverState = 0;
		StopCoroutine("routineGameOver");
	}//end of routine Game Over function
	
	/// <summary>
	/// Check if pause button is tapped during the run.
	/// </summary>
	private void getClicks()
	{
		if(Input.GetMouseButtonUp(0))
		{
			Vector3 screenPoint;
			Vector2 buttonSize;
			Rect Orb_Rect;
			
			if (hMenuScript.isHUDEnabled())
			{
				buttonSize = new Vector2(Screen.width/6,Screen.width/6);
				screenPoint = HUDCamera.WorldToScreenPoint( tPauseButton.position );
				
				Orb_Rect = new Rect (screenPoint.x - ( buttonSize.x * 0.5f ), screenPoint.y - ( buttonSize.y * 0.5f ), buttonSize.x, buttonSize.y);
				if(Orb_Rect.Contains(Input.mousePosition))
				{				
					pauseGame();
				}
			}
		}//end of mouserelease == true if			
	}//end of get clicks function
	
	public void processClicksReviveMenu(ReviveMenuEvents action)
	{
		if (action == ReviveMenuEvents.Revive)
		{
			iGameOverState = 51;
		}
	}
	
	/*
	*	FUNCTION: Execute a function based on button press in Pause Menu
	*	CALLED BY: MenuScript.PauseMenu()
	*/
	public void processClicksPauseMenu(PauseMenuEvents index)
	{
		if (index == PauseMenuEvents.MainMenu)			
		{
			hGameController.relaunchGame();
			hMenuScript.toggleMenuScriptStatus(true);
			hMenuScript.ShowMenu((int)Menus.MainMenu);
		}
		else if (index == PauseMenuEvents.Resume)		
			resumeGame();		
	}//end of process click pause menu
	
	/*
	*	FUNCTION: Execute a function based on button press in Death Menu
	*	CALLED BY: MenuScript.GameOverMenu()
	*/
	public void procesClicksDeathMenu(GameOverMenuEvents index)
	{
		if (index == GameOverMenuEvents.Play)
		{			
			hGameController.relaunchGame();
			launchGame();
		}
		else if (index == GameOverMenuEvents.Back)
		{			
			hGameController.relaunchGame();
			hMenuScript.toggleMenuScriptStatus(true);
			hMenuScript.ShowMenu((int)Menus.MainMenu);
		}
	}//end of DM_ProcessClicks
	
	//paused state
	public bool isGamePaused() { return bGamePaused; }
	public bool isGameOverRoutineActive() 
	{ 
		if (iGameOverState > 0)
			return true;
		else
			return false;
	}
}
