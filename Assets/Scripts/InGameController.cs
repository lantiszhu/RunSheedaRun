using UnityEngine;
using System.Collections;

public class InGameController : MonoBehaviour {
	
	private int iPauseStatus;
	private int iGameOverStatus;
	
	private bool bGamePaused;
	
	//script references
	private MenuScript hMenuScript;
	private PlayerController hPlayerController;
	
	void Start ()
	{
		RenderSettings.fog = true;				//turn on fog on launch
		
		hMenuScript = (MenuScript)GameObject.Find("GUIGroup/MenuGroup").GetComponent(typeof(MenuScript));
		hPlayerController = (PlayerController)this.GetComponent(typeof(PlayerController));
		
		iPauseStatus = 0;
		bGamePaused = true;
	}
		
	void FixedUpdate ()
	{
		//Pause GamePlay
		if(iPauseStatus == 1)//pause game
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
		}
		
		if (iGameOverStatus == 1)
		{
			
		}
	}//end of fixed update
	
	/*
	*	FUNCTION: start the gameplay and display all related elements
	*	CALLED BY: MenuScript.MainMenuGui()
	*			   MenuScript.MissionsGui()
	*/
	public void launchGame()
	{			
		bGamePaused = false;//tell all scripts to resume
					
		hPlayerController.launchGame();//tell the PlayerController to start game		
	}
	
	private void routineGameOver()
	{
		hPlayerController.routineGameOver();
		bGamePaused = true;
		
		hMenuScript.toggleMenuScriptStatus(true);//enable menu script
		hMenuScript.ShowMenu((int)Menus.GameOverMenu);//show game over menu
	}
	
	/*
	*	FUNCTION: Execute a function based on button press in Pause Menu
	*	CALLED BY: MenuScript.PauseMenu()
	*/
	public void processClicksPauseMenu(PauseMenuEvents index)
	{
		if (index == PauseMenuEvents.MainMenu)
			Application.LoadLevel("Game");
		else if (index == PauseMenuEvents.Resume)
		{	
			//hNGUIMenuScript.toggleHUDGroupState(true);
			
			iPauseStatus = 3;
						
			hPlayerController.togglePlayerAnimation(true);//pause legacy animations
		}
	}//end of process click pause menu
	
	/*
	*	FUNCTION: Execute a function based on button press in Death Menu
	*	CALLED BY: MenuScript.GameOverMenu()
	*/
	public void procesClicksDeathMenu(GameOverMenuEvents index)
	{
		if (index == GameOverMenuEvents.Play)
			Application.LoadLevel("Game");
		else if (index == GameOverMenuEvents.Back)	
			Application.LoadLevel("Game");
	}//end of DM_ProcessClicks
	
	//paused state
	public bool isGamePaused() { return bGamePaused; }
}
