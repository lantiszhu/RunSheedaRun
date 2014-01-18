using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PatchTypes{ straight , left , right , tee , TLeft , TRight};
public class PatchController : MonoBehaviour {
	
	public class Patch
	{
		public Transform patchTransform;
		public Transform startNode;
		public Transform midNode;
		public Transform endNode;
		public Transform endNode2;
		public PatchTypes patchType;
		public Transform obstacleGroup;
		
		public Patch(Transform patchT,Transform start,Transform mid, Transform end,PatchTypes type)
		{
			patchTransform = patchT ;
			startNode = start;
			midNode = mid;
			endNode = end;
			patchType = type;
		}

		public Patch(Transform patchT,PatchTypes type)
		{
			this.patchTransform = patchT;
			startNode = patchTransform.Find("Nodes/Start");
			midNode = patchTransform.Find("Nodes/Mid");
			endNode = patchTransform.Find("Nodes/End_1");
<<<<<<< HEAD
			obstacleGroup = patchTransform.Find("Obstacles");
=======

>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
			if(type==PatchTypes.tee || type==PatchTypes.TLeft || type==PatchTypes.TRight)
			{
				endNode2 = patchT.Find("Nodes/End_2");
			}

			patchType = type;
		}
	}
	public List<GameObject> straightPatchList;
	public List<GameObject> leftTurnPatch;
	public List<GameObject>	rightTurnPatch;
	public List<GameObject> teePatchList;
	public List<GameObject> TLeftPatchList;
	public List<GameObject> TRightPatchList;
	GameObject currentPatch;
	PatchTypes currentpatchType;
	GameObject PatchGroup;

	LinkedList<Patch> patchesList = new LinkedList<Patch>();
	int queueCapacity = 5;
	int straightPatchCount = 2;
	Transform startPatch;
	GameObject elementsMainGroup;

	ElementsGenerator hElementsGenerator;

	void Start () 
	{	
		hElementsGenerator = GameObject.Find("Controllers").GetComponent<ElementsGenerator>();
		elementsMainGroup = (GameObject)GameObject.Find("ElementsMainGroup");
		PatchGroup = new GameObject();
		PatchGroup.transform.name = "PatchGroup";
<<<<<<< HEAD
=======

>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
		init();
	}
	private void init()
	{
		if (!currentPatch)
		{
			//currentPatch = GameObject.Instantiate(straightPatchList[0],Vector3.zero,Quaternion.identity) as Transform;
			currentPatch = (GameObject)Instantiate(straightPatchList[0]);
			currentPatch.transform.parent = PatchGroup.transform;
			currentPatch.transform.position = new Vector3(0,0,-10);
<<<<<<< HEAD
			currentPatch.transform.name = "StartPatch";
			//GameObject.Destroy(currentPatch.transform.Find("Obstacles").gameObject);
=======
			GameObject.Destroy(currentPatch.transform.Find("Obstacles").gameObject);
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
			patchesList.AddFirst(new Patch(currentPatch.transform,PatchTypes.straight));
			currentpatchType = PatchTypes.straight;
			//hElementsGenerator.addObstacles(currentPatch.transform,currentpatchType);
		}
<<<<<<< HEAD

=======
		
		//tickTime = 0;
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
		straightPatchCount = Random.Range(2,4);
		for (int i = 0; i < queueCapacity-1; i++)
		{
			updatePatch();
		}
		hElementsGenerator.InitCoins();
	}

	private void clearAll()
	{
		do
		{
			Transform T = patchesList.Last.Value.patchTransform;
			lastTeeNode = null;
			GameObject.Destroy(T.gameObject);
			patchesList.RemoveLast();
		}while(patchesList.First!=patchesList.Last);
		currentPatch = null;
	}

	private void clearAll()
	{
		do
		{
			Transform T = patchesList.Last.Value.patchTransform;
			lastTeeNode = null;
			GameObject.Destroy(T.gameObject);
			patchesList.RemoveLast();
		}while(patchesList.First!=patchesList.Last);
		currentPatch = null;
	}
	public Patch getCurrentPatch()
	{
		return patchesList.Last.Value;
	}
	public Patch getnextPatch()
	{
		return patchesList.Last.Previous.Value;		
	}
	public LinkedList<Patch> getPatchList()
	{
		return patchesList;
	}

	/// <summary>
	/// Gets the current patch mide node.
	/// </summary>
	/// <returns>The current patch mide node.</returns>
	/// 
	public Transform getCurrentPatchMidNode()
	{
		return patchesList.Last.Value.midNode;
	}

	/// <summary>
	/// Gets the next patch mide node.
	/// </summary>
	/// <returns>The next patch mide node.</returns>
	public Transform getNextPatchMidNode()
	{
		return patchesList.Last.Previous.Value.midNode;
	}

	/// <summary>
	/// Gets the type of the next patch.
	/// </summary>
	/// <returns>The next patch type.</returns>
	public PatchTypes getNextPatchType()
	{
		return currentpatchType;
	}

	/// <summary>
	/// Gets the type of the nextpathc.
	/// </summary>
	/// <returns>The nextpathc type.</returns>
	/// 

	public void Restart()
	{
		clearAll();
		init();
	}
	PatchTypes selectNextpatchType()
	{
<<<<<<< HEAD
		return PatchTypes.straight;// Un-comment to just have straight patches.
=======
		//return PatchTypes.straight; Un-comment to just have straight patches.
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
		PatchTypes patchType;
		if (straightPatchCount>0)
		{
			patchType = PatchTypes.straight;
			straightPatchCount--;
		}
		else
		{
			straightPatchCount = Random.Range(2,4);
			patchType = (PatchTypes)Random.Range(1,6);
		}

		//Debug.Log("==>"+patchType.ToString());
		return patchType;
	}

