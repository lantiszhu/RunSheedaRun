/*
*	FUNCTION:
*	- This script handles the menu instantiation, destruction and button event handling.
*	- Each menu item is always present in the scene. The inactive menus are hidden from the 
*	HUD Camera by setting their y position to 1000 units.
*	- To show a menu, the menu prefab's y position is set to 0 units and it appears in front of
*	the HUD Camera.
*
*	USED BY: This script is part of the MenuGroup prefab.
*
*	INFO:
*	To add a new menu, do the following in Editor:
*	- Add its name in the Menus enum.
*	- Set its transform in tMenuTransforms array.
*	- Set its buttons' transforms in an array.
*	- Create its event handler. (E.g. handlerMainMenu(), handlerPauseMenu())
*	- Write implementation of each of the buttons in the handler function.
*	- Add its case in the listenerClicks() function.
*	- Add its case in the ShowMenu(...) function.
*	- Add its case in the CloseMenu(...) function.
*
*	ADDITIONAL INFO:
*	Unity's default GUI components are not used in the UI implementation to reduce 
*	performace overhead.
*
*/

using UnityEngine;
using System.Collections;

//the available menus
public enum Menus
{
	MainMenu = 0, 
	PauseMenu = 1,
	GameOverMenu = 2,
	InstructionsMenu = 3,
	SettingsMenu = 4,
	Shop = 5,
	MissionsMenu = 6,
	AchievementsMenu = 7,
	Revive = 8
}

//events/ buttons on the pause menu
public enum PauseMenuEvents
{
	Resume = 0,
	MainMenu = 1
}

//events/ buttons on the game over menu
public enum GameOverMenuEvents
{
	Back = 0,
	Play = 1
}

public enum ReviveMenuEvents
{
	Revive = 0
}

public class MenuScript : MonoBehaviour {
	
	private Transform tMenuGroup;//get the menu group parent
	private int CurrentMenu = -1;	//current menu index
	//private int PreviousMenu = -1;
	private bool bHUDState;
	private int iTapState = 0;//state of tap on screen
	private int iAndroidBackTapState = 0;//state of tap on back button on android devices
	
	private float aspectRatio = 0.0f;
	private float fResolutionFactor;	//displacement of menu elements based on device aspect ratio
	public float getResolutionFactor() { return fResolutionFactor; }
	
	private Camera HUDCamera;//the menu camera
	
	//script references
	/*private ControllerScriptCS hControllerScriptCS;
	private SoundManagerCS hSoundManagerScriptCS;
	private InGameControllerCS hInGameControllerCS;
	private MissionsControllerCS hMissionsControllerCS;*/
	private InGameController hInGameController;
	
	private Transform[] tMenuTransforms;	//menu prefabs
	//Main Menu
	private Transform[] tMainMenuButtons;	//main menu buttons' transform
	private int iMainMenuButtonsCount = 6;	//total number of buttons on main menu
	//Pause Menu
	private Transform[] tPauseButtons;	//pause menu buttons transforms
	private int iPauseButtonsCount = 2;	//total number of buttons on pause menu
	//private TextMesh tmPauseMenuMissionList;//mission description on the pause menu
	//Game Over Menu
	private Transform[] tGameOverButtons;
	private int iGameOverButtonsCount = 2;
	//revive menu
	private Transform[] tReviveButtons;
	private const int iReviveCuttonsCount = 1;
	//instructions menu
	/*private Transform[] tInstructionsButtons;
	private int iInstructionsButtonsCount = 1;
	//settings menu
	private Transform[] tSettingsButtons;
	private int iSettingsButtonsCount = 7;
	//missions menu
	private Transform[] tMissionsMenuButtons;
	private int iMissionsMenuButtonsCount=1;
	private TextMesh tmMissionsMenuMissionList;
	//achievements menu
	private Transform[] tAchievementMenuButtons;
	private int iAchievementMenuButtonsCount=1;
	private TextMesh tmAchievementsMenuDescription;
	//the Ad
	private Transform[] tAdButtons;
	private int iAdButtonsCount = 2;*/
	//loading splash screen
	private Transform tLoadScreen;
	
