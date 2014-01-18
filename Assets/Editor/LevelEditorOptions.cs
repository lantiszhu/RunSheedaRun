using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

public class LevelEditorOptions : EditorWindow {
		bool groupEnabled = false;
		string fileName;
		string[] options;
		int flags  = 0;
		public static PrefabsHolder.PatternsManeger patternsManeger;
		private string path = "Assets/Editor/LevelPattern.xml";

		PrefabsHolder prefabHolder;
	private Object[] patch;
	int numberOfPatches = 0;
	
	[MenuItem("Window/Patch ObstacleEditor")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(LevelEditorOptions));
	} 
	PrefabsHolder.Pattern selectedPattren;
	void OnGUI()
	{	
		if (options !=null)
			groupEnabled = options.Length!=0;
		groupEnabled = EditorGUILayout.BeginToggleGroup ("Patterns Options", groupEnabled);
			
		GUILayout.Label("Selecte Pattern :: "+flags );
		if (options !=null)
			flags = EditorGUILayout.Popup(flags, options);
		GUILayout.Label("Selecte Pattern :: "+flags );
		
		if(GUILayout.Button("Show selected"))
		{
			Debug.Log(patternsManeger);
			readPatterns();
			selectedPattren = patternsManeger.getPattren(options[flags]);
			if (selectedPattren!=null)
				Debug.Log (selectedPattren.patchName+selectedPattren.Id);
			prefabHolder = GameObject.Find("Helper").GetComponent<PrefabsHolder>();
			prefabHolder.setupPatch(selectedPattren);
		}


		GUILayout.Label("Update Selected Pattern" );
		
		if(GUILayout.Button("Update selected"))
		{
			saveSelectedPattern();
		}

		EditorGUILayout.EndToggleGroup();

		GUILayout.BeginVertical("Label");
		GUILayout.Label("Save new Pattern" );
		
		if(GUILayout.Button("Save new"))
		{
			saveObsticlePattern();
		}

		GUILayout.Label("Load all Pattern File" );
		
		if(GUILayout.Button("Load"))
		{
			readPatterns();
		}

		GUILayout.EndVertical();
		showPosition = EditorGUILayout.Foldout(showPosition, "Warning :: Remove Patterns Section");
		if (showPosition)
		{
			
			GUILayout.Label("Reset all pattern IDs" );
			if(GUILayout.Button("Reset all Ids "))
			{
				resetPetternsId();
			}
			EditorGUILayout.BeginToggleGroup ("Patterns Options", groupEnabled);

			GUILayout.Label("Delete selected patterns" );
			if(GUILayout.Button("Delete selected "))
			{
				patternsManeger.removePatternID(options[flags]);
				savePatternsManeger();
			}
			GUILayout.Label("Delete all patterns" );
			EditorGUILayout.EndToggleGroup();
			if(GUILayout.Button("Delete All"))
			{
				clearAll();
			}
		}
	}
		bool showPosition = false;
		bool showobjects = false;
		public void readPatterns()
		{
		patternsManeger = new PrefabsHolder.PatternsManeger();
			if (System.IO.File.Exists(path))
			{
				XmlSerializer reader = new XmlSerializer(typeof(PrefabsHolder.PatternsManeger));
				System.IO.StreamReader readStream = new System.IO.StreamReader(path);
				patternsManeger = reader.Deserialize(readStream) as PrefabsHolder.PatternsManeger ;
				readStream.Close();
	
				options = new string[patternsManeger.patternCount];
				int i = 0;
			foreach (PrefabsHolder.Pattern item in patternsManeger.straightPattern) 
				{
					options[i] = item.Id;
					Debug.Log(options[i]);
					i++;
				}
			foreach (PrefabsHolder.Pattern item in patternsManeger.leftPattern) 
				{
					options[i] = item.Id;
					i++;
				}
			foreach (PrefabsHolder.Pattern item in patternsManeger.rightPattern) 
				{
					options[i] = item.Id;
					i++;
				}
			foreach (PrefabsHolder.Pattern item in patternsManeger.TPattern) 
				{
					options[i] = item.Id;
					i++;
				}
			foreach (PrefabsHolder.Pattern item in patternsManeger.TLeftPattern) 
				{
					options[i] = item.Id;
					i++;
				}
			foreach (PrefabsHolder.Pattern item in patternsManeger.TRightPattern) 
			{
					options[i] = item.Id;
					i++;
				}
				Debug.Log("ManagerLoaded");
			}
			else
			{
				Debug.Log("File Not Found");
				options = new string[0];
			}
	
		}
		public void saveObsticlePattern()
		{
		patternsManeger = new PrefabsHolder.PatternsManeger();
			if (System.IO.File.Exists(path))
			{
			XmlSerializer reader = new XmlSerializer(typeof(PrefabsHolder.PatternsManeger));
				System.IO.StreamReader readStream = new System.IO.StreamReader(path);
			patternsManeger = reader.Deserialize(readStream) as PrefabsHolder.PatternsManeger ;
				readStream.Close();
			}
			if (Selection.activeObject)
			{
				Transform TempT = Selection.gameObjects[0].transform;
				patternsManeger.AddPattern(TempT);
				savePatternsManeger();
			}
			readPatterns();
		}
		public void saveSelectedPattern()
		{
			if (Selection.activeObject)
			{
				Transform TempT = Selection.gameObjects[0].transform;
				patternsManeger.removePatternID(options[flags]);
				patternsManeger.AddPattern(TempT);
				savePatternsManeger();
			}
			readPatterns();
		}
	public void savePatternsManeger()
	{
		XmlSerializer writer = new XmlSerializer(typeof(PrefabsHolder.PatternsManeger));
		System.IO.StreamWriter file = new System.IO.StreamWriter(path);
		writer.Serialize(file, patternsManeger);
		file.Close();
		readPatterns();
	}
	public void resetPetternsId()
	{
		readPatterns();
		int i = 0;
		foreach (PrefabsHolder.Pattern item in patternsManeger.straightPattern) 
		{
			item.Id = "" + item.patchType.ToString() + "_" + i;
			i++;
		}
		foreach (PrefabsHolder.Pattern item in patternsManeger.leftPattern) 
		{
			item.Id = "" + item.patchType.ToString() + "_" + i;
			i++;
		}
		foreach (PrefabsHolder.Pattern item in patternsManeger.rightPattern) 
		{
			item.Id = "" + item.patchType.ToString() + "_" + i;
			i++;
		}
		foreach (PrefabsHolder.Pattern item in patternsManeger.TPattern) 
		{
			item.Id = "" + item.patchType.ToString() + "_" + i;
			i++;
		}
		foreach (PrefabsHolder.Pattern item in patternsManeger.TLeftPattern) 
		{
			item.Id = "" + item.patchType.ToString() + "_" + i;
			i++;
		}
		foreach (PrefabsHolder.Pattern item in patternsManeger.TRightPattern) 
		{
			item.Id = "" + item.patchType.ToString() + "_" + i;
			i++;
		}
		patternsManeger.lastID = i;
		savePatternsManeger();
		readPatterns();
	
}
public void clearAll()
{	
	File.Delete(path);
	readPatterns();
}
}
