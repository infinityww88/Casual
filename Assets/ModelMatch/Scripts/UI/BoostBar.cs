using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace ModelMatch {
	
	public class BoostBar : MonoBehaviour
	{
		[SerializeField]
		private Image bar;
		[SerializeField]
		[Range(0.1f, 0.5f)]
		private float boostAmount = 0.1f;
		[SerializeField]
		[MinValue(1f)]
		private float regressTime = 10f;
		
		// Start is called before the first frame update
		void Start()
		{
        
		}
		
		[Button]
		public void Boost() {
			bar.fillAmount += boostAmount;
		}

		// Update is called once per frame
		void Update()
		{
			float amount = bar.fillAmount - Time.deltaTime * (1 / regressTime);
			amount = Mathf.Max(0, amount);
			bar.fillAmount = amount;
		}
	}

}
