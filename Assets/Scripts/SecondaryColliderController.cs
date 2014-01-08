using UnityEngine;
using System.Collections;

public class SecondaryColliderController : MonoBehaviour {
	
	private Collider secondaryCollider;
		
	/*private EnemyController hEnemyController;
	private PlayerController hPlayerController;*/
	private InGameController hInGameController;
	private PrimaryColliderController hPrimaryColliderController;
	
	void Start()
	{
		/*hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
		hPlayerController = (PlayerController)GameObject.Find("Player").GetComponent(typeof(PlayerController));*/
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
		hPrimaryColliderController = (PrimaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/PrimaryCollider").GetComponent(typeof(PrimaryColliderController));
		
		secondaryCollider = this.collider;
	}
	
	public void Restart()
	{
		toggleSecondaryCollider(true);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		hInGameController.handleStumble();
		hPrimaryColliderController.togglePrimaryCollider(true);
		toggleSecondaryCollider(true);
	}
	
	/*void OnCollisionExit(Collision collision)
	{
		toggleSecondaryCollider(true);
		hPrimaryColliderController.togglePrimaryCollider(true);
	}*/
	
	public void toggleSecondaryCollider(bool state)
	{
		secondaryCollider.enabled = state;
	}
	
	public bool isSecondaryColliderEnabled()
	{
		return secondaryCollider.enabled;
	}
}
