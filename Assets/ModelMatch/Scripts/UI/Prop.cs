using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

namespace ModelMatch {
	
	public class Prop : MonoBehaviour
	{
		[SerializeField]
		private Image _infoBg;
	
		[SerializeField]
		private Image _plusIcon;
	
		[SerializeField]
		private TextMeshProUGUI _numText;
	
		[SerializeField]
		private Color _iconBgColor = Color.green;
		
		[SerializeField]
		private UnityEngine.Events.UnityEvent OnFunc;
		[SerializeField]
		private UnityEngine.Events.UnityEvent<Prop> OnRequestReward;
		
		private int _num;
		
		public void OnClick() {
			if (Num > 0) {
				OnFunc.Invoke();
				Num -= 1;
			} else {
				OnRequestReward.Invoke(this);
			}
		}
		
		public Sprite GetIcon() {
			return GetComponent<Image>().sprite;
		}
		
		public RectTransform GetTargetRect() {
			return GetComponent<RectTransform>();	
		}
		
		[Button]
		public int Num {
			get {
				return _num;
			}
			set {
				_num = value;
				if (_num <= 0) {
					ShowPlus();
				}
				else {
					ShowNumber(_num);
				}
			}
		}
	
		[Button]
		void ShowPlus() {
			_numText.gameObject.SetActive(false);
			_infoBg.color = _iconBgColor;
			_plusIcon.gameObject.SetActive(true);
		}
	
		[Button]
		void ShowNumber(int n) {
			_numText.gameObject.SetActive(true);
			_infoBg.color = Color.white;
			_plusIcon.gameObject.SetActive(false);
		
			n = Mathf.Max(n ,0);
			string text = n.ToString();
			if (n > 99) {
				text = "99+";
			}
			_numText.text = text;
		}
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Start()
		{
			Num = 0;
		}
	}
}

