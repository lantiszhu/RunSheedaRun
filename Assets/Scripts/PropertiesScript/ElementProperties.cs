using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public enum ElementType {Currency , Powerup};

public class ElementProperties : MonoBehaviour {


	#region Public
	public int spoolSize = 1;
	public ElementType elementType;

	#endregion

	void OnCollisionEnter(Collision c)
	{
		print ("Collision");
		GameObject.Find("Controllers").GetComponent<ElementsGenerator>().updateCoinPos(transform);
	}

}
