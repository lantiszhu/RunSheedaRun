using UnityEngine;
using UnityEditor;
using System.Collections;

public class TextureSetings : EditorWindow {

	//string myString = "Hello World";
	bool groupEnabled;
	//bool myBool = true;
	//float myFloat = 1.23f;
	
	string[] usedFormatetypesIOS = {TextureImporterFormat.AutomaticTruecolor.ToString(),TextureImporterFormat.PVRTC_RGB4.ToString(),TextureImporterFormat.PVRTC_RGBA4.ToString()};
	string[] usedFormatetypesAndroid = {TextureImporterFormat.AutomaticTruecolor.ToString(),TextureImporterFormat.ETC_RGB4.ToString(),TextureImporterFormat.DXT5.ToString()};
	
	TextureImporterFormat[] IosAlpha  = {TextureImporterFormat.AutomaticTruecolor,TextureImporterFormat.PVRTC_RGB4,TextureImporterFormat.PVRTC_RGBA4};
	TextureImporterFormat[] AndroindAlpha = {TextureImporterFormat.AutomaticTruecolor,TextureImporterFormat.ETC_RGB4,TextureImporterFormat.DXT5};
	
	Object[] selectionList;
	int selectedIosna = 0;
	//int selectedIos = 0;
	//int selectedAndroid = 0;
	int selectedAndroidNa = 0;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/MyWindow")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(TextureSetings));
	} 
	void SetTextureSeting()
	{	
		int size=0;
		TextureImporterFormat taxFormat = TextureImporterFormat.Alpha8;
		int Quality = 0;
		
        foreach (Texture2D texture in selectionList)  
        {
            string path = AssetDatabase.GetAssetPath(texture);
            //Debug.Log("path: " + path);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.GetPlatformTextureSettings("iPhone",out size,out taxFormat,out Quality);
            Debug.Log(taxFormat +" "+IosAlpha[selectedIosna]);
            Debug.Log((taxFormat == IosAlpha[selectedIosna]) + " ");
            if (taxFormat.ToString() == IosAlpha[selectedIosna].ToString())
            {
	            textureImporter.SetPlatformTextureSettings("Android",size,AndroindAlpha[selectedAndroidNa],Quality); 
	            Debug.Log("IOS "+ size+" "+taxFormat+" "+Quality);
	            size = 0;
	            
	            textureImporter.GetPlatformTextureSettings("Android",out size,out taxFormat,out Quality);
	            Debug.Log("Android "+ size+" "+taxFormat+" "+Quality);
	        }
	    }
		
	}

	void whatsSelected()
	{
		selectionList = Selection.GetFiltered(typeof(Texture2D),SelectionMode.Assets);
		for (int i=0; i< selectionList.Length;i++)
			Debug.Log(selectionList[i].name);
		//Texture2D tex;
	}
	void OnGUI()
	{
		
		if(GUILayout.Button("What is selected"))
		{
			whatsSelected();
		}
		
		groupEnabled = EditorGUILayout.BeginToggleGroup ("Override for Android", groupEnabled);
		 	
		 	GUILayout.Label("If IOS ==");
		 	selectedIosna = EditorGUILayout.Popup(selectedIosna, usedFormatetypesIOS);
		 	
		 	GUILayout.Label("Then Android");
		 	selectedAndroidNa = EditorGUILayout.Popup(selectedAndroidNa, usedFormatetypesAndroid);
		 	
		 	if(GUILayout.Button("Apply"))
			{
				SetTextureSeting();
			}
		EditorGUILayout.EndToggleGroup ();
	}
}
