using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	private Transform tPlayer;
	
	void Start () 
	{
		tPlayer = GameObject.Find("Sphere").transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = tPlayer.position;
	}
}
