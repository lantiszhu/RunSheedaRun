using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabsHolder	 : MonoBehaviour 
{

	public List<GameObject> straightPatchList;
	public List<GameObject> leftTurnPatch;
	public List<GameObject>	rightTurnPatch;
	public List<GameObject> teePatchList;
	public List<GameObject> TLeftPatchList;
	public List<GameObject> TRightPatchList;

	public List<GameObject> obsticles;

	public List<GameObject> elements;

	public void setupPatch(Pattern pattern)
	{

		List <GameObject> selected  = getprefabList(pattern.patchType);
		int rand = Random.Range(0,selected.Count);
		GameObject patch = selected[0]; //= GameObject.Instantiate(straightPatchList[rand]) as GameObject;

		foreach (GameObject p in selected) 
			{
				if (pattern.patchName == p.name)
				{
					patch = GameObject.Instantiate(p) as GameObject;
					patch.name = p.name;
					break;
				}
			}
			if (patch==null)
			{
			patch =GameObject.Instantiate(selected[rand]) as GameObject;
			patch.name = selected[rand].name;
			}

			Transform obsticleGroup = patch.transform.Find("Obstacles").transform;
			Transform elementGroup = patch.transform.Find("Elements").transform;
			Transform temp;

			foreach (Obstacle obs in pattern.obstacleList)
			{
				for (int i = 0; i < obsticles.Count; i++) 
				{
					if (obsticles[i].name == obs.name)
					{
						Debug.Log(obsticles[i].name );
						temp = ((GameObject)GameObject.Instantiate(obsticles[i])).transform;
						temp.parent = obsticleGroup;
						temp.name = obs.name;
						temp.localPosition = obs.position;
						temp.localRotation = Quaternion.Euler(obs.rotation);
						temp.localScale = obs.scale;
					}

				} 
			}
			foreach (Currency cur in pattern.currencyList)
			{
				for (int i = 0; i < elements.Count; i++) 
				{
					if (elements[i].name == cur.name)
					{
						Debug.Log(obsticles[i].name );
						temp = ((GameObject)GameObject.Instantiate(elements[i])).transform;
						temp.parent = elementGroup;
						temp.name = cur.name;
						temp.localPosition = cur.position;
						temp.localRotation = Quaternion.Euler(cur.rotation);
						temp.localScale = cur.scale;
					}
					
				} 
			}

 	}
	List<GameObject> getprefabList (PatchTypes pType)
	{ 
		if (pType == PatchTypes.straight)
		{
			return straightPatchList;
		}
		else if (pType == PatchTypes.left)
		{
			return leftTurnPatch;		
		}
		else if (pType == PatchTypes.right)
		{
			return rightTurnPatch;				
		}
		else if (pType== PatchTypes.tee)
		{
			return teePatchList;
		}
		else if (pType == PatchTypes.TLeft)
		{
			return TLeftPatchList;
		}
		else if (pType == PatchTypes.TRight)
		{
			return TRightPatchList;
		}
		return null;
	}
	public class PatternsManeger
	{
		public List<Pattern> straightPattern;
		public List<Pattern> leftPattern;
		public List<Pattern> rightPattern;
		public List<Pattern> TPattern;
		public List<Pattern> TLeftPattern;
		public List<Pattern> TRightPattern;
		
		public int patternCount = 0;
		public int lastID = 0;
		
		public PatternsManeger()
		{
			straightPattern = new List<Pattern>();
			leftPattern = new List<Pattern>();
			rightPattern = new List<Pattern>();
			TPattern = new List<Pattern>();
			TLeftPattern = new List<Pattern>();
			TRightPattern = new List<Pattern>();
		}
		public void AddPattern(Transform ObstacleGroup)
		{
			patternCount++;
			Pattern pattern = new Pattern(ObstacleGroup , lastID++);
			if (pattern.patchType == PatchTypes.straight)
			{
				straightPattern.Add(pattern);
			}
			else if (pattern.patchType == PatchTypes.left)
			{
				leftPattern.Add(pattern);		
			}
			else if (pattern.patchType == PatchTypes.right)
			{
				rightPattern.Add(pattern);				
			}
			else if (pattern.patchType == PatchTypes.tee)
			{
				TPattern.Add(pattern);
			}
			else if (pattern.patchType == PatchTypes.TLeft)
			{
				TLeftPattern.Add(pattern);
			}
			else if (pattern.patchType == PatchTypes.TRight)
			{
				TRightPattern.Add(pattern);
			}
		}
		List <Pattern> getPatternListFromType(string ID)
		{
			for (int i = 0; i < 6; i++) 
			{
				List <Pattern> selectedPatternList = getPatternListFromType((PatchTypes)i);
				foreach (Pattern p in selectedPatternList) 
				{
					if (p.Id == ID)
					{
						return selectedPatternList;
					}
				}
				
			}
			return null;
		}
		List <Pattern> getPatternListFromType(PatchTypes pType)
		{
			if (pType == PatchTypes.straight)
			{
				return straightPattern;
			}
			else if (pType == PatchTypes.left)
			{
				return leftPattern;		
			}
			else if (pType == PatchTypes.right)
			{
				return rightPattern;				
			}
			else if (pType== PatchTypes.tee)
			{
				return TPattern;
			}
			else if (pType == PatchTypes.TLeft)
			{
				return TLeftPattern;
			}
			else if (pType == PatchTypes.TRight)
			{
				return TRightPattern;
			}
			return null;
		}
		public Pattern getPattren(string ID)
		{
			for (int i = 0; i < 6; i++) 
			{
				List <Pattern> selectedPatternList = getPatternListFromType((PatchTypes)i);
				foreach (Pattern p in selectedPatternList) 
				{
					if (p.Id == ID)
					{
						return p;
					}
				}
			}
			return null;
		}
		public Pattern getPattren(PatchTypes pTypes)
		{
			List <Pattern> selectedPatternList = getPatternListFromType(pTypes);
			if (selectedPatternList.Count != 0)
			{
				return selectedPatternList[Random.Range(0,selectedPatternList.Count-1)];
			}
			else return null;

		}
		public Pattern getPattren(PatchTypes pTypes , int Index)
		{
			List <Pattern> selectedPatternList = getPatternListFromType(pTypes);
			return selectedPatternList[Random.Range(1,selectedPatternList.Count)];
		}
		public int getPattrenIndex(string ID)
		{
			int j=0;
			for (int i = 0; i < 6; i++) 
			{
				List <Pattern> selectedPatternList = getPatternListFromType((PatchTypes)i);
				foreach (Pattern p in selectedPatternList) 
				{
					if (p.Id == ID)
					{
						return j;
					}
					j++;	
				}
			}
			return -1;
		}
		public void removeAt(int index, PatchTypes pType)
		{
			List <Pattern> selectedPatternList = getPatternListFromType(pType);
			selectedPatternList.RemoveAt(index);
			patternCount--;
		}
		public void removePatternID(string ID)
		{
			List <Pattern> selectedPatternList = getPatternListFromType(ID);

			selectedPatternList.RemoveAt(getPattrenIndex(ID));
			patternCount--;
		}
		
	}
	public class Pattern
	{
		public PatchTypes patchType;
		public string patchName;
		public int obstacleCount;
		public int elementsCount;
		public Obstacle[] obstacleList;
		public Currency[] currencyList;
		public string Id;
		
		public Pattern()
		{
			
		}
		public Pattern(Transform TempT , int ID)
		{
			Transform obstacleGroup;
			Transform elementsGroup;
			
			if (TempT.Find("Obstacles"))
				obstacleGroup = TempT.Find("Obstacles");
			else
			{
				TempT = TempT.parent;
				obstacleGroup = TempT.Find("Obstacles");
			}
			
			if (TempT.Find("Elements"))
				elementsGroup = TempT.Find("Elements");
			else
			{
				TempT = TempT.parent;
				elementsGroup = TempT.Find("Elements");
			}
			
			obstacleCount = obstacleGroup.childCount;
			elementsCount = elementsGroup.childCount;
			
			patchType = TempT.GetComponent<PatchProperties>().patchType;
			patchName = TempT.name;
			
			obstacleList = new Obstacle[obstacleCount];
			currencyList = new Currency[elementsCount];
			int i = 0;
			
			foreach (Transform t in obstacleGroup)
			{
				obstacleList[i] = new Obstacle(t);
				i++;
			}
			i = 0;
			foreach (Transform t in elementsGroup)
			{
				currencyList[i] = new Currency(t);
				i++;
			}
			Id = ""+patchType.ToString() + "_" + ID;
		}
	}
	public class Currency
	{
		public string name;
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;
		//public ElementProperties elementProperties;
		public Powerups elementType;
		
		public Currency()
		{
			
		}
		public Currency(Transform currency)
		{
			name = currency.name;
			position = currency.localPosition;
			rotation = currency.localEulerAngles;
			scale = currency.localScale;
			PowerupElement elementProperties = currency.GetComponent<PowerupElement>();
			elementType = elementProperties.powerupType;
		}
	}
	public class Obstacle
	{
		public string name;
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;
		
		public SpawnType spawnType;
		public ObstacleDificulty obstacleDificulty;
		public LanesCovered lanesCovered;
		public ObstacleType obstacleType;
		
		public Obstacle()
		{
			
		}
		public Obstacle(Transform obstacle)
		{
			name = obstacle.name;
			position = obstacle.localPosition;
			rotation = obstacle.localEulerAngles;
			scale = obstacle.localScale;
			ObstacleProperties obstacleProperties = obstacle.GetComponent<ObstacleProperties>();
			spawnType = obstacleProperties.spawnType;
			obstacleDificulty = obstacleProperties.obstacleDificulty;
			lanesCovered = obstacleProperties.lanesCovered;
			obstacleType = obstacleProperties.obstacleType;
		}
	}

}
