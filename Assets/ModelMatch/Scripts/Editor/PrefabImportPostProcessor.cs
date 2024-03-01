using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PrefabImportPostProcessor : AssetPostprocessor
{
	protected void OnPostprocessPrefab(GameObject root)
	{
		//Debug.Log($"post processor {root.name} {root.tag} path {EditorUtility.GetPrefabParent(root)} - {AssetDatabase.GetAssetPath(root)} {PrefabUtility.IsPartOfRegularPrefab(root)} path {PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root)}");
	}
}
