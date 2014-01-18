using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
 
public class TransformCopier : ScriptableObject
{
        private static Vector3 position;
        private static Quaternion rotation;
        private static Vector3 scale;
        //private static string myName;
    
    [MenuItem ("Custom/Transform Copier/Copy Transform &c")]
    static void DoRecord()
    {
       position = Selection.activeTransform.localPosition;
       rotation = Selection.activeTransform.localRotation;
       scale = Selection.activeTransform.localScale;
      // myName = Selection.activeTransform.name;       
        
        //EditorUtility.DisplayDialog("Transform Copy", "Local position, rotation, & scale of "+myName +" copied relative to parent.", "OK", "");
    }
 
    [MenuItem ("Custom/Transform Copier/Paste Transform &v")]
    static void DoApply()
    {
        Selection.activeTransform.localPosition = position;
        Selection.activeTransform.localRotation = rotation;
        Selection.activeTransform.localScale = scale;      
        
        //EditorUtility.DisplayDialog("Transform Paste", "Local position, rotation, and scale of "+myName +"  pasted relative to parent of "+Selection.activeTransform.name+".", "OK", "");
    }
    
    [MenuItem ("Custom/Take Screenshot &a")]
    static void TakeScreenshot()
    {
		string screenshotFilename;
		int screenshotCount = 1;
		
		int i = 0;
		while(true)
		{
			screenshotFilename = "Screenshot_" + screenshotCount + ".png";
			if(System.IO.File.Exists(screenshotFilename))
				screenshotCount++;
			else
				break;
			i++;
			if(i>=999)
			{
				Debug.LogWarning("Empty your folder. Screenshots limit reached");
				return;
			}
		}
		
		Application.CaptureScreenshot(screenshotFilename);
		Debug.Log("Screenshot saved as : "+screenshotFilename);
    	//Application.CaptureScreenshot("Screenshot.png");
    }
	
	[MenuItem ("Custom/Prune Small Valeus &d")]
    static void PruneSmallValues()
    {        
        float wrong_Val = 0.0f;
        float right_Val_X = 0.0f;
        float right_Val_Y = 0.0f;
        float right_Val_Z = 0.0f;
        
       	wrong_Val = Selection.activeTransform.localScale.x;
        right_Val_X = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localScale.y;
        right_Val_Y = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localScale.z;
        right_Val_Z = Mathf.Round(wrong_Val*100)/100;
        Selection.activeTransform.localScale = new Vector3(right_Val_X, right_Val_Y, right_Val_Z);
        
        wrong_Val = Selection.activeTransform.localPosition.x;
        right_Val_X = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localPosition.y;
        right_Val_Y = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localPosition.z;
        right_Val_Z = Mathf.Round(wrong_Val*100)/100;
        Selection.activeTransform.localPosition = new Vector3(right_Val_X, right_Val_Y, right_Val_Z);
        
        wrong_Val = Selection.activeTransform.localEulerAngles.x;
        right_Val_X = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localEulerAngles.y;
        right_Val_Y = Mathf.Round(wrong_Val*100)/100;
        wrong_Val = Selection.activeTransform.localEulerAngles.z;
        right_Val_Z = Mathf.Round(wrong_Val*100)/100;
        Selection.activeTransform.localEulerAngles = new Vector3(right_Val_X, right_Val_Y, right_Val_Z);
    }
	
