/// <summary>
/// Missions controller.
/// </summary>
using UnityEngine;
using System.Collections;

//the different types of missions
public enum MissionTypes
{
	Score,		
	Powerups,
	Jump,
	Duck,
	StandardCurrency,
	StartGame
}

public class MissionsController : MonoBehaviour {
	
	//detail of a particular mission
	public class MissionDetail
	{
		public string missionDescription;
		public int missionCount;
		public MissionTypes missionType;
	}
	
	void Start () 
	{
		
	}
		
	void Update () 
	{
		
	}
}
