using UnityEngine;
using System.Collections;

public class MyCharacterController : MonoBehaviour {

	Player player;
	void Start () {
		player = new Player(transform);
	}

	void Update () {
		player.run(Time.deltaTime*10);

		if (Input.GetKeyUp(KeyCode.A))
		{
			StartCoroutine("turnPlayer",-1);
		}
		if (Input.GetKeyUp(KeyCode.D))
		{
			StartCoroutine("turnPlayer",1);
		}
	}
	IEnumerator turnPlayer(object d)
	{
		int dir = (int)d;
		print (player.setNewPlayerAngle(dir));
		print (Time.deltaTime);
		while (player.turn(dir,Time.deltaTime))
		{
			yield return new WaitForEndOfFrame();
		}
	}
}
class Player
{
	public Transform playerTransform;
	public Transform PlayerVisual;
	float playerAngle;
	float newAngle;
	int playerXDir;
	int playerZDir;
	
	PatchController patchController;
	
	Vector3 NodeA;
	Vector3 NodeB;
	Vector3 NodeC;
	
	public Player(Transform t)
	{
		playerTransform = t;
		PlayerVisual = t.Find("CharacterGroup");
		patchController = playerTransform.GetComponent<PatchController>();
		newAngle = 0;
		playerAngle = 0;
		playerXDir = 0;
		playerZDir = 1;
		NodeA = Vector3.zero;
		NodeB = patchController.getNextPatchMidNode().position;
		NodeC = patchController.getPatchList().Last.Previous.Previous.Value.midNode.position;
	}
	
	public string setNewPlayerAngle(int direction)
	{
		newAngle = playerAngle+direction* 90;
		if (playerXDir == 0)
		{
			if (playerZDir >0)
			{
				playerXDir = direction;
			}
			else
			{
				playerXDir = -direction;
			}
			playerZDir = 0;
		}
		else if (playerZDir == 0)
		{
			if (playerXDir >0)
			{
				playerZDir = -direction;
			}
			else
			{
				playerZDir = direction;
			}
			
			playerXDir = 0;
		}
		return ("applied = "+direction+" X= "+playerXDir+" Z= "+playerZDir);
	}
	
	public bool turn(int direction , float dtime)
	{
		playerAngle = Mathf.Lerp(playerAngle,newAngle,dtime);
		playerTransform.eulerAngles = new Vector3(0,playerAngle,0);
		
		if (playerAngle>=newAngle && direction<0)
		{
			playerAngle = newAngle;
			//playerTransform.eulerAngles = new Vector3(0,playerAngle,0);
			return true;
		}
		else if (playerAngle<=newAngle && direction>0)
		{
			playerAngle = newAngle;
			//playerTransform.eulerAngles = new Vector3(0,playerAngle,0);
			return true;
		}
		else 
			return false;
	}
	
	void updateNode()
	{
		NodeC = NodeB;
		NodeB = NodeA;
		NodeA = patchController.getNextPatchMidNode().position;
	}
	public void run(float dtime)
	{
		Vector3 playerPos = playerTransform.position;
		//playerTransform.Translate((dtime*playerXDir),0,(dtime*playerZDir));
		playerTransform.position = new Vector3(playerPos.x+(dtime*playerXDir),0,playerPos.z+(dtime*playerZDir));
		if (Vector3.Distance(playerPos,NodeB)<3)
		{
			updateNode();
		}
	}
}