	/*//meshrenderers of all the radio buttons in settings menu
	private MeshRenderer mrSwipeControls;
	private MeshRenderer mrGyroControls;
	private MeshRenderer mrMusicON;
	private MeshRenderer mrMusicOFF;
	private MeshRenderer mrSoundON;
	private MeshRenderer mrSoundOFF;
	
	//shop menu
	private ShopScriptCS hShopScriptCS;*/
	
	private Transform tHUDGroup;
		
	//resume game countdown
	private int iResumeGameState = 0;
	private float iResumeGameStartTime = 0;
	private TextMesh tmPauseCountdown;	//count down numbers after resume
		
	void Start ()
	{
		HUDCamera = (Camera)GameObject.Find("GUIGroup/Camera").GetComponent(typeof(Camera));
		/*hControllerScriptCS = (ControllerScriptCS)GameObject.Find("Player").GetComponent(typeof(ControllerScriptCS));
		hSoundManagerScriptCS = (SoundManagerCS)GameObject.Find("SoundManager").GetComponent(typeof(SoundManagerCS));
		hInGameControllerCS = (InGameControllerCS)GameObject.Find("Player").GetComponent(typeof(InGameControllerCS));
		hMissionsControllerCS = (MissionsControllerCS)GameObject.Find("Player").GetComponent(typeof(MissionsControllerCS));*/
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
				
		//the fResolutionFactor can be used to adjust components according to screen size
		aspectRatio = ( (Screen.height * 1.0f)/(Screen.width * 1.0f) - 1.77f);
		fResolutionFactor = (43.0f * (aspectRatio));
		
		tMenuGroup = GameObject.Find("MenuGroup").transform;
		tMenuTransforms = new Transform[(int)Menus.GetValues(typeof(Menus)).Length];
		
		//main menu initialization
		tMenuTransforms[(int)Menus.MainMenu] = (Transform)tMenuGroup.Find("MainMenu").GetComponent(typeof(Transform));
		tMainMenuButtons = new Transform[iMainMenuButtonsCount];
		tMainMenuButtons[0] = tMenuTransforms[(int)Menus.MainMenu].Find("Buttons/Button_TapToPlay");
		tMainMenuButtons[1] = tMenuTransforms[(int)Menus.MainMenu].Find("Buttons/Button_Instructions");
		tMainMenuButtons[2] = tMenuTransforms[(int)Menus.MainMenu].Find("Buttons/Button_Settings");
		tMainMenuButtons[3] = tMenuTransforms[(int)Menus.MainMenu].Find("Buttons/Button_Shop");
		tMainMenuButtons[4] = tMenuTransforms[(int)Menus.MainMenu].Find("Buttons/Button_Missions");
		tMainMenuButtons[5] = tMenuTransforms[(int)Menus.MainMenu].Find("Buttons/Button_Achievements");
				
		//pause menu initialization
		tMenuTransforms[(int)Menus.PauseMenu] = (Transform)tMenuGroup.Find("PauseMenu").GetComponent(typeof(Transform));
		tPauseButtons = new Transform[iPauseButtonsCount];
		tPauseButtons[0] = tMenuTransforms[(int)Menus.PauseMenu].Find("Buttons/Button_Back");
		tPauseButtons[1] = tMenuTransforms[(int)Menus.PauseMenu].Find("Buttons/Button_Resume");
		//tmPauseMenuMissionList = (TextMesh)tMenuTransforms[(int)Menus.PauseMenu].Find("Text_MissionDescription").GetComponent(typeof(TextMesh));
		
		//game over menu initialization
		tMenuTransforms[(int)Menus.GameOverMenu] = (Transform)tMenuGroup.Find("GameOver").GetComponent(typeof(Transform));
		tGameOverButtons = new Transform[iGameOverButtonsCount];
		tGameOverButtons[0] = tMenuTransforms[(int)Menus.GameOverMenu].Find("Buttons/Button_Back");
		tGameOverButtons[1] = tMenuTransforms[(int)Menus.GameOverMenu].Find("Buttons/Button_Play");
		
		//revive menu initialization
		tMenuTransforms[(int)Menus.Revive] = tMenuGroup.Find("Revive").transform;
		tReviveButtons = new Transform[iReviveCuttonsCount];
		tReviveButtons[0] = tMenuTransforms[(int)Menus.Revive].Find("Buttons/Revive").transform;
		
		/*//instructions menu initialization
		tMenuTransforms[(int)Menus.InstructionsMenu] = (Transform)tMenuGroup.Find("Instructions").GetComponent(typeof(Transform));
		tInstructionsButtons = new Transform[iInstructionsButtonsCount];
		tInstructionsButtons[0] = (Transform)tMenuTransforms[(int)Menus.InstructionsMenu].Find("Buttons/Button_Back").GetComponent(typeof(Transform));
		
		//settings menu initialization
		tMenuTransforms[(int)Menus.SettingsMenu] = (Transform)tMenuGroup.Find("Settings").GetComponent(typeof(Transform));
		tSettingsButtons = new Transform[iSettingsButtonsCount];
		tSettingsButtons[0] = tMenuTransforms[(int)Menus.SettingsMenu].Find("Buttons/Button_Back");
		tSettingsButtons[1] = (Transform)tMenuTransforms[(int)Menus.SettingsMenu].Find("ControlType/Button_Swipe/RadioButton_Background").GetComponent(typeof(Transform));
		tSettingsButtons[2] = (Transform)tMenuTransforms[(int)Menus.SettingsMenu].Find("ControlType/Button_Gyro/RadioButton_Background").GetComponent(typeof(Transform));
		tSettingsButtons[3] = (Transform)tMenuTransforms[(int)Menus.SettingsMenu].Find("Music/Button_ON/RadioButton_Background").GetComponent(typeof(Transform));
		tSettingsButtons[4] = (Transform)tMenuTransforms[(int)Menus.SettingsMenu].Find("Music/Button_OFF/RadioButton_Background").GetComponent(typeof(Transform));
		tSettingsButtons[5] = (Transform)tMenuTransforms[(int)Menus.SettingsMenu].Find("Sound/Button_ON/RadioButton_Background").GetComponent(typeof(Transform));
		tSettingsButtons[6] = (Transform)tMenuTransforms[(int)Menus.SettingsMenu].Find("Sound/Button_OFF/RadioButton_Background").GetComponent(typeof(Transform));
						
		mrSwipeControls = (MeshRenderer)tMenuTransforms[(int)Menus.SettingsMenu].Find("ControlType/Button_Swipe/RadioButton_Foreground").GetComponent(typeof(MeshRenderer));
		mrGyroControls = (MeshRenderer)tMenuTransforms[(int)Menus.SettingsMenu].Find("ControlType/Button_Gyro/RadioButton_Foreground").GetComponent(typeof(MeshRenderer));
		mrMusicON = (MeshRenderer)tMenuTransforms[(int)Menus.SettingsMenu].Find("Music/Button_ON/RadioButton_Foreground").GetComponent(typeof(MeshRenderer));
		mrMusicOFF = (MeshRenderer)tMenuTransforms[(int)Menus.SettingsMenu].Find("Music/Button_OFF/RadioButton_Foreground").GetComponent(typeof(MeshRenderer));
		mrSoundON = (MeshRenderer)tMenuTransforms[(int)Menus.SettingsMenu].Find("Sound/Button_ON/RadioButton_Foreground").GetComponent(typeof(MeshRenderer));
		mrSoundOFF = (MeshRenderer)tMenuTransforms[(int)Menus.SettingsMenu].Find("Sound/Button_OFF/RadioButton_Foreground").GetComponent(typeof(MeshRenderer));
		
		//shop
		tMenuTransforms[(int)Menus.Shop] = tMenuGroup.Find("Shop").GetComponent(typeof(Transform)) as Transform;
		hShopScriptCS = (ShopScriptCS)tMenuTransforms[(int)Menus.Shop].GetComponent(typeof(ShopScriptCS));
			
		//missions menu
		tMenuTransforms[(int)Menus.MissionsMenu] = (Transform)tMenuGroup.Find("MissionsMenu").GetComponent(typeof(Transform));
		tMissionsMenuButtons = new Transform[iMissionsMenuButtonsCount];
		tMissionsMenuButtons[0] = (Transform)tMenuTransforms[(int)Menus.MissionsMenu].Find("Buttons/Button_Back").GetComponent(typeof(Transform));
		tmMissionsMenuMissionList = (TextMesh)tMenuTransforms[(int)Menus.MissionsMenu].Find("Text_MissionDescription").GetComponent(typeof(TextMesh));
		
		//achievements menu
		tMenuTransforms[(int)Menus.AchievementsMenu] = (Transform)tMenuGroup.Find("AchievementsMenu").GetComponent(typeof(Transform));
		tAchievementMenuButtons = new Transform[iAchievementMenuButtonsCount];
		tAchievementMenuButtons[0] = (Transform)tMenuTransforms[(int)Menus.AchievementsMenu].Find("Buttons/Button_Back").GetComponent(typeof(Transform));
		tmAchievementsMenuDescription = (TextMesh)tMenuTransforms[(int)Menus.AchievementsMenu].Find("Text_Achievements").GetComponent(typeof(TextMesh));
		
		//ad
		tMenuTransforms[(int)Menus.Ad] = (Transform)tMenuGroup.Find("Ad").GetComponent(typeof(Transform));
		tAdButtons = new Transform[iAdButtonsCount];
		tAdButtons[0] = (Transform)tMenuTransforms[(int)Menus.Ad].Find("Button_Ad").GetComponent(typeof(Transform));
		tAdButtons[1] = (Transform)tMenuTransforms[(int)Menus.Ad].Find("Button_Close").GetComponent(typeof(Transform));*/
		
		tLoadScreen = (Transform)tMenuGroup.Find("LoadingSplashScreen").GetComponent(typeof(Transform));//the loading splash screen
		tHUDGroup = (Transform)GameObject.Find("GUIGroup/HUDGroup").GetComponent(typeof(Transform));
		tmPauseCountdown = GameObject.Find("GUIGroup/HUDGroup/HUDPauseCounter").GetComponent(typeof(TextMesh)) as TextMesh;
		tmPauseCountdown.text = string.Empty;
		/*///////HUD//////
		((MeshRenderer)GameObject.Find("HUDMainGroup/HUDPauseCounter").GetComponent(typeof(MeshRenderer))).enabled = false;
		
		//set the HUD position according to the screen resolution
		((Transform)GameObject.Find("HUDMainGroup/HUDGroup/HUDCurrencyGroup").GetComponent(typeof(Transform))).transform.Translate(-fResolutionFactor,0,0);
		(GameObject.Find("HUDMainGroup/HUDGroup/HUDScoreGroup").GetComponent(typeof(Transform)) as Transform).transform.Translate(-fResolutionFactor,0,0);
		(GameObject.Find("HUDMainGroup/HUDGroup/HUDPause").GetComponent(typeof(Transform)) as Transform).transform.Translate(fResolutionFactor,0,0);*/
		
		Init();
			
		ShowMenu((int)Menus.MainMenu);	//show Main Menu on game launch
		
		//StartCoroutine(popUpAd());//start the coroutine to show that ad after unit time
	}
	
