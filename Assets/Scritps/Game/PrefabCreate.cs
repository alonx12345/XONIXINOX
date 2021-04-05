//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//public class PrefabCreate : MonoBehaviour
//{

//    private void Start()
//    {
//        List<bool[][]> gridList = GameData.LoadGrids();
//        GridHolder holder  = FindObjectOfType<GridHolder>();
//        holder.Grids = gridList;

//        CreatePrefab(holder.gameObject);
//    }



//    // Creates a new menu item 'Examples > Create Prefab' in the main menu.
//    static void CreatePrefab(GameObject i_Object)
//    {
//        // Keep track of the currently selected GameObject(s)
//            // Set the path as within the Assets folder,
//            // and name it as the GameObject's name with the .Prefab format
//        string localPath = "Assets/" + i_Object.name + ".prefab";

//        // Make sure the file name is unique, in case an existing Prefab has the same name.
//        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

//        // Create the new Prefab.
//        PrefabUtility.SaveAsPrefabAssetAndConnect(i_Object, localPath, InteractionMode.UserAction);
//    }
//}

//    // Disable the menu item if no selection is in place.
//    //static bool ValidateCreatePrefab()
//    //{
//    //    return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
//    //}