	GameObject getNextPatch(PatchTypes patchType)
	{

		if (patchType == PatchTypes.straight)
		{
			return straightPatchList[Random.Range(0,straightPatchList.Count)];
		}
		else if (patchType == PatchTypes.left)
		{
			return leftTurnPatch[Random.Range(0,leftTurnPatch.Count)];			
		}
		else if (patchType == PatchTypes.right)
		{
			return rightTurnPatch[Random.Range(0,rightTurnPatch.Count)];				
		}
		else if (patchType == PatchTypes.tee)
		{
			return teePatchList[Random.Range(0,teePatchList.Count)];
		}
		else if (patchType == PatchTypes.TLeft)
		{
			return TLeftPatchList[Random.Range(0,TLeftPatchList.Count)];
		}
		else if (patchType == PatchTypes.TRight)
		{
			return TRightPatchList[Random.Range(0,TRightPatchList.Count)];
		}
		else
			return null;
	}
	LinkedListNode<Patch> lastTeeNode;

	public void makeDecision(int decision)
	{
		if (decision == 2 && lastTeeNode != null)
		{
			if(lastTeeNode.Previous !=null )
			{
				lastTeeNode.Previous.Value.patchTransform.position = lastTeeNode.Value.endNode2.position;
				lastTeeNode.Previous.Value.patchTransform.rotation = lastTeeNode.Value.endNode2.rotation;
				reconnectPatches(lastTeeNode);
			}
			//lastTeeNode = null;
		}
	}

	/// <summary>
	/// Updates the patch.
	/// </summary>
	/// <returns>The patch.</returns>
	/// <param name="decission">Directions decission</param>
	/// 
	public Transform updatePatch()
	{
		PatchTypes nextPatchType = selectNextpatchType();

		if (lastTeeNode != null && (nextPatchType == PatchTypes.tee || nextPatchType==PatchTypes.TLeft || nextPatchType==PatchTypes.TRight))
		{
			nextPatchType = PatchTypes.left;
		}


		Transform tempT = instentiatNextPach(nextPatchType);
		tempT.transform.parent = PatchGroup.transform;
		LinkedListNode<Patch> newPatch = new LinkedListNode<Patch>(new Patch(tempT,nextPatchType));
		print (patchesList.First.Value.endNode);
		newPatch.Value.patchTransform.position = patchesList.First.Value.endNode.position;
		newPatch.Value.patchTransform.rotation = patchesList.First.Value.endNode.rotation;

		patchesList.AddFirst(newPatch);

		if (nextPatchType == PatchTypes.tee || nextPatchType==PatchTypes.TLeft || nextPatchType==PatchTypes.TRight)
		{
			lastTeeNode = patchesList.First;
		}

		currentPatch = tempT.gameObject;
		currentpatchType = nextPatchType;

		if(patchesList.Count>queueCapacity)
		{
			Transform T = patchesList.Last.Value.patchTransform;

<<<<<<< HEAD
			if (patchesList.Last.Value.patchType == PatchTypes.tee || patchesList.Last.Value.patchType==PatchTypes.TLeft || patchesList.Last.Value.patchType==PatchTypes.TRight)
=======
			if (patchesList.Last.Value.patchType == PatchTypes.tee || patchesList.Last.Value.patchType==PatchTypes.TLeft 
				|| patchesList.Last.Value.patchType==PatchTypes.TRight)
>>>>>>> d014ce9d4d0b2c964a9c37c61bd26178aa9abee9
			{
				//Debug.Log("nullyfy the T-Node");
				lastTeeNode = null;
			}
			if ( patchesList.Last.Value.obstacleGroup != null)
			{
				Transform obsGroup = patchesList.Last.Value.obstacleGroup;
				Debug.Log("++>"+obsGroup.childCount);
				Transform[] tempObs = new Transform [obsGroup.childCount];
				int i = 0;
				for (i = 0; i < obsGroup.childCount; i++) 
				{
					tempObs[i] = obsGroup.GetChild(i);
				}

				for (i = 0; i < tempObs.Length; i++) 
				{
					tempObs[i].parent = elementsMainGroup.transform;
				}

			}
			else Debug.Log("Usman");
			GameObject.Destroy(T.gameObject);
			patchesList.RemoveLast();
		}
		hElementsGenerator.addObstacles(patchesList.First.Next.Value.patchTransform,patchesList.First.Next.Value.patchType);
		return getNextPatchMidNode();

	}

	Transform instentiatNextPach(PatchTypes pType)
	{
		//if(patchesList.Count > queueCapacity)
			
		GameObject nextPatch = getNextPatch(pType);
		return ((GameObject)Instantiate(nextPatch, Vector3.zero, Quaternion.identity)).transform ;
	}

	void reconnectPatches(LinkedListNode<Patch> p)
	{
		LinkedListNode<Patch> point = p.Previous;
		//if(point != null && point == patchesList.Last)
		int count=0;
		//print (point != patchesList.Last);
		while(point.Previous != null)
		{
			count++;
			//print (count);
			point.Previous.Value.patchTransform.position = point.Value.endNode.position;
			point.Previous.Value.patchTransform.rotation = point.Value.endNode.rotation;
			//print (point.Next.Value.patchTransform.name +""+ point.Value.patchTransform.name);
			point = point.Previous;
		}
	}
	float tick = 0;
	void Update()
	{
		if (tick>1)
		{
			tick = 0;
			//updatePatch();
		}
		tick += Time.deltaTime;
		if (Input.GetKeyUp(KeyCode.D))
		{
			//makeDecision(2);
		}
		if (Input.GetKeyUp(KeyCode.A))
		{
			updatePatch();
		}
	}
}