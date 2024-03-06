using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace ModelMatch {
	
	public class TaskZoom : MonoBehaviour
	{
		[SerializeField]
		private Transform zoomTarget;
		
		[SerializeField]
		private float duration;
		[SerializeField]
		private float zoomScale = 1;
		
		[Button]
		public void ZoomIn() {
			var seq = DOTween.Sequence();
			seq.Append(transform.DOMove(zoomTarget.position, duration));
			seq.Join(transform.DOScale(zoomScale, duration));
			seq.SetTarget(gameObject);
		}
		
		[Button]
		public void ZoomOut() {
			var seq = DOTween.Sequence();
			seq.Append(transform.DOLocalMove(Vector3.zero, duration));
			seq.Join(transform.DOScale(1, duration));
			seq.SetTarget(gameObject);
		}
	}
}

