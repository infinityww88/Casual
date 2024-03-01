using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelMatch {
	
	public class ComponentData : MonoBehaviour
	{
		[SerializeField]
		private int m_modelID = 0;
		[SerializeField]
		private int m_ID = 0;
		
		public int ModelID => m_modelID;
		public int ID => m_ID;
	}
}
