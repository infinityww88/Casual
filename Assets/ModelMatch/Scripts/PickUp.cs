using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

namespace ModelMatch {
	
	public class PickUp : MonoBehaviour
	{
		public LayerMask pickLayerMask;
		
		private Outline lastHighlight = null;
		
		// Start is called before the first frame update
		void Start()
		{
			
		}
		
		public void OnTouchCancel() {
			if (lastHighlight != null) {
				lastHighlight.enabled = false;
				GlobalManager.Instance.OnPickupComponent?.Invoke(lastHighlight.gameObject);
				lastHighlight = null;
			}
		}
		
		public void OnFingerUpdate(LeanFinger finger) {
			if (!finger.Up) {
				PickUpModel(finger.ScreenPosition);
			} else {
				OnTouchCancel();
			}
		}
		
		public void PickUpModel(Vector2 pos) {
			Ray ray = Camera.main.ScreenPointToRay(pos);
			if (!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, pickLayerMask)) {
				if (lastHighlight != null) {
					lastHighlight.enabled = false;
					lastHighlight = null;
				}
				return;
			}
			
			var go = hitInfo.collider.transform.gameObject;
			if (go == lastHighlight) {
				return;
			}
			Outline outline;
			if (!go.TryGetComponent<Outline>(out outline)) {
				outline = go.AddComponent<Outline>();
				outline.OutlineColor = Color.yellow;
				outline.OutlineWidth = 6;
			}
			
			if (lastHighlight != outline) {
				if (lastHighlight != null) {
					lastHighlight.enabled = false;
				}
				lastHighlight = outline;
			}
			
			outline.enabled = true;
		}
	}

}
