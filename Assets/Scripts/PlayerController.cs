using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	#region Constants	
	private const float fStartForwardSpeed = 5.0f;	//player's speed when game starts
	private const float fLanePositionThreshold = 0.5f;//distance from the center
	private const float fLaneSwitchSpeed = 5.0f;//how fast to strafe
	
	private const float fGravity = 100.0f;	//the value with which to pull down the player
	private const float fJumpForce = 40;
	private const float fVerticalAccleration = 2;
	private const float fDuckDuration = 0.8f;
	private const float fDuckColliderScaleDownFactor = 2;
	private const float fDuckColliderTranslationFactor = 0.1f;
		
	private const float fTurnSwipeThreshold = 4.5f;//how close to the mid node the player should turn
	private const float fTurnRotateThreshold = 0.1f;//when on rotate on axis
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
	private Transform currentMidNode;	//transform of the current mid node
	private Transform nextMidNode;		//transform of the next mid node
		
	private float fCurrentForwardSpeed;
	private float fRunAnimationSpeed;
	private Vector3 forwardUnitVector;//direction of player
	
	private int currentLane;// -1, 0 or 1
	private float currentHorizontalPosition;//position when lanes are switched
	private float fVerticalPosition;
	private float fRayContactPosition;
	
	private int JumpState;
	private int DuckState;
	private float fDuckStartTime;
	
	private int PatchProgressionState;//when to get the next mid node	
	private bool bGroundHit;//if there is terrain under the player
	private RaycastHit hitInfo;
	#endregion
	
	//script references
	private PatchController hPatchController;
	private SwipeControls hSwipeControls;
	private InGameController hInGameController;
	
	void Start () 
	{
		hPatchController = (PatchController)GameObject.Find("Player").GetComponent(typeof(PatchController));
		hSwipeControls = (SwipeControls)this.GetComponent(typeof(SwipeControls));
		hInGameController = (InGameController)this.GetComponent(typeof(InGameController));
		
		tPlayer = this.transform;
		aPlayer = (Animation)this.transform.Find("CharacterGroup/sheeda").GetComponent(typeof(Animation));
		tCharacter = this.transform.Find("CharacterGroup").transform;
		tShadow = this.transform.Find("CharacterGroup/Shadow");
		tPrimaryCollider = this.transform.Find("CharacterGroup/Colliders/PrimaryCollider");
		tSecondaryCollider = this.transform.Find("CharacterGroup/Colliders/SecondaryCollider");
				
		currentLane = 0;		
		fCurrentForwardSpeed = fStartForwardSpeed;
		fRunAnimationSpeed = 1.0f;//run animation's speed
		fVerticalPosition = 0;
		fRayContactPosition = 0;
		
		PatchProgressionState = 0;
		JumpState = 0;
		DuckState = 0;
		
		//get patch information
		currentPatch = hPatchController.getCurrentPatch();
		nextPatch = hPatchController.getnextPatch();
		currentMidNode = hPatchController.getCurrentPatchMidNode();		
		nextMidNode = hPatchController.getNextPatchMidNode();//extract mid node position
		
		//calculate the direction in which the player has to move
		forwardUnitVector = (nextMidNode.position- new Vector3(0,0,0))/
			MathCustom.VectorDistanceXZ(nextMidNode.position,new Vector3(0,0,0));
		
		togglePlayerAnimation(false);//disable animation
	}
	
	public void Restart()
	{
		currentLane = 0;		
		fCurrentForwardSpeed = fStartForwardSpeed;
		fRunAnimationSpeed = 1.0f;//run animation's speed
		fVerticalPosition = 0;
		fRayContactPosition = 0;
		
		PatchProgressionState = 0;
		JumpState = 0;
		DuckState = 0;
		
		//get patch information
		currentPatch = hPatchController.getCurrentPatch();
		nextPatch = hPatchController.getnextPatch();
		currentMidNode = hPatchController.getCurrentPatchMidNode();		
		nextMidNode = hPatchController.getNextPatchMidNode();//extract mid node position
		
		//calculate the direction in which the player has to move
		forwardUnitVector = (nextMidNode.position- new Vector3(0,0,0))/
			MathCustom.VectorDistanceXZ(nextMidNode.position,new Vector3(0,0,0));
		
		togglePlayerAnimation(false);//disable animation
	}
	
	/*
	*	FUNCTION: Enable controls, start player animation and movement
	*/
	public void launchGame()
	{		
		togglePlayerAnimation(true);//enable animations
		aPlayer["run"].speed = fRunAnimationSpeed;
		aPlayer.Play("run");	//play run animation
	}
	
	void FixedUpdate () 
	{
		if (hInGameController.isGamePaused())
			return;
		
		setVerticalPosition();
		setForwardPosition();
				
		setHorizontalPosition();
		handlerSwipes();
		
		//get next patch's mid node when user reaches a mid node
		if (PatchProgressionState == 0)//regular case
		{
			if (MathCustom.VectorDistanceXZ(tPlayer.position,nextMidNode.position) <= 1)
			{
				hPatchController.updatePatch();
				updateNextMidNode();
			}
		}
		else if (PatchProgressionState == 1)//if user attempts to turn
		{
			
		}
		
	}//end of fixed update
	
	private void updateNextMidNode()
	{
		currentPatch = nextPatch;
		nextPatch = hPatchController.getnextPatch();
		
		currentMidNode = nextMidNode;//make record of previous mid node		
		nextMidNode = hPatchController.getNextPatchMidNode();//get next patch info
		
		//update direction if the next patch is straight ahead
		// the turnPlayer functions call this function manually on turns
		if (MathCustom.AngleDir(currentMidNode.forward, nextMidNode.position, currentMidNode.up) == 0)
			updateForwardUnitVector();
	}
	
	public void routineGameOver()
	{
		aPlayer.Play("stumble");
		//togglePlayerAnimation(false);
	}
	
	/*
	 * FUNCTION:	Control the user's forward movement.
	 * */
	private void setVerticalPosition()
	{
		bGroundHit = Physics.Linecast(tPlayer.position + new Vector3(0,5,0), tPlayer.position + new Vector3(0,-10,0), out hitInfo, (1<<8));
				
		if (bGroundHit)
		{
			fRayContactPosition = hitInfo.transform.position.y  + 0.02f;
			//set the position of a shadow
			tShadow.position = new Vector3(tCharacter.position.x, hitInfo.point.y + 0.01f, tCharacter.position.z);
		}
		else
			fRayContactPosition -= (fGravity*Time.deltaTime);
		
		if (JumpState == 0)//on ground
			fVerticalPosition = fRayContactPosition;
		else if (JumpState == 1)//jump triggered
		{
			aPlayer["jump"].speed = 1.5f;
			aPlayer.Play("jump");
			
			fVerticalPosition += fJumpForce;			
			JumpState = 2;
		}
		else if (JumpState == 2)//in air
		{
			fVerticalPosition -= (fGravity*Time.deltaTime);
			
			if (fVerticalPosition <= fRayContactPosition)//reached the ground
			{
				aPlayer["run"].speed = fRunAnimationSpeed;
				aPlayer.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers);
				JumpState = 0;
			}
		}
		
		tPlayer.position = new Vector3(tPlayer.position.x,
			MathCustom.LerpLinear(tPlayer.position.y, fVerticalPosition, Time.deltaTime*fVerticalAccleration),
			tPlayer.position.z);		
	}//end of set Vertical Position function
		
	private void setForwardPosition()
	{		
		tPlayer.position += forwardUnitVector * Time.deltaTime * fCurrentForwardSpeed;		
	}//end of set Forward Position function
		
	private void updateForwardUnitVector()
	{
		tPlayer.position = currentMidNode.position;
		forwardUnitVector = (nextMidNode.position-currentMidNode.position)/
			MathCustom.VectorDistanceXZ(nextMidNode.position,currentMidNode.position);
	}
		
	private void setHorizontalPosition()
	{
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
		
	}//end of set Horizontal Position
		
	/// <summary>
	/// Handlers the swipes.
	/// </summary>
	SwipeDirection swipeDirection;
	private void handlerSwipes()
	{//print(MathCustom.VectorDistanceXZ(tPlayer.position, nextMidNode.position));		
		swipeDirection = hSwipeControls.getSwipeDirection();
		
		//handle strafes or turns
		if (swipeDirection == SwipeDirection.Right || swipeDirection == SwipeDirection.Left)
		{	
			if (MathCustom.VectorDistanceXZ(tPlayer.position, nextMidNode.position) <= fTurnSwipeThreshold)
				StartCoroutine(turnPlayerOnNextMidNode(swipeDirection));
			else if (MathCustom.VectorDistanceXZ(tPlayer.position, currentMidNode.position) <= fTurnSwipeThreshold)
				turnPlayerOnCurrentMidNode(swipeDirection);
			else
				changeLane(swipeDirection);
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
					aPlayer.Play("roll");
				
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
				aPlayer.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers);//play run animation
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
		}
		else if (swipeDirection == SwipeDirection.Left)
		{
			aPlayer["strafe_left"].speed = 1;
			aPlayer.Play("strafe_left");
		}
		aPlayer["run"].speed = fRunAnimationSpeed;
		aPlayer.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers);		
		
		if (direction == SwipeDirection.Right && currentLane != 1)
			currentLane ++;
		else if (direction == SwipeDirection.Left && currentLane != -1)
			currentLane --;
	}
	
	private IEnumerator turnPlayerOnNextMidNode(SwipeDirection direction)
	{
		PatchProgressionState = 1;//stop the default patch update sequence
		
		hPatchController.updatePatch();
		updateNextMidNode();
		
		//float turnState = MathCustom.AngleDir(currentMidNode.forward, nextMidNode.position, currentMidNode.up);
		
		if (nextPatch.patchType == PatchTypes.straight)//if the next patch is straight
		{
			changeLane(direction);
			
			while (true)
			{
				yield return new WaitForFixedUpdate();
				
				if (MathCustom.VectorDistanceXZ(tPlayer.position, currentMidNode.position) <= fTurnRotateThreshold)
					break;				
			}//end of while
		}
		else //if the next patch is a turn
		{
			if ( (direction == SwipeDirection.Right && nextPatch.patchType != PatchTypes.right)//right swipe on a right turn?
				|| (direction == SwipeDirection.Left && nextPatch.patchType != PatchTypes.left) )//left swipe on a left turn?			
				changeLane(direction);			
			else
			{
				while (true)
				{
					yield return new WaitForFixedUpdate();
										
					if (MathCustom.VectorDistanceXZ(tPlayer.position, currentMidNode.position) <= fTurnRotateThreshold )//in range?
					{
						StartCoroutine(rotatePlayer(direction));//make the player face towards the new horizon
						updateForwardUnitVector();	//update direction
						currentLane = 0;	//switch to mid lane on rotation
						
						break;
					}
				}//end of while
			}//end of else	
			
		}//end of outer else
		
		PatchProgressionState = 0;//set the patch update squence to default
		StopCoroutine("turnPlayer");//stop current routine
	}//end of turn player on next mid node coroutine
	
	private void turnPlayerOnCurrentMidNode(SwipeDirection direction)
	{
		//float turnState = MathCustom.AngleDir(currentMidNode.forward, nextMidNode.position, currentMidNode.up);
		
		if (nextPatch.patchType == PatchTypes.straight)//if the next patch is straight		
			changeLane(direction);		
		else
		{
			if ( (direction == SwipeDirection.Right && nextPatch.patchType != PatchTypes.right)//right swipe on a right turn?
				|| (direction == SwipeDirection.Left && nextPatch.patchType != PatchTypes.left) )//left swipe on a left turn?			
				changeLane(direction);			
			else//turn the character
			{
				//TODO: decition PatchController
				//TODO: get Next Mid Node again
				StartCoroutine(rotatePlayer(direction));//make the player face towards the new horizon
				updateForwardUnitVector();	//update direction
				currentLane = 0;	//switch to mid lane on rotation
			}
			
		}//end of outer else
	}//end of turn player on current mid node function
	
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
		aPlayer["run"].speed = fRunAnimationSpeed;
		aPlayer.CrossFadeQueued("run", 0.5f, QueueMode.CompleteOthers);
		
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
	}
	
	/*
	*	FUNCTION: Turn player animations On or Off
	*/
	public void togglePlayerAnimation(bool bValue) { aPlayer.enabled = bValue; }
	
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
}