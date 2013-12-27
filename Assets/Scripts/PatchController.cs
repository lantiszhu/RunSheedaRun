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
			startNode = patchTransform.Find("Nodes/StartNode");
			midNode = patchTransform.Find("Nodes/MidNode");
			if(type!=PatchTypes.tee)
				endNode = patchTransform.Find("Nodes/EndNode");
			else
			{
				endNode = patchTransform.Find("Nodes/EndNode_1");
				endNode2 = patchTransform.Find("Nodes/EndNode_2");
	
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

	LinkedList<Patch> patchesList;
	const int queueCapacity = 5;
	int straightPatchCount = 2;

	void Start () 
	{	
		patchesList = new LinkedList<Patch>();
			
		if (!currentPatch)
		{
			//currentPatch = GameObject.Instantiate(straightPatchList[0],Vector3.zero,Quaternion.identity) as Transform;
			currentPatch = (GameObject)Instantiate(straightPatchList[0]);
			patchesList.AddFirst(new Patch(currentPatch.transform,PatchTypes.straight));
			currentpatchType = PatchTypes.straight;
		}
		
		straightPatchCount = Random.Range(2,4);
		for (int i = 0; i < queueCapacity-1; i++)
		{
			updatePatch();
		}
	}
	
	public void Restart()
	{
		patchesList.Clear();
		
		if (!currentPatch)
		{
			//currentPatch = GameObject.Instantiate(straightPatchList[0],Vector3.zero,Quaternion.identity) as Transform;
			currentPatch = (GameObject)Instantiate(straightPatchList[0]);
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
			patchType = (PatchTypes)Random.Range(1,3);
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

	/// <summary>
	/// Updates the patch.
	/// </summary>
	/// <returns>The patch.</returns>
	/// <param name="decission">Directions decission</param>
	/// 
	public Transform updatePatch(int decission = 2)
	{
		PatchTypes nextPatchType = selectNextpatchType();
		PatchTypes secondPatchType = PatchTypes.straight;
		GameObject nextPatch = getNextPatch(nextPatchType);
		//Transform tempT = GameObject.Instantiate(nextPatch,Vector3.zero,Quaternion.identity) as Transform;
		Transform tempT = ((GameObject)Instantiate(nextPatch, Vector3.zero, Quaternion.identity)).transform;

		if (currentpatchType != PatchTypes.tee)
		{
			tempT.position = currentPatch.transform.Find("Nodes/EndNode").position;
			tempT.rotation = currentPatch.transform.Find("Nodes/EndNode").rotation;
		}
		else if (currentpatchType == PatchTypes.tee)
		{
			if (decission == 1)
			{
				tempT.position = currentPatch.transform.Find("Nodes/EndNode_1").position;
				tempT.rotation = currentPatch.transform.Find("Nodes/EndNode_1").rotation;
			}
			else if (decission == 2 && currentpatchType == PatchTypes.tee)
			{
				tempT.position = currentPatch.transform.Find("Nodes/EndNode_2").position;
				tempT.rotation = currentPatch.transform.Find("Nodes/EndNode_2").rotation;
			}
		}

		patchesList.AddFirst(new Patch(tempT,currentpatchType));
		currentPatch = tempT.gameObject;
		currentpatchType = nextPatchType;

		if(patchesList.Count>queueCapacity)
		{
			Transform T = patchesList.Last.Value.patchTransform;
			//print (T.name);
			GameObject.Destroy(T.gameObject);
			patchesList.RemoveLast();
		}

		return getNextPatchMidNode();

	}
}