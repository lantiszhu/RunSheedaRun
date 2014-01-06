/*
 * FUNCTION:	Export the complete project including editor specific items
 * 				such as tags, physics, quality and script execution order.
 * */

using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExportProject : ScriptableObject {
	
	[MenuItem ("Custom/Export Project")]
	static void DoApply()
	{
		string[] s = Application.dataPath.Split('/');
	    string projectName = s[s.Length - 2];
	    projectName += ".unitypackage";
		
		string[] projectContent = AssetDatabase.GetAllAssetPaths();
		AssetDatabase.ExportPackage(projectContent, projectName, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets );
		Debug.Log("Project Exported");
		
		
	}
}
