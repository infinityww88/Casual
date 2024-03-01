using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ModelMatch {
	
	public class MenuCollection : MonoBehaviour
	{
		[MenuItem("ModelMatch/GenerateModel")]
		public static void GenerateModelID() {
			string path = "Assets/Casual/Prefabs";
			string[] paths = AssetDatabase.FindAssets("t:Prefab", new string[] {path});
			
			paths.Foreach((i, e) => {
				string p = AssetDatabase.GUIDToAssetPath(e);
				GenerateModelID(i, p);
			});
		}
		
		private static void GenerateModelID(int index, string assetPath) {
			using (var scope = new PrefabUtility.EditPrefabContentsScope(assetPath)) {
				var o = scope.prefabContentsRoot;
				ModelData modelData = o.GetOrCreateComponent<ModelData>();
				
				int childCount = o.transform.childCount;
				for (int i = 0; i < childCount; i++) {
					ComponentData compData = o.transform.GetChild(i).gameObject.GetOrCreateComponent<ComponentData>();
					MyEditorUtils.UpdateProperties(compData,
						ValueTuple.Create("m_modelID", (object)index),
						ValueTuple.Create("m_ID", (object)i));
				}
				MyEditorUtils.UpdateProperties(modelData, 
					ValueTuple.Create("m_ID", (object)index),
					ValueTuple.Create("m_componentNum", (object)childCount));
			}
		}
	}

}
