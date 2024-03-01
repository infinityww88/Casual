using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ModelMatch {
	public class GlobalManager : MonoBehaviour
	{
		public Action<GameObject> OnPickupComponent;
		public Action OnBlow;
		
		public static GlobalManager Instance;
		
		// Start is called before the first frame update
		void Awake()
		{
			Debug.Log("GlobalManager init");
			Instance = this;
		}
    
		// This function is called when the MonoBehaviour will be destroyed.
		protected void OnDestroy()
		{
		}
	}
}

