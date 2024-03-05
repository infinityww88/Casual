using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace ModelMatch {
	
	public class Card : MonoBehaviour
	{
		[SerializeField]
		private Image card;
		[SerializeField]
		private float tweenDuration = 0.5f;
		[SerializeField]
		public Vector3 zoomMove = Vector3.zero;
		[SerializeField]
		public float zoomScale = 1;
		
		private bool frontFace = true;
		private bool zoom = false;
		
		private Tweener flipTweener = null;
		private Tweener zoomTweener = null;
		
		// Start is called before the first frame update
		void Start()
		{
			
		}
		
		[Button]
		public void Flip() {
			float angle = frontFace ? 180 : 0;
			frontFace = !frontFace;
			flipTweener = card.rectTransform.DOLocalRotate(Vector3.up * angle, tweenDuration, RotateMode.Fast);
		}
		
		[Button]
		void TestFlip() {
			if (flipTweener != null) {
				flipTweener.Flip();
			}
		}
		
		[Button]
		public void Zoom() {
			Vector3 move = zoom ? Vector3.zero : zoomMove;
			float scale = zoom ? 1 : zoomScale;
			
			if (zoom) {
				zoom = false;
			}
			
			card.rectTransform.DOLocalMove(move, tweenDuration, true);
			card.rectTransform.DOScale(scale, tweenDuration);
			
			if (zoom) {
				
			}
		}

		// Update is called once per frame
		void Update()
		{
        
		}
	}
}