	private void Init()
	{
		bHUDState = true;
	}
	
	public void Restart()
	{
		Init();
		//ShowMenu((int)Menus.MainMenu);	//show Main Menu on game launch
	}
	
	/*
	*	FUNCTION: Show the Ad after every specific unit time.
	*/
	/*private IEnumerator popUpAd()
	{
		while (true)
		{
			yield return new WaitForSeconds(600);
			
			if (CurrentMenu >= 0 && CurrentMenu != (int)Menus.Ad)
			{
				PreviousMenu = CurrentMenu;
				ShowMenu((int)Menus.Ad);
			}
		}//end of while		
	}//end of pop-up ad*/
	
	private void toggleLoadSplash(bool state)
	{
		if (state)
			tLoadScreen.localPosition = new Vector3(tLoadScreen.localPosition.x, 0, tLoadScreen.localPosition.z);
		else
			tLoadScreen.localPosition = new Vector3(tLoadScreen.localPosition.x, 1000, tLoadScreen.localPosition.z);
	}
	
	public void pauseGame()
	{
		tmPauseCountdown.text = string.Empty;
		iResumeGameState = 0;
		ShowMenu((int)Menus.PauseMenu);
	}
	
	/*
	*	FUNCTION: Show the pause menu
	*	CALLED BY:	InGameController.Update()
	*/
	/*public void displayPauseMenu()
	{
		ShowMenu((int)Menus.PauseMenu);
	}*/
	
