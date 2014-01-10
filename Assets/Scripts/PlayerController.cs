using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	#region Constants	
	private const float fStartForwardSpeed = 12.0f;	//player's speed when game starts
	private const float fLanePositionThreshold = 1.5f;//distance from the center
	private const float fLaneSwitchSpeed = 5.0f;//how fast to strafe
	
	private const float fGravity = 270.0f;	//the value with which to pull down the player
	private const float fJumpForce = 120;
	private const float fVerticalAccleration = 5;
	
	private const float fDuckDuration = 1.0f;
	private const float fDuckColliderScaleDownFactor = 3;
	private const float fDuckColliderTranslationFactor = 0.8f;
	
	private const float fSwitchMidNodeThreshold = 3;//when to switch to the next mid node
	private const float fTurnSwipeThreshold = 10.0f;//how close to the mid node the player should turn
	private const float fTurnRotateThreshold = 2f;//when to rotate on axis
	#endregion
	
	#region Global Variables
	private Transform tPlayer;
	private Animation aPlayer;
	private Transform tCharacter;
	private Transform tShadow;
	private Transform tPrimaryCollider;
	private Transform tSecondaryCollider;
	
	private PatchController.Patch currentPatch;//informaiton of the current patch
	private PatchController.Patch nextPatch;	//information of the next patch
	private PatchController.Patch turnPatch;	//transform of the node on which to turn
	/*private Transform currentMidNode;	//transform of the current mid node
	private Transform nextMidNode;		//transform of the next mid node
	private Transform turnPatchMidNode;	*/
		
	private float fCurrentForwardSpeed;
	private float fRunAnimationSpeed;
	private Vector3 forwardUnitVector;//direction of player
	private Vector2 initialScorePosition;
	private Vector2 finalScorePosition;
	private float deltaScorePosition;	//score earned during movement in delta time
	
	//horizontal position data
	private int currentLane;// -1, 0 or 1
	private int previousLane;//send back to previous lane in case of stumble
	private float currentHorizontalPosition;//position when lanes are switched
	private float previousHorizontalPosition;
	private float deltaHorizontalPosition;
	
	//vertical position data
	private float fVerticalPosition;
	private float fRayContactPosition;
	
	//private bool enteredTurnRadius;
	private bool ControlsEnabled;
	private int JumpState;
	private int DuckState;
	private float fDuckStartTime;
		
	private bool bGroundHit;//if there is terrain under the player
	private RaycastHit hitInfo;
	#endregion
	
	#region Script References
	private PatchController hPatchController;
	private SwipeControls hSwipeControls;
	private InGameController hInGameController;
	private EnemyController hEnemyController;
	private CameraController hCameraController;
	#endregion
	
	void Start () 
	{
		hPatchController = (PatchController)this.GetComponent(typeof(PatchController));
		hSwipeControls = (SwipeControls)this.GetComponent(typeof(SwipeControls));
		hInGameController = (InGameController)this.GetComponent(typeof(InGameController));
		hEnemyController = (EnemyController)GameObject.Find("Enemy").GetComponent(typeof(EnemyController));
		hCameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
		
		tPlayer = this.transform;
		aPlayer = (Animation)this.transform.Find("CharacterGroup/Sheeda").GetComponent(typeof(Animation));
		tCharacter = this.transform.Find("CharacterGroup").transform;
		tShadow = this.transform.Find("CharacterGroup/Shadow");
		tPrimaryCollider = this.transform.Find("CharacterGroup/Colliders/PrimaryCollider");
		tSecondaryCollider = this.transform.Find("CharacterGroup/Colliders/SecondaryCollider");
		
		Init();
	}
	
	private void Init()
	{
		currentLane = 0;
		fCurrentForwardSpeed = fStartForwardSpeed;
		fRunAnimationSpeed = 0.8f;//run animation's speed
		fVerticalPosition = 0;
		fRayContactPosition = 0;
		turnPatch = null;
		
		//enteredTurnRadius = false;
		ControlsEnabled = false;
		JumpState = 0;
		DuckState = 0;
		
		//get patch information
		currentPatch = hPatchController.getCurrentPatch();
		nextPatch = hPatchController.getnextPatch();
		
		tPlayer.position = currentPatch.startNode.position;	//set player's start position
		tPlayer.rotation = Quaternion.identity;
		tCharacter.localPosition = Vector3.zero;	//reset character group's position
		
		//calculate the direction in which the player has to move
		forwardUnitVector = (nextPatch.midNode.position - tPlayer.position)/
			MathCustom.VectorDistanceXZ(nextPatch.midNode.position, tPlayer.position);
		
		togglePlayerAnimation(false);//disable animation
	}
	
	public void Restart()
	{				
		Init();
	}
	
	/*
	*	FUNCTION: Enable controls, start player animation and movement
	*/
	public void launchGame()
	{		
		togglePlayerAnimation(true);//enable animations
		aPlayer["run"].speed = fRunAnimationSpeed;
		aPlayer.Play("run");	//play run animation
		
		ControlsEnabled = true;
	}
	
	/*void Update()
	{
		if (hInGameController.isGamePaused())
			return;
		
		
	}*/
	
	void FixedUpdate () 
	{
		if (hInGameController.isGamePaused())
			return;
		
		setVerticalPosition();//set the position in y-axis
		setForwardPosition();//set the position in which player is running
				
		setHorizontalPosition();//set lane position
		
		if (ControlsEnabled)
			handlerSwipes();//handle user commands
		
		//get next patch's mid node when user reaches a mid node
		if (MathCustom.VectorDistanceXZ(tPlayer.position, nextPatch.midNode.position) 
			<= fSwitchMidNodeThreshold 
			 /*&& enteredTurnRadius == false*/)
		{//print("regular switch condition executed");
			hPatchController.updatePatch();//tell patch controller to switch to next mid node
			updateNextMidNode();//get the detail of the next mid node
		}//end of if
		
		/*if (turnPatch != null && enteredTurnRadius == false
			&& MathCustom.VectorDistanceXZ(tPlayer.position, turnPatch.midNode.position) <= fTurnSwipeThreshold)
		{print("condition 2 executed");
			enteredTurnRadius = true;
		}
		else if (enteredTurnRadius == true && turnPatch == null)//user made the turn
		{print("condition 3 executed");
			
			enteredTurnRadius = false;
		}
		else if (enteredTurnRadius == true
			&& MathCustom.VectorDistanceXZ(tPlayer.position, turnPatch.midNode.position) > fTurnSwipeThreshold)//user did not make the turn
		{print("condition 4 executed");
			hPatchController.updatePatch();//tell patch controller to switch to next mid node
			updateNextMidNode();
			
			enteredTurnRadius = false;
		}*/
		
	}//end of fixed update
	
	private void updateNextMidNode()
	{
		currentPatch = nextPatch;	//record the next patch information
		nextPatch = hPatchController.getnextPatch();//get the next patch info
		
		if (nextPatch.patchType != PatchTypes.straight)
			turnPatch = nextPatch;
				
		//update direction if the next patch is straight ahead
		// the turnPlayer functions call this function manually on turns
		if (MathCustom.AngleDir(currentPatch.midNode.forward, nextPatch.midNode.position, currentPatch.midNode.up) == 0)		
			updateForwardUnitVector();
	}
	
	public void routineGameOver()
	{
		aPlayer.Play("stumble");
		//togglePlayerAnimation(false);
	}
	
	public void revivePlayer()
	{
		togglePlayerAnimation(true);
		aPlayer["run"].speed = fRunAnimationSpeed;
		aPlayer.Play("run");
	}
	
	public void handleStumble()
	{
		aPlayer.Play("stumble");
		aPlayer.PlayQueued("run", QueueMode.CompleteOthers);
		
		//revert back to previous lane if the user was changing lanes
		if (isPlayerChangingLane())
		{
			currentLane = previousLane;
			hCameraController.changeLane(currentLane);
		}
	}//end of handle sumble function
	
	/*
	 * FUNCTION:	Control the user's forward movement.
	 * */
	private void setVerticalPosition()
	{
		bGroundHit = Physics.Linecast(tPlayer.position + new Vector3(0,5,0), tPlayer.position + new Vector3(0,-10,0), out hitInfo, (1<<8));
				
		if (bGroundHit)
		{
			fRayContactPosition = hitInfo.transform.position.y  + 0.1f;
			//set the position of a shadow
			tShadow.position = new Vector3(tCharacter.position.x, hitInfo.transform.position.y + 0.1f, tCharacter.position.z);
		}
		else
			fRayContactPosition -= (fGravity*Time.deltaTime);//make the character fall if there is no terrain under player
		
		if (JumpState == 0)//on ground
			fVerticalPosition = fRayContactPosition;
		else if (JumpState == 1)//jump triggered
		{
			aPlayer["jump"].speed = 1f;
			aPlayer.Play("jump");
			hEnemyController.playEnemyAnimation(EnemyAnimation.jump);
			
			fVerticalPosition += fJumpForce;			
			JumpState = 2;
		}
		else if (JumpState == 2)//in air
		{
			fVerticalPosition -= (fGravity*Time.deltaTime);
			
			if (tPlayer.position.y <= fRayContactPosition)//reached the ground
			{
				(aPlayer.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers) )
					.speed = fRunAnimationSpeed;
				hEnemyController.playEnemyAnimation(EnemyAnimation.run);
				
				JumpState = 0;
			}
		}
		
		tPlayer.position = new Vector3(tPlayer.position.x,
			MathCustom.LerpLinear(tPlayer.position.y, fVerticalPosition, Time.deltaTime*fVerticalAccleration),
			tPlayer.position.z);		
	}//end of set Vertical Position function
		
	private void setForwardPosition()
	{
		initialScorePosition = finalScorePosition;//keep record of previous position
		tPlayer.position += forwardUnitVector * Time.deltaTime * fCurrentForwardSpeed;
		
		finalScorePosition = new Vector2(tPlayer.position.x, tPlayer.position.z);//get next position
		deltaScorePosition = Vector2.Distance(finalScorePosition,initialScorePosition);//calculate distance
	}//end of set Forward Position function
	
	public float getDistanceCoveredInDeltaTime() { return deltaScorePosition; }
	
	private void updateForwardUnitVector()
	{		
		tPlayer.position = new Vector3(currentPatch.midNode.position.x, tPlayer.position.y,
			currentPatch.midNode.position.z);
		forwardUnitVector = (nextPatch.midNode.position-currentPatch.midNode.position)/
			MathCustom.VectorDistanceXZ(nextPatch.midNode.position, currentPatch.midNode.position);		
	}
	
	/// <summary>
	/// Sets the horizontal position of the character in a lane.
	/// </summary>
	private void setHorizontalPosition()
	{	
		previousHorizontalPosition = currentHorizontalPosition;
				
		if (currentLane == 0)//mid lane
		{
			tCharacter.localPosition = new Vector3(currentHorizontalPosition, tCharacter.localPosition.y, tCharacter.localPosition.z);
			currentHorizontalPosition = MathCustom.LerpLinear(currentHorizontalPosition, 0, Time.deltaTime*fLaneSwitchSpeed);
		}
		else if (currentLane == -1)//left lane
		{
			tCharacter.localPosition = new Vector3(currentHorizontalPosition, tCharacter.localPosition.y, tCharacter.localPosition.z);
			currentHorizontalPosition = MathCustom.LerpLinear(currentHorizontalPosition, -fLanePositionThreshold,
				Time.deltaTime*fLaneSwitchSpeed);
		}
		else if (currentLane == 1)//right lane
		{				
			tCharacter.localPosition = new Vector3(currentHorizontalPosition, tCharacter.localPosition.y, tCharacter.localPosition.z);
			currentHorizontalPosition = MathCustom.LerpLinear(currentHorizontalPosition, +fLanePositionThreshold,
				Time.deltaTime*fLaneSwitchSpeed);
		}
		
		//check if character is moving horizontally
		deltaHorizontalPosition = previousHorizontalPosition-currentHorizontalPosition;
	}//end of set Horizontal Position
	
	private bool isPlayerChangingLane()
	{
		if (deltaHorizontalPosition == 0)
			return false;
		else
			return true;
	}
	
	/// <summary>
	/// Handlers the swipes.
	/// </summary>
	SwipeDirection swipeDirection;
	private void handlerSwipes()
	{
		swipeDirection = hSwipeControls.getSwipeDirection();
		
		//handle strafes or turns
		if (swipeDirection == SwipeDirection.Right || swipeDirection == SwipeDirection.Left)
		{
			try
			{
				if (turnPatch != null
					&& MathCustom.VectorDistanceXZ(tPlayer.position, turnPatch.midNode.position) <= fTurnSwipeThreshold)
					StartCoroutine(turnPlayerOnNextMidNode(swipeDirection));
				else
					changeLane(swipeDirection);
			}//end of try
			catch (System.Exception) { changeLane(swipeDirection); }
		}		
		else if (swipeDirection == SwipeDirection.Jump)
		{
			if (!isInJump())
				JumpState = 1;
		}
		else if (swipeDirection == SwipeDirection.Duck)
		{
			if (!isInDuck())
				StartCoroutine(routineDuck());
		}
	}//end of handler Swipes function
	
	private IEnumerator routineDuck()
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();
			
			if (DuckState == 0)//start duck
			{
				//reduce the size of colliders
				tPrimaryCollider.localScale /= fDuckColliderScaleDownFactor;
				tSecondaryCollider.localScale /= fDuckColliderScaleDownFactor;
				tPrimaryCollider.localPosition = new Vector3(tPrimaryCollider.localPosition.x,
					tPrimaryCollider.localPosition.y - fDuckColliderTranslationFactor,
					tPrimaryCollider.localPosition.z);
				tSecondaryCollider.localPosition = new Vector3(tSecondaryCollider.localPosition.x,
					tSecondaryCollider.localPosition.y - fDuckColliderTranslationFactor,
					tSecondaryCollider.localPosition.z);
				
				//play the slide or roll animation
				if (UnityEngine.Random.Range(0,2) == 0)
					aPlayer.Play("slide");
				else
				{
					aPlayer["roll"].speed = 1.5f;
					aPlayer.Play("roll");
				}
				hEnemyController.playEnemyAnimation(EnemyAnimation.slide);
				
				fDuckStartTime = Time.time;//check when the duck started
				DuckState = 1;
			}
			else if (DuckState == 1)//wait for duck duration to pass
			{
				if ( (Time.time-fDuckStartTime) >= fDuckDuration)
					DuckState = 2;
			}
			else if (DuckState == 2)//exit duck state
			{
				//set the size of colliders to default
				tPrimaryCollider.localScale *= fDuckColliderScaleDownFactor;
				tSecondaryCollider.localScale *= fDuckColliderScaleDownFactor;
				tPrimaryCollider.localPosition = new Vector3(tPrimaryCollider.localPosition.x,
					tPrimaryCollider.localPosition.y + fDuckColliderTranslationFactor,
					tPrimaryCollider.localPosition.z);
				tSecondaryCollider.localPosition = new Vector3(tSecondaryCollider.localPosition.x,
					tSecondaryCollider.localPosition.y + fDuckColliderTranslationFactor,
					tSecondaryCollider.localPosition.z);
				
				aPlayer["run"].speed = fRunAnimationSpeed;
				aPlayer.CrossFade("run", 0.5f);//play run animation
				hEnemyController.playEnemyAnimation(EnemyAnimation.run);
				
				DuckState = 0;
				break;
			}
		}//end of while
		
		StopCoroutine("routineDuck");
	}
	
	/// <summary>
	/// Changes the lane.
	/// </summary>
	/// <param name='direction'>
	/// Direction.
	/// </param>
	private void changeLane(SwipeDirection direction)
	{
		if (swipeDirection == SwipeDirection.Right)
		{
			aPlayer["strafe_right"].speed = 1;
			aPlayer.Play("strafe_right");
			hEnemyController.playEnemyAnimation(EnemyAnimation.strafe_right);
		}
		else if (swipeDirection == SwipeDirection.Left)
		{
			aPlayer["strafe_left"].speed = 1;
			aPlayer.Play("strafe_left");
			hEnemyController.playEnemyAnimation(EnemyAnimation.strafe_left);
		}
		
		(aPlayer.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers) )
			.speed = fRunAnimationSpeed;
		hEnemyController.playEnemyAnimation(EnemyAnimation.run);
		
		previousLane = currentLane;//keep record of previous lane in case of stumble
		if (direction == SwipeDirection.Right && currentLane != 1)
		{
			currentLane ++;
			hCameraController.changeLane(currentLane);
		}
		else if (direction == SwipeDirection.Left && currentLane != -1)
		{
			currentLane --;
			hCameraController.changeLane(currentLane);
		}
	}
	
	private IEnumerator turnPlayerOnNextMidNode(SwipeDirection direction)
	{		
		ControlsEnabled = false;
		if ( 
			(direction == SwipeDirection.Right && turnPatch.patchType == PatchTypes.left)//right swipe on a right turn?
			|| (direction == SwipeDirection.Left && turnPatch.patchType == PatchTypes.right) //left swipe on a left turn?
			|| (direction == SwipeDirection.Left && turnPatch.patchType == PatchTypes.TRight)//left swipe on right T?
			|| (direction == SwipeDirection.Right && turnPatch.patchType == PatchTypes.TLeft)//right swipe on left T?
			)
		{
			changeLane(direction);
		}
		else//make the player turn
		{			
			if (turnPatch.patchType == PatchTypes.tee)//option of straight and turn
			{
				//tell patch controller about user decision
				if (direction == SwipeDirection.Right)
					hPatchController.makeDecision(1);
				else if (direction == SwipeDirection.Left)				
					hPatchController.makeDecision(2);
			}
			else if (turnPatch.patchType == PatchTypes.TLeft)//if left branch
			{				
				hPatchController.makeDecision(2);
			}
			else if (turnPatch.patchType == PatchTypes.TRight)//if right branch
			{
				hPatchController.makeDecision(2);
			}
			nextPatch.midNode = hPatchController.getnextPatch().midNode;//get the updated mid node
			
			while (true)
			{//print(MathCustom.VectorDistanceXZ(tPlayer.position, turnPatchMidNode.position));
				yield return new WaitForFixedUpdate();
				
				if (MathCustom.VectorDistanceXZ(tPlayer.position, turnPatch.midNode.position) <= fTurnRotateThreshold )//in range?
				{
					StartCoroutine(rotatePlayer(direction));//make the player face towards the new horizon
					updateForwardUnitVector();	//update direction
					break;
				}
			}//end of while
		}
		
		turnPatch = null;
		//enteredTurnRadius = false;
		ControlsEnabled = true;
		StopCoroutine("turnPlayer");//stop current routine*/
	}//end of turn player on next mid node coroutine
			
	private IEnumerator rotatePlayer(SwipeDirection direction)
	{
		Quaternion newRoation = Quaternion.identity;
				
		if (direction == SwipeDirection.Right)
			newRoation = Quaternion.Euler(0, 90, 0) * tPlayer.rotation;//right turn
		else if (direction == SwipeDirection.Left)
			newRoation = Quaternion.Euler(0, -90, 0) * tPlayer.rotation;//left turn
		
		//play turn animation
		if (direction == SwipeDirection.Right)
		{
			aPlayer["strafe_right"].speed = 0.5f;
			aPlayer.Play("strafe_right");
		}
		else if (direction == SwipeDirection.Left)
		{
			aPlayer["strafe_left"].speed = 0.5f;
			aPlayer.Play("strafe_left");
		}
		
		(aPlayer.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers) )
			.speed = fRunAnimationSpeed;
		
		while (true)
		{
			yield return new WaitForEndOfFrame();
						
			tPlayer.rotation = Quaternion.Slerp(tPlayer.rotation, newRoation, Time.deltaTime*10);
			
			if (tPlayer.rotation == newRoation)//escape condition
			{	
				tPlayer.rotation = newRoation;
				break;
			}
			
		}//end of while
		
		StopCoroutine("rotatePlayer");
		StartCoroutine(hEnemyController.rotateEnemy(direction));
	}
	
	/*
	*	FUNCTION: Turn player animations On or Off
	*/
	public void togglePlayerAnimation(bool bValue) 
	{ 
		if (aPlayer.enabled != bValue)
			aPlayer.enabled = bValue;
	}
	
	/// <summary>
	/// Checks if the player is in jump state.
	/// </summary>
	/// <returns>
	/// true or false
	/// </returns>
	public bool isInJump() 
	{
		if (JumpState > 0)
			return true;		
		else		
			return false;		
	}
	
	public bool isInDuck()
	{
		if (DuckState > 0)
			return true;
		else
			return false;
	}
	
	public Vector3 getCurrentForwardUnitVector() { return forwardUnitVector; }
}