using UnityEngine;
using System.Collections;

public class SecondaryColliderController : MonoBehaviour {
	
	private Collider secondaryCollider;
	
	private EnemyController hEnemyController;
	private PrimaryColliderController hPrimaryColliderController;
	
	void Start()
	{
		hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
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
		hEnemyController.handleStumble();
		hPrimaryColliderController.togglePrimaryCollider(false);
		toggleSecondaryCollider(false);
	}
	
	void OnCollisionExit(Collision collision)
	{
		toggleSecondaryCollider(true);
		hPrimaryColliderController.togglePrimaryCollider(true);
	}
	
	public void toggleSecondaryCollider(bool state)
	{
		secondaryCollider.enabled = state;
	}
	
	public bool isSecondaryColliderEnabled()
	{
		return secondaryCollider.enabled;
	}
}
