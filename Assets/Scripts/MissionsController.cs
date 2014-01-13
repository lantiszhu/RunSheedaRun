using UnityEngine;
using System.Collections;

//the different types of missions
public enum MissionTypes
{
	Score,	
	Powerups,
	MagnetismPowerup,
	Jump,
	Duck,
	StandardCurrency,
	StartGame
}

//detail of a particular mission
public class MissionDetail
{
	public string description;
	public int totalCount;//total number of steps/ collections needed
	public int progressCount;
	public MissionTypes type;
}

public class MissionsController : MonoBehaviour {
	
	#region Constants
	private const int iActiveMissionCount = 3;//number of missions active at a time (3 by default)
	#endregion
	
	#region Variables
	private MissionDetail[] missions;//details of all missions	
	private int[] iActiveMissions;//keeps record of currently active missions
	private int iNextMission;//index of the next mission if a current one is completed
	private int iTotalMissionCount;//the total number available missions
	#endregion
	
	#region Script References
	private InGameController hInGameController;
	private MenuScript hMenuScript;
	private HUDController hHUDController;
	#endregion
		
	void Start () 
	{
		hInGameController = (InGameController)this.GetComponent(typeof(InGameController));
		hMenuScript = (MenuScript)GameObject.Find("GUIGroup/MenuGroup").GetComponent(typeof(MenuScript));
		hHUDController = (HUDController)GameObject.Find("GUIGroup/HUDGroup").GetComponent(typeof(HUDController));
						
		//set the next mission index
		if (PlayerPrefs.HasKey("NextMissionIndex"))
		{
			iNextMission = PlayerPrefs.GetInt("NextMissionIndex");
		}
		else
		{
			iNextMission = 0;
			PlayerPrefs.SetInt("NextMissionIndex", iNextMission);
		}
				
		//get the MissionList file from the resources folder
		TextAsset taFile = (TextAsset)Resources.Load("MissionsList");
		string[] lines = taFile.text.Split('\n');
		
		if (lines.Length == 0)//if the file was empty
		{
			Debug.Log("No missions found in file");
			this.enabled = false;
		}
		else//read file and extract mission detail
		{
			int lineIndex=0;
			int arrayIndex=0;
			iTotalMissionCount = lines.Length/3;
			missions = new MissionDetail[iTotalMissionCount];//allocate memory according to the number of missions
			for (int i=0; i<iTotalMissionCount; i++)
				missions[i] = new MissionDetail();
			
			while (lineIndex < lines.Length)//store the file content in mission array
			{
				missions[arrayIndex].description = lines[lineIndex++];
				missions[arrayIndex].totalCount = int.Parse(lines[lineIndex++]);
				missions[arrayIndex].type = (MissionTypes)System.Enum.Parse(typeof(MissionTypes), lines[lineIndex++]);
			
				arrayIndex++;
			}//end of while
					
			iActiveMissions = new int[iActiveMissionCount];
			for (int i=0; i<iActiveMissionCount; i++)//set the currently active missions
			{
				if (PlayerPrefs.HasKey("ActiveMission_"+i.ToString()))
				{
					iActiveMissions[i] = PlayerPrefs.GetInt("ActiveMission_"+i.ToString());//get the index of active missions
					missions[ iActiveMissions[i] ].progressCount = PlayerPrefs.GetInt("MissionProgress"+i.ToString());
				}
				else
				{				
					iActiveMissions[i] = getNextMission();
					PlayerPrefs.SetInt("ActiveMission_"+i.ToString(), iActiveMissions[i]);
					
					missions[ iActiveMissions[i] ].progressCount = 0;
					PlayerPrefs.SetInt("MissionProgress"+i.ToString(), missions[ iActiveMissions[i] ].progressCount);
				}
			}//end of for
			
			updateMenuDescriptions();//update mission description on mission and pause menu
			
		}//end of else
		
		PlayerPrefs.Save();
	}//end of start
	
	/// <summary>
	/// Increments the mission count.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	public void incrementMissionCount(MissionTypes type)
	{
		for (int i=0; i<iActiveMissionCount; i++)
		{
			if (missions[i].type == type)
			{
				missions[i].progressCount++;
				checkCompletion();
				updateMenuDescriptions();
				break;
			}
		}//end of for
	}
	/// <summary>
	/// Increments the mission count.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	/// <param name='iVal'>
	/// I value.
	/// </param>
	public void incrementMissionCount(MissionTypes type, int iVal)
	{
		for (int i=0; i<iActiveMissionCount; i++)
		{
			if (missions[i].type == type)
			{
				missions[i].progressCount += iVal;
				//permanently store mission progress
				PlayerPrefs.SetInt("MissionProgress"+i.ToString(), missions[i].progressCount);
				checkCompletion();
				updateMenuDescriptions();
				break;
			}
		}//end of for
	}
	
	/// <summary>
	/// Checks the completion of all active missions.
	/// </summary>
	private void checkCompletion()
	{
		for (int i = 0; i<iActiveMissionCount; i++)//check if an active misson has been completed
		{
			if (missions[ iActiveMissions[i] ].progressCount >= missions[ iActiveMissions[i] ].totalCount)
				markMissionComplete(i);
		}//end of for
	}
	
	public void updateMenuDescriptions()
	{
		string combinedText = string.Empty;
		
		//combine all the description text in one string
		for (int i=0; i<iActiveMissionCount; i++)
		{		
			combinedText += (i+1).ToString() +". " + missions[ iActiveMissions[i] ].description
			+ "\n (" + missions[ iActiveMissions[i] ].progressCount + "/" 
			+ missions[ iActiveMissions[i] ].totalCount + ")\n\n";
		}
		
		//tell the MenuScript.js to update the missions description on Pause Menu
		hMenuScript.updatePauseMenuMissions(combinedText);
		hMenuScript.updateMissionsMenuMissions(combinedText);
	}//end of update menu description function
	
	private void markMissionComplete(int missionIndex)
	{
		//announce mission completion on HUD
		//StartCoroutine(hHUDController.displayMissionDescriptionDropDown("DONE!\n" + missions[ iActiveMissions[missionIndex] ].missionDescription));
		
		//replace the completed mission with a new one
		iActiveMissions[missionIndex] = getNextMission();
		//reset the new active mission count
		PlayerPrefs.SetInt("MissionProgress"+missionIndex.ToString(), 0);
		
		//permenantly save the new active mission
		PlayerPrefs.SetInt("ActiveMission_"+missionIndex, iActiveMissions[missionIndex]);
			
		//update the mission decription on the pause menu and missions menu
		updateMenuDescriptions();
	}
	
	private int getNextMission()
	{
		int tempNext = iNextMission;
		
		if ( (iNextMission+1) == iTotalMissionCount)//if all missions completed, restart mission list
		{
			iNextMission = 0;
			PlayerPrefs.SetInt("NextMissionIndex", iNextMission);	
		}
		else//return next mission's index located in the 'missions' array
		{
			iNextMission++;
			PlayerPrefs.SetInt("NextMissionIndex", iNextMission);		
		}
				
		return tempNext;
	}
}