	/*
	*	FUNCTION: Show the game over menu
	*	CALLED BY:	InGameController.Update()
	*/
	/*public void displayGameOverMenu()
	{	
		ShowMenu((int)Menus.GameOverMenu);	
	}*/
	
	void FixedUpdate()
	{		
		//display countdown timer on Resume
		if (iResumeGameState == 1)//display the counter
		{	
			toggleHUDState(true);
			iResumeGameStartTime = Time.time;		
			iResumeGameState = 2;
		}
		else if (iResumeGameState == 2)//count down
		{
			tmPauseCountdown.text = Mathf.RoundToInt(4 - (Time.time - iResumeGameStartTime)).ToString();
			
			if ( (Time.time - iResumeGameStartTime) >= 3)//resume the game when time expires
			{
				tmPauseCountdown.text = string.Empty;
				hInGameController.processClicksPauseMenu(PauseMenuEvents.Resume);				
				iResumeGameState = 0;
			}
		}	
	}//end of fixed update
	
	void OnGUI()
	{
		listenerClicks();//listen for clicks
		
		if (Application.platform == RuntimePlatform.Android || Application.isEditor)
			handlerAndroidBackButton();
	}
	
	/*
	*	FUNCTION: Detect taps and call the relevatn event handler.
	*	CALLED BY:	The FixedUpdate() function.
	*/
	private RaycastHit hit;
	private void listenerClicks()
	{
		if (Input.GetMouseButtonDown(0) && iTapState == 0)//detect taps
		{	
			iTapState = 1;			
		}//end of if get mouse button
		else if (iTapState == 1)//call relevent handler
		{
			if (Input.GetMouseButtonUp(0))			
				iTapState = 2;
		}		
		else if (iTapState == 2)//wait for user to release before detcting next tap
		{
			if (Physics.Raycast(HUDCamera.ScreenPointToRay(Input.mousePosition), out hit))//if a button has been tapped		
			{
				//call the listner function of the active menu
				if (CurrentMenu == (int)Menus.MainMenu)
					handlerMainMenu(hit.transform);
				else if (CurrentMenu == (int)Menus.PauseMenu)
					handlerPauseMenu(hit.transform);
				else if (CurrentMenu == (int)Menus.GameOverMenu)
					handlerGameOverMenu(hit.transform);
				else if (CurrentMenu == (int)Menus.Revive)
					handlerReviveMenu(hit.transform);
				/*else if (CurrentMenu == (int)Menus.InstructionsMenu)
					handlerInstructionsMenu(hit.transform);
				else if (CurrentMenu == (int)Menus.SettingsMenu)
					handlerSettingsMenu(hit.transform);
				else if (CurrentMenu == (int)Menus.MissionsMenu)
					handlerMissionsMenu(hit.transform);
				else if (CurrentMenu == (int)Menus.AchievementsMenu)
					handlerAchievementsMenu(hit.transform);
				else if (CurrentMenu == (int)Menus.Ad)
					handlerAd(hit.transform);*/
			}//end of if raycast
						
			iTapState = 0;		
		}
	}//end of listner function
	
