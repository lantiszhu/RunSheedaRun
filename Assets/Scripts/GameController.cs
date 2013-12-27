using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	private MenuScript hMenuScript;
	private PatchController hPatchController;
	private InGameController hInGameController;
	private PlayerController hPlayerController;	
		
	void Start () 
	{
		Application.targetFrameRate = 60;		//ceiling the frame rate on 60 (debug only)
		
		hMenuScript = (MenuScript)GameObject.Find("GUIGroup/MenuGroup").GetComponent(typeof(MenuScript));
		hPatchController = (PatchController)this.GetComponent(typeof(PatchController));
		hInGameController = (InGameController)this.GetComponent(typeof(InGameController));
		hPlayerController = (PlayerController)this.GetComponent(typeof(PlayerController));
	}
		
	public void relaunchGame()
	{		
		hPatchController.Restart();
		hInGameController.Restart();
		hPlayerController.Restart();
		hMenuScript.Restart();
		
		System.GC.Collect();
	}
	
	
}