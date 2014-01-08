﻿using UnityEngine;
using System.Collections;

public class PrimaryColliderController : MonoBehaviour {
	
	private Collider primaryCollider;
	
	private PlayerController hPlayerController;
	private InGameController hInGameController;	
	private EnemyController hEnemyController;
	//private SecondaryColliderController hSecondaryColliderController;
	
	void Start () 
	{
		hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
		hPlayerController = (PlayerController)GameObject.Find("Player").GetComponent(typeof(PlayerController));
		/*hSecondaryColliderController = (SecondaryColliderController)GameObject
			.Find("Player/CharacterGroup/Colliders/SecondaryCollider").GetComponent(typeof(SecondaryColliderController));*/
		
		primaryCollider = this.collider;
	}
	
	public void Restart()
	{
		togglePrimaryCollider(true);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if (hPlayerController.isInJump())//if the player was in air
		{
			hEnemyController.handleStumble();//call stumble instead of game over
			togglePrimaryCollider(false);
		}
		else//regular case
		{
			StartCoroutine(hInGameController.routineGameOver());
		}
	}
	
	void OnCollisionExit(Collision collision)
	{
		if (!isPrimaryColliderEnabled())//if a forced stumble was called
			togglePrimaryCollider(true);
	}
	
	public void togglePrimaryCollider(bool state)
	{
		primaryCollider.enabled = state;
	}
	
	public bool isPrimaryColliderEnabled()
	{
		return primaryCollider.enabled;
	}
}
