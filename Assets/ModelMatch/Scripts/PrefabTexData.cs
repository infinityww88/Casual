using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ModelMatch {
	[CreateAssetMenu(menuName="ModelMatch/PrefabTexMap", fileName="PrefabTexMap")]
	public class PrefabTexData : SerializedScriptableObject
	{
		public Dictionary<GameObject, Texture> map;
	}

}
