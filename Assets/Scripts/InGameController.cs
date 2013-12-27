using UnityEngine;
using System.Collections;

public class InGameController : MonoBehaviour {
	
	private int iPauseStatus;
	
	private int iGameOverState;	
	private const float fGameOverSceneDuration = 1;
	private float fGameOverSceneStart;
		
	private bool bGamePaused;
	
	//script references
	private MenuScript hMenuScript;
	private GameController hGameController;
	private PlayerController hPlayerController;	
	
	void Start ()
	{
		RenderSettings.fog = true;				//turn on fog on launch
		
		hMenuScript = (MenuScript)GameObject.Find("GUIGroup/MenuGroup").GetComponent(typeof(MenuScript));
		hGameController = (GameController)this.GetComponent(typeof(GameController));
		hPlayerController = (PlayerController)this.GetComponent(typeof(PlayerController));
		
		iPauseStatus = 0;
		iGameOverState = 0;
		bGamePaused = true;
	}
	
	public void Restart()
	{
		RenderSettings.fog = true;				//turn on fog on launch
		
		iPauseStatus = 0;
		iGameOverState = 0;
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
	}//end of fixed update
	
	/*
	*	FUNCTION: start the gameplay and display all related elements
	*	CALLED BY: MenuScript.MainMenuGui()
	*			   MenuScript.MissionsGui()
	*/
	public void launchGame()
	{			
		bGamePaused = false;//tell all scripts to resume
		
		hMenuScript.toggleMenuScriptStatus(false);
		hPlayerController.launchGame();//tell the PlayerController to start game
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
				hPlayerController.routineGameOver();
				fGameOverSceneStart = Time.time;
				
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
	*	FUNCTION: Execute a function based on button press in Pause Menu
	*	CALLED BY: MenuScript.PauseMenu()
	*/
	public void processClicksPauseMenu(PauseMenuEvents index)
	{
		if (index == PauseMenuEvents.MainMenu)
			//hGameController.relaunchGame();
			Application.LoadLevel("Game");
		else if (index == PauseMenuEvents.Resume)
		{							
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
		{
			Application.LoadLevel("Game");
			/*hGameController.relaunchGame();
			launchGame();*/
		}
		else if (index == GameOverMenuEvents.Back)
		{
			Application.LoadLevel("Game");
			//hGameController.relaunchGame();
			/*hMenuScript.toggleMenuScriptStatus(true);
			hMenuScript.ShowMenu((int)Menus.MainMenu);*/
		}
	}//end of DM_ProcessClicks
	
	//paused state
	public bool isGamePaused() { return bGamePaused; }
}
