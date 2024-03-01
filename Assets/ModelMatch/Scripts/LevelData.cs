using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ModelMatch {
	
	[Serializable]
	public class ModelItem {
		public GameObject m_Model;
		public int m_Num = 1;
	}
	
	[CreateAssetMenu(menuName="ModelMatch/LevelData", fileName="level", order=-1)]
	public class LevelData : ScriptableObject
	{
		public List<ModelItem> tasks;
		public List<ModelItem> intersperses;
	}

}
