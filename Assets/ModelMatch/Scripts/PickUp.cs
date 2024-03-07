using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;

namespace ModelMatch {
	
	public class PickUp : MonoBehaviour
	{
		public LayerMask pickLayerMask;
		
		private GameObject lastPickObject = null;
		
		// Start is called before the first frame update
		void Start()
		{
			
		}
		
		private void HighLight(GameObject go) {
			go.GetComponent<Rigidbody>().isKinematic = true;
			go.transform.localScale *= 1.2f;
			Outline outline;
			if (!go.TryGetComponent<Outline>(out outline)) {
				outline = go.AddComponent<Outline>();
				outline.OutlineColor = Color.yellow;
				outline.OutlineWidth = 6;
			}
			outline.enabled = true;
		}
		
		private void UnhighLight(GameObject go) {
			go.GetComponent<Rigidbody>().isKinematic = false;
			go.transform.localScale /= 1.2f;
			go.GetComponent<Outline>().enabled = false;
		}
		
		public void OnTouchEnd() {
			if (lastPickObject != null) {
				UnhighLight(lastPickObject);
				GlobalManager.Instance.OnPickupComponent?.Invoke(lastPickObject);
				lastPickObject = null;
			}
		}
		
		public void OnFingerUpdate(LeanFinger finger) {
			if (!finger.Up) {
				PickUpModel(finger.ScreenPosition);
			} else {
				OnTouchEnd();
			}
		}
		
		public void PickUpModel(Vector2 pos) {
			Ray ray = Camera.main.ScreenPointToRay(pos);
			if (!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, pickLayerMask)) {
				if (lastPickObject != null) {
					UnhighLight(lastPickObject);
					lastPickObject = null;
				}
				return;
			}
			
			var go = hitInfo.collider.transform.gameObject;
			if (go == lastPickObject) {
				return;
			}
			HighLight(go);
			
			if (lastPickObject != go) {
				if (lastPickObject != null) {
					UnhighLight(lastPickObject);
				}
				lastPickObject = go;
			}
		}
	}
}
