using UnityEngine;
using System.Collections;

public class PrimaryColliderController : MonoBehaviour {
	
	private InGameController hInGameController;
	
	void Start () 
	{
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
	}
	
	void OnCollisionEnter(Collision collision)
	{
		//hInGameController.routineGameOver();
	}
}
