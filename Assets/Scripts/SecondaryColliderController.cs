using UnityEngine;
using System.Collections;

public class SecondaryColliderController : MonoBehaviour {
	
	private Collider secondaryCollider;
	
	private EnemyController hEnemyController;
	
	void Start()
	{
		hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
		
		secondaryCollider = this.collider;
	}
	
	public void Restart()
	{
		toggleSecondaryCollider(true);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		hEnemyController.handleStumble();
		toggleSecondaryCollider(false);
	}
	
	void OnCollisionExit(Collision collision)
	{
		toggleSecondaryCollider(true);
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
