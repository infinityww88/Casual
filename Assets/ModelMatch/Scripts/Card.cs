using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;

namespace ModelMatch {
	
	public class Card : MonoBehaviour
	{
		private Image card;
		[SerializeField]
		private float tweenDuration = 0.5f;
		[SerializeField]
		private RectTransform zoomInFrame;
		[SerializeField]
		private RectTransform offScreenRect;
		[SerializeField]
		private RectTransform spawnRect;
		
		[SerializeField]
		private PreviewTexture previewTexture;
		
		private Transform zoomOutFrame;
		private RectTransform dialog;
		
		private bool frontFace = true;
		
		private bool isZoomIn = false;
		
		// Start is called before the first frame update
		void Start()
		{
			card = GetComponent<Image>();
			Debug.Log($"{gameObject.name} {card}");
			zoomOutFrame = card.transform.parent;
			dialog = zoomInFrame.parent as RectTransform;
		}
		
		public void Flip() {
			float angle = frontFace ? 180 : 0;
			frontFace = !frontFace;
			card.rectTransform.DOLocalRotate(Vector3.up * angle, tweenDuration, RotateMode.Fast);
		}
		
		[Button]
		public Tween OffScreen() {
			return transform.DOMove(offScreenRect.position, 0.4f);
		}
		
		[Button]
		public void ToSpawnRect() {
			transform.position = spawnRect.position;
			transform.localScale = spawnRect.localScale;
		}
		
		[Button]
		public Tween SpawnNew() {
			return DOTween.Sequence()
				.Append(transform.DOLocalMove(Vector3.zero, 0.4f))
				.Join(transform.DOScale(Vector3.one, 0.4f));
		}
		
		public void ZoomIn() {
			isZoomIn = true;
			Zoom(zoomInFrame);
			dialog.gameObject.SetActive(true);
		}
		
		public void ZoomOut() {
			isZoomIn = false;
			Zoom(zoomOutFrame, null);
			dialog.gameObject.SetActive(false);
		}
		
		private void Zoom(Transform parent, TweenCallback onEnd = null) {
			var seq = DOTween.Sequence();
			card.transform.SetParent(parent);
			seq.Append(card.rectTransform.DOAnchorPos(Vector2.zero, tweenDuration));
			seq.Join(card.rectTransform.DOSizeDelta(Vector2.zero, tweenDuration));
			seq.SetTarget(card);
			if (onEnd != null) {
				seq.AppendCallback(onEnd);
			}
		}

		public void Click() {
			if (isZoomIn) {
				Flip();
			} else {
				ZoomIn();
			}
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			if (transform.rotation.eulerAngles.y >= 90) {
				previewTexture.Back();
			} else {
				previewTexture.Front();
			}
		}
	}
}

