using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModelMatch {
	
	public class UI_Play : MonoBehaviour
	{
		private VisualElement root;
	
		// Start is called before the first frame update
		void Start()
		{
			root = GetComponent<UIDocument>().rootVisualElement;
			Debug.Log($"{Screen.safeArea}");
			root.Q("Head").style.marginTop = Screen.safeArea.y;
			root.Q<Button>("BlowButton").RegisterCallback<ClickEvent>(OnBlow);
		}
    
		void OnBlow(ClickEvent evt) {
			GlobalManager.Instance.OnBlow?.Invoke();
		}

		// Update is called once per frame
		void Update()
		{
        
		}
	}

}
