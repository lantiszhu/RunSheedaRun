using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PatchTypes{ straight , left , right , tee};
public class PatchController : MonoBehaviour {
	
	public class Patch
	{
		public Transform patchTransform;
		public Transform startNode;
		public Transform midNode;
		public Transform endNode;
		public Transform endNode2;
		public PatchTypes patchType;
		
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

			if(type==PatchTypes.tee)
			{
				endNode2 = patchT.Find("Nodes/End_2");
			}

			patchType = type;
		}
	}

	public GameObject[]  straightPatchList;
	public GameObject 	leftTurnPatch;
	public GameObject 	rightTurnPatch;
	public GameObject[]  teePatchList;

	GameObject currentPatch;
	PatchTypes currentpatchType;
	GameObject PatchGroup;

	LinkedList<Patch> patchesList = new LinkedList<Patch>();
	int queueCapacity = 5;
	int straightPatchCount = 2;

	void Start () 
	{	
		PatchGroup = new GameObject();
		PatchGroup.transform.name = "PatchGroup";
		if (!currentPatch)
		{
			//currentPatch = GameObject.Instantiate(straightPatchList[0],Vector3.zero,Quaternion.identity) as Transform;
			currentPatch = (GameObject)Instantiate(straightPatchList[0]);
			currentPatch.transform.parent = PatchGroup.transform;
			patchesList.AddFirst(new Patch(currentPatch.transform,PatchTypes.straight));
			currentpatchType = PatchTypes.straight;
		}
		//tickTime = 0;
		straightPatchCount = Random.Range(2,4);
		for (int i = 0; i < queueCapacity-1; i++)
		{
			updatePatch();
		}
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

	}
	PatchTypes selectNextpatchType()
	{
		PatchTypes patchType;
		if (straightPatchCount>0)
		{
			patchType = PatchTypes.straight;
			straightPatchCount--;
		}
		else
		{
			straightPatchCount = Random.Range(2,4);
			patchType = (PatchTypes)Random.Range(1,4);
		}

		//Debug.Log("==>"+patchType.ToString());
		return patchType;
	}

	GameObject getNextPatch(PatchTypes patchType)
	{
		if (patchType == PatchTypes.straight)
		{
			return straightPatchList[Random.Range(0,straightPatchList.Length)];
		}
		else if (patchType == PatchTypes.left)
		{
			return leftTurnPatch;
		}
		else if (patchType == PatchTypes.right)
		{
			return rightTurnPatch;
		}
		else if (patchType == PatchTypes.tee)
		{
			return teePatchList[Random.Range(0,teePatchList.Length)];
		}
		else
			return null;
	}
	LinkedListNode<Patch> lastTeeNode;

	public void makeDecision(int decision)
	{
		if (decision == 2 && lastTeeNode != null)
		{
			if(lastTeeNode.Next !=null )
			{
				lastTeeNode.Previous.Value.patchTransform.position = lastTeeNode.Value.endNode2.position;
				lastTeeNode.Previous.Value.patchTransform.rotation = lastTeeNode.Value.endNode2.rotation;
				reconnectPatches(lastTeeNode);
			}
			lastTeeNode = null;
		}
	}

	/// <summary>
	/// Updates the patch.
	/// </summary>
	/// <returns>The patch.</returns>
	/// <param name="decission">Directions decission</param>
	/// 
	public Transform updatePatch(int decision = 1)
	{
		PatchTypes nextPatchType = selectNextpatchType();

		if (lastTeeNode != null && nextPatchType == PatchTypes.tee)
		{
			nextPatchType = PatchTypes.left;
		}

		GameObject nextPatch = getNextPatch(nextPatchType);
		Transform tempT = ((GameObject)Instantiate(nextPatch, Vector3.zero, Quaternion.identity)).transform;
		tempT.transform.parent = PatchGroup.transform;
		LinkedListNode<Patch> newPatch = new LinkedListNode<Patch>(new Patch(tempT,nextPatchType));

		newPatch.Value.patchTransform.position = patchesList.First.Value.endNode.position;
		newPatch.Value.patchTransform.rotation = patchesList.First.Value.endNode.rotation;

		patchesList.AddFirst(newPatch);

		if (nextPatchType == PatchTypes.tee)
		{
			lastTeeNode = patchesList.First;
		}

		currentPatch = tempT.gameObject;
		currentpatchType = nextPatchType;

		if(patchesList.Count>queueCapacity)
		{
			Transform T = patchesList.Last.Value.patchTransform;

			if (patchesList.Last.Value.patchType == PatchTypes.tee)
			{
				lastTeeNode = null;
			}

			GameObject.Destroy(T.gameObject);
			patchesList.RemoveLast();
		}
	
		return getNextPatchMidNode();

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
			makeDecision(2);
		}
		if (Input.GetKeyUp(KeyCode.A))
		{
			updatePatch();
		}
	}
}