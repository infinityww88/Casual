using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelMatch {
	
	public class ModelData : MonoBehaviour
	{
		[SerializeField]
		private int m_ID = 0;
		[SerializeField]
		private int m_componentNum = 0;
		
		public int ID => m_ID;
		public int ComponentNum => m_componentNum;
	}
}

