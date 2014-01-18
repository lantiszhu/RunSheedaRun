using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public enum ObstacleDificulty{Easy, Medium, Hard};
public enum SpawnType{SelectedObsticle , RandomObsticleSameType};
public enum LanesCovered{One , Two , Three};
public enum ObstacleType{JumpOnly , SlideOnly , JumpAndSlide , StrafeOnly};

public class ObstacleProperties : MonoBehaviour {

	// Use this for initialization

	#region Public
	public int spoolSize = 1;
	public ObstacleDificulty obstacleDificulty;
	public LanesCovered lanesCovered;
	public ObstacleType obstacleType;

	public SpawnType spawnType;
	#endregion

}
