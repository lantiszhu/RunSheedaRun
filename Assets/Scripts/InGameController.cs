using UnityEngine;
using System.Collections;

public class InGameController : MonoBehaviour {
	
	#region Constants
	private const float fGameOverSceneDuration = 1;
	#endregion
	
	private int iGameOverState;	
	private float fGameOverSceneStart;
		
	private bool bGamePaused;
	
	private Transform tPauseButton;
	private Camera HUDCamera;
	
	#region Script References
	private MenuScript hMenuScript;
	private GameController hGameController;
	private PlayerController hPlayerController;
	private EnemyController hEnemyController;
	private PrimaryColliderController hPrimaryColliderController;
	private SecondaryColliderController hSecondaryColliderController;
	#endregion
	
	void Start ()
	{
		RenderSettings.fog = true;				//turn on fog on launch
		
		hMenuScript = (MenuScript)GameObject.Find("GUIGroup/MenuGroup").GetComponent(typeof(MenuScript));
		hGameController = (GameController)this.GetComponent(typeof(GameController));		
		hPlayerController = (PlayerController)this.GetComponent(typeof(PlayerController));
		hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
		hPrimaryColliderController = (PrimaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/PrimaryCollider").GetComponent(typeof(PrimaryColliderController));
		hSecondaryColliderController = (SecondaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/SecondaryCollider").GetComponent(typeof(SecondaryColliderController));
		
		tPauseButton = (Transform)GameObject.Find("GUIGroup/HUDGroup/HUDPause").GetComponent(typeof(Transform));
		HUDCamera = (Camera)GameObject.Find("GUIGroup/Camera").camera;
		
		iGameOverState = 0;
		bGamePaused = true;
	}
	
	public void Restart()
	{
		RenderSettings.fog = true;				//turn on fog on launch
		
		//iPauseStatus = 0;
		iGameOverState = 0;
		bGamePaused = true;
	}
		
	void FixedUpdate ()
	{
		//Pause GamePlay
		/*if(iPauseStatus == 1)//pause game
		{	
			hMenuScript.toggleMenuScriptStatus(true);
			//hMenuScript.displayPauseMenu();
			hMenuScript.ShowMenu((int)Menus.PauseMenu);
		
			iPauseStatus = 2;
		}
		else if(iPauseStatus==3)//resume game
		{		
			hMenuScript.toggleMenuScriptStatus(false);
			
			bGamePaused = false;
			iPauseStatus = 0;
		}*/
		
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
	}
	
	public void pauseGame()
	{
		bGamePaused = true;
		hMenuScript.toggleMenuScriptStatus(true);
		hMenuScript.pauseGame();
		
		hPlayerController.togglePlayerAnimation(false);
		hEnemyController.toggleEnemyAnimation(false);
	}
	
	public void resumeGame()
	{
		bGamePaused = false;
		hMenuScript.toggleHUDState(true);
		hMenuScript.toggleMenuScriptStatus(false);
		
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
			yield return new WaitForFixedUpdate();
			
			if (iGameOverState == 0)
			{
				bGamePaused = true;
				hPlayerController.routineGameOver();//signal player controller
				fGameOverSceneStart = Time.time;
				
				//turn off colliders
				hPrimaryColliderController.togglePrimaryCollider(false);
				hPrimaryColliderController.togglePrimaryCollider(false);
				
				hEnemyController.toggleEnemyAnimation(false);//stop enemy animation
				
				iGameOverState = 1;
			}
			else if (iGameOverState == 1)
			{
				if ( (Time.time-fGameOverSceneStart) >= fGameOverSceneDuration)
					iGameOverState = 2;
			}
			else if (iGameOverState == 2)
			{		
				hMenuScript.toggleMenuScriptStatus(true);//enable menu script
				hMenuScript.ShowMenu((int)Menus.GameOverMenu);//show game over menu
				break;
			}
		}//end of while
		
		StopCoroutine("routineGameOver");
	}//end of routine Game Over function
	
	/*
	*	FUNCTION: Check if pause button is tapped in-game
	*	CALLED BY:	Update()
	*/
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
}