	/*
	 * FUNCTION:	Handler back button execution on android devices.
	 * 
	 * CALLED BY:	OnGUI()
	 * */
	private void handlerAndroidBackButton()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && iAndroidBackTapState == 0)
		{
			iAndroidBackTapState = 1;
		}//end of if input
		else if (iAndroidBackTapState == 1)
		{
			if (Input.GetKeyUp(KeyCode.Escape))
				iAndroidBackTapState = 2;
		}
		else if (iAndroidBackTapState == 2)
		{			
			if (CurrentMenu == (int)Menus.MainMenu)			
				Application.Quit();
			/*else if (CurrentMenu == (int)Menus.AchievementsMenu)
				handlerAchievementsMenu(tAchievementMenuButtons[0]);*/
			else if (CurrentMenu == (int)Menus.GameOverMenu)
				handlerGameOverMenu(tGameOverButtons[0]);
			/*else if (CurrentMenu == (int)Menus.InstructionsMenu)
				handlerInstructionsMenu(tInstructionsButtons[0]);
			else if (CurrentMenu == (int)Menus.MissionsMenu)
				handlerMissionsMenu(tMissionsMenuButtons[0]);*/
			else if (CurrentMenu == (int)Menus.PauseMenu)
				handlerPauseMenu(tPauseButtons[1]);
			/*else if (CurrentMenu == (int)Menus.SettingsMenu)
				handlerSettingsMenu(tSettingsButtons[0]);*/
			
			iAndroidBackTapState = 0;
		}
	}//end of handler android back button function
	
	/*
	*	FUNCTION: Handle clicks on Main Menu
	*/
	private void handlerMainMenu(Transform buttonTransform)
	{
		if (tMainMenuButtons[0] == buttonTransform)//Tap to Play button
		{
			CloseMenu((int)Menus.MainMenu);			
			hInGameController.launchGame();	//start the gameplay			
		}
		/*else if (tMainMenuButtons[1] == buttonTransform)//information button
		{
			CloseMenu((int)Menus.MainMenu);
			ShowMenu((int)Menus.InstructionsMenu);
			CurrentMenu = (int)Menus.InstructionsMenu;
		}
		else if (tMainMenuButtons[2] == buttonTransform)//settings button
		{
			CloseMenu((int)Menus.MainMenu);
			ShowMenu((int)Menus.SettingsMenu);		
		}
		else if (tMainMenuButtons[3] == buttonTransform)//shop button
		{
			CloseMenu((int)Menus.MainMenu);
					
			hShopScriptCS.setShopScriptEnabled(true);
			toggleMenuScriptStatus(false);
		}
		else if (tMainMenuButtons[4] == buttonTransform)//mission menu button
		{
			CloseMenu((int)Menus.MainMenu);
			ShowMenu((int)Menus.MissionsMenu);
		}
		else if (tMainMenuButtons[5] == buttonTransform)//achievements menu button
		{
			CloseMenu((int)Menus.MainMenu);
			ShowMenu((int)Menus.AchievementsMenu);
		}*/		
	}//end of handler main menu function
	
	/*
	*	FUNCTION: Handle clicks on pause menu.
	*/
	private void handlerPauseMenu(Transform buttonTransform)
	{
		if (tPauseButtons[0] == buttonTransform)//back button handler
		{
			//toggleLoadSplash(true);
			CloseMenu((int)Menus.PauseMenu);
			hInGameController.processClicksPauseMenu(PauseMenuEvents.MainMenu);
		}
		else if (tPauseButtons[1] == buttonTransform)//resume button handler
		{
			CloseMenu((int)Menus.PauseMenu);
			iResumeGameState = 1;//begin the counter to resume
		}
	}
	
	/*
	*	FUNCTION: Handle clicks on Game over menu.
	*/
	private void handlerGameOverMenu(Transform buttonTransform)
	{
		if (tGameOverButtons[0] == buttonTransform)//back button
		{
			//toggleLoadSplash(true);			
			CloseMenu((int)Menus.GameOverMenu);
			hInGameController.procesClicksDeathMenu(GameOverMenuEvents.Back);
			//ShowMenu((int)Menus.MainMenu);
		}
		else if (tGameOverButtons[1] == buttonTransform)//play button
		{
			//toggleLoadSplash(true);
			CloseMenu((int)Menus.GameOverMenu);
			hInGameController.procesClicksDeathMenu(GameOverMenuEvents.Play);			
		}
	}
	
	private void handlerReviveMenu(Transform buttonTransform)
	{
		if (tReviveButtons[0] == buttonTransform)
		{
			hInGameController.processClicksReviveMenu(ReviveMenuEvents.Revive);
			CloseMenu((int)Menus.Revive);
			toggleHUDState(true);
		}
	}
	
	/*
	*	FUNCTION: Handle the clicks on Information menu.
	*/
	/*private void handlerInstructionsMenu(Transform buttonTransform)
	{
		if (tInstructionsButtons[0] == buttonTransform)//main menu button
		{
			CloseMenu((int)Menus.InstructionsMenu);
			ShowMenu((int)Menus.MainMenu);		
		}	
	}*/
	
	/*
	*	FUNCTION: Handle the clicks on Information menu.
	*	CALLED BY:	listenerClicks()
	*/
	/*private void handlerSettingsMenu(Transform buttonTransform)
	{
		if (tSettingsButtons[0] == buttonTransform)//home button
		{
			CloseMenu((int)Menus.SettingsMenu);
			ShowMenu((int)Menus.MainMenu);
		}
		else if (tSettingsButtons[1] == buttonTransform)//swipe controls
		{		
			if (mrSwipeControls.enabled == false)
			{
				mrSwipeControls.enabled = true;
				mrGyroControls.enabled = false;
				hControllerScriptCS.toggleSwipeControls(true);
			}		
		}
		else if (tSettingsButtons[2] == buttonTransform)//gyro controls
		{		
			if (mrGyroControls.enabled == false)
			{
				mrGyroControls.enabled = true;
				mrSwipeControls.enabled = false;
				hControllerScriptCS.toggleSwipeControls(false);
			}		
		}
		else if (tSettingsButtons[3] == buttonTransform)//music ON radio button
		{
			if (mrMusicON.enabled == false)
			{
				mrMusicON.enabled = true;
				mrMusicOFF.enabled = false;
				hSoundManagerScriptCS.toggleMusicEnabled(true);
			}
		}
		else if (tSettingsButtons[4] == buttonTransform)//music OFF radio button
		{
			if (mrMusicON.enabled == true)
			{
				mrMusicON.enabled = false;
				mrMusicOFF.enabled = true;
				hSoundManagerScriptCS.toggleMusicEnabled(false);
			}
		}
		else if (tSettingsButtons[5] == buttonTransform)//music ON radio button
		{
			if (mrSoundON.enabled == false)
			{
				mrSoundON.enabled = true;
				mrSoundOFF.enabled = false;
				hSoundManagerScriptCS.toggleSoundEnabled(true);
			}
		}
		else if (tSettingsButtons[6] == buttonTransform)//music ON radio button
		{
			if (mrSoundON.enabled == true)
			{
				mrSoundON.enabled = false;
				mrSoundOFF.enabled = true;
				hSoundManagerScriptCS.toggleSoundEnabled(false);
			}
		}
	}*/
	
	/*
	*	FUNCTION:	Handle the clicks on the Mission Menu
	*	CALLED BY:	listenerClicks()
	*/
	/*private void handlerMissionsMenu(Transform buttonTransform)
	{
		if (tMissionsMenuButtons[0] == buttonTransform)
		{
			CloseMenu((int)Menus.MissionsMenu);
			ShowMenu((int)Menus.MainMenu);
		}
	}*/
	
	/*
	*	FUNCTION:	Handle the clicks on the Achievements Menu
	*	CALLED BY:	listenerClicks()
	*/
	/*private void handlerAchievementsMenu(Transform buttonTransform)
	{
		if (tAchievementMenuButtons[0] == buttonTransform)
		{
			CloseMenu((int)Menus.AchievementsMenu);
			ShowMenu((int)Menus.MainMenu);
		}
	}*/
		
	/*
	*	FUNCTION: Handle the clicks on Ads
	*/
	/*private void handlerAd(Transform buttonTransform)
	{
		if (tAdButtons[0] == buttonTransform)
		{
			Application.OpenURL("http://bit.ly/1fIMCdr");
			CloseMenu((int)Menus.Ad);
			ShowMenu(PreviousMenu);
		}
		else if (tAdButtons[1] == buttonTransform)
		{
			CloseMenu((int)Menus.Ad);
			ShowMenu(PreviousMenu);
		}
	}*/
	
	/*
	*	FUNCTION: Set the menu to show in front of the HUD Camera
	*/
	public void ShowMenu(int index)
	{		
		if ((int)Menus.GameOverMenu == index)
		{
			CloseMenu((int)Menus.Revive);
		}
		/*if ((int)Menus.SettingsMenu == index)
		{
			//check which type of controls are active and 
			//set the appropriate radio button 
			if ( hControllerScriptCS.isSwipeControlEnabled() )
			{
				mrSwipeControls.enabled = true;
				mrGyroControls.enabled = false;
			}
			else
			{
				mrSwipeControls.enabled = false;
				mrGyroControls.enabled = true;
			}
			
			//check if the music is enabled or disabled and
			//set the appropriate radio button
			if (hSoundManagerScriptCS.isMusicEnabled())
			{
				mrMusicON.enabled = true;
				mrMusicOFF.enabled = false;
			}
			else
			{
				mrMusicON.enabled = false;
				mrMusicOFF.enabled = true;
			}
			
			//check if the sound is ON or OFF and se the
			//appropriate radio button
			if (hSoundManagerScriptCS.isSoundEnabled())
			{
				mrSoundON.enabled = true;
				mrSoundOFF.enabled = false;
			}
			else
			{
				mrSoundON.enabled = false;
				mrSoundOFF.enabled = true;
			}			
		}
		else if ((int)Menus.MissionsMenu == index)//display mission menu
			hMissionsControllerCS.updateMenuDescriptions();//list the mission on the missions menu*/
				
		tMenuTransforms[index].localPosition = new Vector3(tMenuTransforms[index].localPosition.x,
			0, tMenuTransforms[index].localPosition.z);//move the menu in front of the HUD Camera
		
		CurrentMenu = index;//set the current menu
		toggleHUDState(false);	//hide the HUD
		//hSoundManagerScriptCS.playSound(SoundManagerCS.MenuSounds.ButtonTap);
	}
	
	/*
	*	FUNCTION: Send the menu away from the HUD Camera
	*/
	private void CloseMenu(int index)
	{	
		tMenuTransforms[index].position = new Vector3(tMenuTransforms[index].position.x,
			1000, tMenuTransforms[index].position.z);//move the menu away from the camera
		
		CurrentMenu = -1;
	}
	
	/*
	*	FUNCTION:	Display description of the currently active
	*				missions on Pause Menu.
	*/
	/*public void updatePauseMenuMissions(string description)
	{
		tmPauseMenuMissionList.text = description;
	}*/
	
	/*
	*	FUNCTION:	Display the currently active mission on the Mission Menu.
	*/
	/*public void updateMissionsMenuMissions(string description)
	{
		tmMissionsMenuMissionList.text = description;
	}*/
	
	/*
	*	FUNCTION:	Display the currently active mission on the Achievements Menu.
	*/
	/*public void updateAchievementsMenuDescription(string description)
	{
		tmAchievementsMenuDescription.text = description;
	}*/
	
	public void toggleHUDState(bool state)
	{
		if (state == bHUDState)//if state has not changed
			return;
		
		if (state)
			tHUDGroup.localPosition = new Vector3(0, 0, 0);//show HUD
		else
			tHUDGroup.localPosition = new Vector3(0, 1000, 0);//hide HUD
		
		bHUDState = state;//update HUD status
	}
	
	public bool isHUDEnabled()
	{
		return bHUDState;
	}
	
	/*
	*	FUNCTION: Enable/ disable MenuScript.
	*	CALLED BY: InGameController.Update()
	*	ADDITIONAL INFO: The MenuScript is disabled during gameplay for improved performance.
	*/
	public void toggleMenuScriptStatus(bool flag)
	{
		if (flag != this.enabled)
			this.enabled = flag;
	}
}
