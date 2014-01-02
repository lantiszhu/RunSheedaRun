﻿using UnityEngine;
using System.Collections;

public enum EnemyAnimation
{ run, jump, strafe_right, strafe_left, slide }

public class EnemyController : MonoBehaviour {
	
	#region Constants
	private const float fPlayerEnemyDistance = 160.0f;
	private const float fActiveAcclearation = 10;
	private const float fInactiveAccleration = 4;
	private const float fFollowDuration = 5;//time in seconds till when to follow player
	#endregion
	
	private Transform tPlayerCharacter;
	private Transform tEnemy;
	private Animation aEnemy;
		
	private float fAccleration;
	private int EnemyState;
	private float fCurrentFollowTime;
	
	private InGameController hInGameController;
	
	void Start () 
	{
		hInGameController = (InGameController)GameObject.Find("Player").GetComponent(typeof(InGameController));
		
		tPlayerCharacter = GameObject.Find("Player/CharacterGroup").transform;
		tEnemy = this.transform;
		aEnemy = (Animation)this.transform.Find("Qassai").GetComponent(typeof(Animation));
		
		tEnemy.position = new Vector3(tPlayerCharacter.position.x, tPlayerCharacter.position.y,
			tPlayerCharacter.position.z-fPlayerEnemyDistance);
		fAccleration = fInactiveAccleration;
		EnemyState = 0;
		
		toggleEnemyAnimation(false);
	}
	
	public void Restart()
	{		
		tEnemy.position = new Vector3(tPlayerCharacter.position.x, tPlayerCharacter.position.y,
			tPlayerCharacter.position.z-fPlayerEnemyDistance);//reset enemy position
		tEnemy.rotation = Quaternion.identity;//reset enemy location
		
		fAccleration = fInactiveAccleration;//reset enemy accleration
		EnemyState = 0;
		
		toggleEnemyAnimation(false);
	}
	
	public void launchGame()
	{
		toggleEnemyAnimation(true);
		aEnemy.Play("run");
		StartCoroutine(followPlayer());
	}
	
	void FixedUpdate () 
	{
		if (hInGameController.isGamePaused())
			return;
		
		tEnemy.position = Vector3.Lerp(tEnemy.position, tPlayerCharacter.position, Time.deltaTime*fAccleration);
	}
	
	public void handleStumble()
	{
		if (EnemyState == 0)
			StartCoroutine(followPlayer());
		else
			StartCoroutine(hInGameController.routineGameOver());
	}
	
	private IEnumerator followPlayer()
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();
			
			if (EnemyState == 0)
			{
				fCurrentFollowTime = Time.time;
				fAccleration = fActiveAcclearation;
				
				EnemyState = 1;
			}
			else if (EnemyState == 1)
			{
				if ( (Time.time-fCurrentFollowTime) > fFollowDuration)
					EnemyState = 2;
			}
			else if (EnemyState == 2)
			{
				fAccleration = fInactiveAccleration;
				EnemyState = 0;
				
				break;
			}
		}//end of while
		
		StopCoroutine("followPlayer");
	}//end of follow Player coroutine
	
	public IEnumerator rotateEnemy(SwipeDirection direction)
	{
		Quaternion newRoation = Quaternion.identity;
				
		if (direction == SwipeDirection.Right)
			newRoation = Quaternion.Euler(0, 90, 0) * tEnemy.rotation;//right turn
		else if (direction == SwipeDirection.Left)
			newRoation = Quaternion.Euler(0, -90, 0) * tEnemy.rotation;//left turn
		
		//play turn animation
		if (direction == SwipeDirection.Right)
		{
			aEnemy["strafe_right"].speed = 0.5f;
			aEnemy.Play("strafe_right");
		}
		else if (direction == SwipeDirection.Left)
		{
			aEnemy["strafe_left"].speed = 0.5f;
			aEnemy.Play("strafe_left");
		}
		//aEnemy["run"].speed = fRunAnimationSpeed;
		aEnemy.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers);
		
		while (true)
		{
			yield return new WaitForEndOfFrame();
						
			tEnemy.rotation = Quaternion.Slerp(tEnemy.rotation, newRoation, Time.deltaTime*10);
			
			if (tEnemy.rotation == newRoation)//escape condition
			{	
				tEnemy.rotation = newRoation;
				break;
			}			
		}//end of while
		
		StopCoroutine("rotatePlayer");
	}
	
	public void playEnemyAnimation(EnemyAnimation anim)
	{
		if (anim == EnemyAnimation.run)
			aEnemy.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers);
		else if (anim == EnemyAnimation.jump)
			aEnemy.CrossFade("jump", 0.5f);
		else if (anim == EnemyAnimation.slide)
			aEnemy.CrossFade("slide", 0.5f);
		else if (anim == EnemyAnimation.strafe_left)
			aEnemy.Play("strafe_left");
		else if (anim == EnemyAnimation.strafe_right)
			aEnemy.Play("strafe_right");
	}
	
	public void toggleEnemyAnimation(bool state)
	{
		aEnemy.enabled = state;
	}
}