	[MenuItem ("Custom/Add Node: &n")]
	static void addEndNode()
	{
		Transform t = Selection.activeTransform;
		float endPoint =t.renderer.bounds.size.z/2;
		float startPoint =t.renderer.bounds.size.z/2;

		Transform startNode =new GameObject("StartNode").transform;
		Transform node =new GameObject("EndNode").transform;

		node.parent = t;
		node.localPosition = new Vector3(0,0,endPoint/t.lossyScale.z);
		node.localRotation = Quaternion.identity;

		startNode.parent = t;
		startNode.localPosition = new Vector3(0,0,-startPoint/t.lossyScale.z);
		startNode.localRotation = Quaternion.identity;

	}
	[MenuItem ("Custom/Add Mid Node: &m")]
	static void addMidNode()
	{
		Transform start = Selection.transforms[0];
		Transform end = Selection.transforms[1];
		Transform midNode =new GameObject("MidNode").transform;

		midNode.position = new Vector3((start.position.x+end.position.x)/2,0,(start.position.z+end.position.z)/2);
		midNode.rotation = Quaternion.identity;
		
	}	[MenuItem ("Custom/Level Editor/Save obsticle pattern &s")]
	public static void saveObsticlePattern()
	{
		//LevelEditorOptions.saveObsticlePattern();
	}
	/*
// copy glyph data from MyFont.ttf into a custom font called f2
// 
	[MenuItem ("Custom/Copy Font Data")]
	static void FontDataCopy() 
	{
		 Debug.Log("FontDataCopy");
	    Font f1 = (Selection.activeObject) as Font;
	    Debug.Log(f1.characterInfo.Length);

	 	Stream st = new FileStream("FontData.txt" , FileMode.OpenOrCreate);
		StreamWriter stw = new StreamWriter(st);
		stw.WriteLine(f1.characterInfo.Length);
	    for (var i = 0; i < f1.characterInfo.Length; i++)
		{
			write(stw , f1.characterInfo[i]);
	    }
		stw.Close();
	}
	
    // copy glyph data from MyFont.ttf into a custom font called f2
// 
	[MenuItem ("Custom/Paste Font Data")]
	static void FontDataPaste() 
	{
		 Debug.Log("FontDataPaste");
	 
	    Font f2  = AssetDatabase.LoadAssetAtPath("Assets/Miscellaneous/Fonts/DR_Font_copy.fontsettings", typeof(Font)) as Font;
		Debug.Log(f2.characterInfo.Length);
	 	
		Stream st = new FileStream("FontData.txt" , FileMode.Open);
		StreamReader sr = new StreamReader(st);
		int charLenght = int.Parse(sr.ReadLine());
	    for (var i = 0; i < charLenght; i++)
		{
	        f2.characterInfo[i] = read(sr);
	    }
	   // f2.characterInfo = myCI;
	}
	
	static void write(StreamWriter sw , CharacterInfo cinfo)
	{
			sw.WriteLine(cinfo.flipped);
			sw.WriteLine(cinfo.index);
			sw.WriteLine(cinfo.size);
			sw.WriteLine(cinfo.style);
			sw.WriteLine(cinfo.uv.x);
			sw.WriteLine(cinfo.uv.y);
			sw.WriteLine(cinfo.uv.width);
			sw.WriteLine(cinfo.uv.height);
			sw.WriteLine(cinfo.vert.x);
			sw.WriteLine(cinfo.vert.y);
			sw.WriteLine(cinfo.vert.width);
			sw.WriteLine(cinfo.vert.height);
			sw.WriteLine(cinfo.width);
	}
	
	static CharacterInfo read(StreamReader sr)
	{
		CharacterInfo cinfo = new CharacterInfo();
			cinfo.flipped = bool.Parse(sr.ReadLine());
		Debug.Log(cinfo.flipped);
			cinfo.index = int.Parse(sr.ReadLine());
			cinfo.size = int.Parse(sr.ReadLine());
			cinfo.style = FontStyle.Normal;
			sr.ReadLine();
			cinfo.uv.x = float.Parse( sr.ReadLine());
			cinfo.uv.y = float.Parse(sr.ReadLine());
			cinfo.uv.width = float.Parse(sr.ReadLine());
			cinfo.uv.height = float.Parse(sr.ReadLine());
			cinfo.vert.x = float.Parse(sr.ReadLine());
			cinfo.vert.y = float.Parse(sr.ReadLine());
			cinfo.vert.width = float.Parse(sr.ReadLine());
			cinfo.vert.height = float.Parse(sr.ReadLine());
			cinfo.width = float.Parse(sr.ReadLine());
		return cinfo;
	}
*/	
}
