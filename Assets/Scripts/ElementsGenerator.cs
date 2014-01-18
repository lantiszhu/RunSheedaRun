using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

public class ElementsGenerator : MonoBehaviour {

	public GameObject [] obstaclesPrefabs;
	public GameObject [] powerupPrefabs;
	public GameObject coinPrefab;
	// Use this for initialization
	private PrefabsHolder.PatternsManeger patternsManeger;
	private string path = "Assets/Editor/LevelPattern.xml";

	List<GameObject>[] obstacleList;
	int[] lastobstacleindex;
	List<GameObject> coinsList;
	int lastcoinindex = 0;
	LinkedList<Transform> CoinPositions;

	GameObject elementsMainGroup;


	void Start () 
	{
		elementsMainGroup = (GameObject)GameObject.Find("ElementsMainGroup");
		readPatterns();
		init(true);
	}
	void init(bool firstTime)
	{
		ObstacleProperties obstacleProperties;
		PowerupElement elementProperties;
		CoinPositions = new LinkedList<Transform>();
		if (firstTime)
		{
			obstacleList = new List<GameObject>[obstaclesPrefabs.Length];
			lastobstacleindex = new int[obstaclesPrefabs.Length];
			GameObject tempGo;
			for (int i = 0; i < lastobstacleindex.Length; i++) {
				lastobstacleindex[i] = 0;
			}
			int j = 0;
			foreach (GameObject obstacle in obstaclesPrefabs) 
			{
				obstacleList[j] = new List<GameObject>();
				obstacleProperties = obstacle.GetComponent<ObstacleProperties>();
				for (int i = 0; i < obstacleProperties.spoolSize; i++) 
				{
					tempGo = (GameObject)Instantiate(obstacle);
					tempGo.transform.parent = elementsMainGroup.transform;
					tempGo.name = obstacle.name;
					tempGo.transform.position = new Vector3(0,1000,0);
					obstacleList[j].Add(tempGo);
				}
				j++;
			}

			coinsList = new List<GameObject>();
			elementProperties = coinPrefab.GetComponent<PowerupElement>();
			for (int i = 0; i < elementProperties.spoolSize; i++) 
			{
				tempGo = (GameObject)Instantiate(coinPrefab);
				tempGo.transform.parent = elementsMainGroup.transform;
				tempGo.name = coinPrefab.name;
				tempGo.transform.position = new Vector3(0,1000,0);
				coinsList.Add(tempGo);
			}
		}
	}
	void readPatterns()
	{
		patternsManeger = new PrefabsHolder.PatternsManeger();
		if (System.IO.File.Exists(path))
		{
			XmlSerializer reader = new XmlSerializer(typeof(PrefabsHolder.PatternsManeger));
			System.IO.StreamReader readStream = new System.IO.StreamReader(path);
			patternsManeger = reader.Deserialize(readStream) as PrefabsHolder.PatternsManeger ;
			readStream.Close();
			Debug.Log("ManagerLoaded");
		}
		else
		{
			Debug.Log("File Not Found");
		}
	}
	int count = 0 ;
	public void addObstacles(Transform patch,PatchTypes pTyps)
	{
		Debug.Log(count + " => " + patternsManeger);


		if (patternsManeger!=null)
		{
			//Debug.Log(count);
			PrefabsHolder.Pattern pattern = patternsManeger.getPattren(pTyps);
			if (pattern !=null)
			{
			Transform obsticleGroup = patch.transform.Find("Obstacles").transform;
			Transform elementGroup = patch.transform.Find("Elements").transform;
			Transform temp;
			GameObject go;
			int i = 0;
			Debug.Log( "Pattern Count =="+pattern.obstacleList.Length);
			foreach (PrefabsHolder.Obstacle obs in pattern.obstacleList)
			{
				for (i = 0; i < obstaclesPrefabs.Length; i++) 
				{
					if (obstaclesPrefabs[i].name == obs.name)
					{
						//Debug.Log(obstaclesPrefabs[i].name + "" +obstacleList[i].Count);
						go =  obstacleList[i][lastobstacleindex[i]];
						lastobstacleindex[i] = (lastobstacleindex[i]+1)%obstacleList[i].Count;
						if (go != null)
						{
							temp = go.transform;
							temp.parent = obsticleGroup;
							temp.name = obs.name;
							temp.localPosition = obs.position;
							temp.localRotation = Quaternion.Euler(obs.rotation);
							temp.localScale = obs.scale;
							//temp.parent = elementsMainGroup.transform;
						}
						else
						{
							Debug.LogWarning("Go - null");
						}
						//break;
					}
					
				} 
			}
			print ("Coins Test" + pattern.currencyList.Length);
			foreach (PrefabsHolder.Currency c in pattern.currencyList) 
			{
				//print ("Coins Test");
					temp = (new GameObject()).transform;
					temp.parent = elementGroup;
					temp.name = c.name;
					temp.localPosition = c.position;
					temp.localRotation = Quaternion.Euler(c.rotation);
					temp.localScale = c.scale;
					CoinPositions.AddFirst(temp);
			}
			print (CoinPositions.Count);
			count++;
			}
		}
	}

	public void updateCoinPos(Transform coin)
	{
		print ("CoinCount = "+CoinPositions.Count);
		if(CoinPositions.Count>0)
		{
			Transform temp = CoinPositions.Last.Value;
			coin.parent = temp.parent;
			coin.name = temp.name;
			coin.localPosition = temp.localPosition;
			coin.localRotation = temp.localRotation;
			coin.localScale = temp.localScale;
			CoinPositions.RemoveLast();
		}
		else
		{
			coin.parent = elementsMainGroup.transform;
			coin.localPosition = new Vector3(0,1000,0);
			//coin.name = "Dead Coin";
		}
	}
	public void InitCoins()
	{
		int i = 0;
		while (CoinPositions.Last!=null && i<coinsList.Count)
		{
			updateCoinPos(coinsList[i].transform);
			i++;
		}
			//CoinPositions
	}

}
