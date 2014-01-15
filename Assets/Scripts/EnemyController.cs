using UnityEngine;
using System.Collections;

public enum EnemyAnimation
{ run, jump, strafe_right, strafe_left, slide }

public class EnemyController : MonoBehaviour {
	
	#region Constants
	//private const float fPlayerEnemyDisplacement = 8.0f;
	private const float fPlayerEnemyDistanceActive = 1.2f;
	private const float fPlayerEnemyDistanceInactive = 2;
	private const float fActiveAcclearation = 25;
	private const float fInactiveAccleration = 25;
	private const float fFollowDuration = 5;//time in seconds till when to follow player
	
	private const float fRunAnimationSpeed = 0.75f;
	#endregion
	
	private Transform tPlayer;
	private Transform tPlayerCharacter;
	private Transform tEnemy;
	private Animation aEnemy;
		
	private float fAccleration;
	private float fPlayerEnemyDistance = 5;
	private int EnemyState;
	private float fCurrentFollowTime;
	//private Vector3 previousForwardUnitVector;
	private Vector3 currentForwardUnitVector;
	
	private InGameController hInGameController;
	private PlayerController hPlayerController;
	private SecondaryColliderController hSecondaryColliderController;
	
	void Start () 
	{	
		hInGameController = GameObject.Find("Player").GetComponent<InGameController>();
		hPlayerController = GameObject.Find("Player").GetComponent<PlayerController>();
		hSecondaryColliderController = GameObject.Find("Player/CharacterGroup/Colliders/SecondaryCollider")
			.GetComponent<SecondaryColliderController>();
		
		tPlayer = GameObject.Find("Player").transform;
		tPlayerCharacter = GameObject.Find("Player/CharacterGroup").transform;
		tEnemy = this.transform;
		aEnemy = (Animation)this.transform.Find("Qassai").GetComponent(typeof(Animation));
		
		Init();
	}
	
	void Init()
	{
		tEnemy.position = new Vector3(tPlayerCharacter.position.x, tPlayerCharacter.position.y,
			tPlayerCharacter.position.z-fPlayerEnemyDistance);//reset enemy position
		tEnemy.rotation = Quaternion.identity;//reset enemy location
		
		fPlayerEnemyDistance = fPlayerEnemyDistanceActive;
		fAccleration = fInactiveAccleration;//reset enemy accleration
		EnemyState = 0;
		
		toggleEnemyAnimation(false);
	}
	
	public void Restart()
	{		
		Init();
	}
	
	public void launchGame()
	{
		toggleEnemyAnimation(true);
		aEnemy["run"].speed = fRunAnimationSpeed;
		aEnemy.Play("run");
		StartCoroutine(followPlayer());
	}
	
	void FixedUpdate ()
	{
		if (hInGameController.isGamePaused())
			return;
				
		currentForwardUnitVector = hPlayerController.getCurrentForwardUnitVector();
		
		if (Mathf.Abs(currentForwardUnitVector.x) < Mathf.Abs(currentForwardUnitVector.z) )
		{
			tEnemy.position = Vector3.Lerp(tEnemy.position, 
				new Vector3(tPlayerCharacter.position.x, tPlayerCharacter.position.y,
				tPlayer.position.z- (fPlayerEnemyDistance*currentForwardUnitVector.z) ),
				Time.deltaTime*fAccleration);
		}
		else
		{
			tEnemy.position = Vector3.Lerp(tEnemy.position, 
				new Vector3(tPlayer.position.x- (fPlayerEnemyDistance*currentForwardUnitVector.x),
				tPlayerCharacter.position.y, tPlayerCharacter.position.z),
				Time.deltaTime*fAccleration);
		}
	}
	
	public void handleStumble()
	{
		if (EnemyState == 0)
			StartCoroutine(followPlayer());
		else if (EnemyState > 0 && !hInGameController.isGameOverRoutineActive())		
			StartCoroutine(hInGameController.routineGameOver());
	}
	
	public void revivePlayer()
	{
		EnemyState = 0;
		StopCoroutine("followPlayer");
		StartCoroutine(followPlayer());
		
		toggleEnemyAnimation(true);
		playEnemyAnimation(EnemyAnimation.run);
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
				fPlayerEnemyDistance = fPlayerEnemyDistanceActive;
				
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
				fPlayerEnemyDistance = fPlayerEnemyDistanceInactive;
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
		
		(aEnemy.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers) )
			.speed = fRunAnimationSpeed;
		
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
		{			
			(aEnemy.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers) )
				.speed = fRunAnimationSpeed;			
		}
		else if (anim == EnemyAnimation.jump)
			aEnemy.CrossFade("jump", 0.5f);
		else if (anim == EnemyAnimation.slide)
			aEnemy.CrossFade("slide", 0.5f);
		else if (anim == EnemyAnimation.strafe_left)
			aEnemy.Play("strafe_left");
		else if (anim == EnemyAnimation.strafe_right)
			aEnemy.Play("strafe_right");
	}
	
	public bool isEnemyActive() 
	{
		if (EnemyState > 0)
			return true;
		else
			return false;
	}
	
	public void toggleEnemyAnimation(bool state)
	{
		if (aEnemy.enabled != state)
			aEnemy.enabled = state;
	}
}
