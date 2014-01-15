using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float laneWidth = 1;
	public float laneSwitchAngle = 7;
	public float laneSwitchSpedd = 2;

	private Transform camPoint;
	private Transform cameraNode;
	private int currentLane = 0;
	private float globleLerp = 1f;

	private Vector3 cameraNodeStartPos;
	private Quaternion cameraNodeStartRot;
	
	void Start () 
	{
		cameraNode = GameObject.Find("Player/CameraNode").transform;
		camPoint = GameObject.Find("CamPoint").transform;
		camPointRot = cameraNode.rotation.eulerAngles;
		cameraNodeStartPos = cameraNode.localPosition;
		cameraNodeStartRot = cameraNode.rotation;
	}	
	
	public void Restart()
	{
		cameraNode.localPosition = cameraNodeStartPos;
		cameraNode.localRotation = cameraNodeStartRot;

		transform.position = camPoint.position;
		transform.rotation = camPoint.rotation;
	}
	
	float sineAngleVal = 0;
	float sineLerpVal = 0;
	Vector3 camPointRot ;

	void LateUpdate () 
	{
		transform.position = camPoint.position;
		transform.rotation = camPoint.rotation;

//			if (Input.GetKeyUp(KeyCode.D))
//			{
//				currentLane++;
//				StartCoroutine("laneSwitched");
//			}
//			if (Input.GetKeyUp(KeyCode.A))
//			{
//				currentLane--;
//				StartCoroutine("laneSwitched");
//			}

	}

	public void changeLane(int newLane)
	{
		
		StopCoroutine("laneSwitched");
		//print(currentLane);
		currentLane = newLane;
		globleLerp = 0;
		StartCoroutine("laneSwitched");
	}

	IEnumerator laneSwitched()
	{
		//print(cameraNode);
		//print ("start");
		cameraNode = GameObject.Find("Player/CameraNode").transform;
		int lane = currentLane;
		Vector3 v1 = cameraNode.localPosition;
		Vector3 v2;
		Quaternion q1 = cameraNode.localRotation;
		v2 = q1.eulerAngles;

		v1.x = lane*laneWidth;
		v2.y = lane*laneSwitchAngle;
		//globleLerp = 0;

		while(true)
		{
			yield return new WaitForFixedUpdate();
			globleLerp += Time.deltaTime*2;
			if (globleLerp<=1)
			{
				cameraNode.localPosition = Vector3.Lerp(cameraNode.localPosition,v1,globleLerp);
				cameraNode.localRotation = Quaternion.Lerp(q1,Quaternion.Euler(v2),globleLerp);
			}
			else 
			{
				cameraNode.localPosition = v1;
				cameraNode.localRotation = Quaternion.Euler(v2);
				break;
			}
		}
		//print ("stop");
		StopCoroutine("laneSwitched");
	}

}
