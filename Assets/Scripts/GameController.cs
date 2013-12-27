using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	
	private MenuScript hMenuScript;
	private InGameController hInGameController;
	private PlayerController hPlayerController;
		
	void Start () 
	{
		Application.targetFrameRate = 60;		//ceiling the frame rate on 60 (debug only)
	}
		
	private void relaunchGame()
	{
		
	}
	
	
}