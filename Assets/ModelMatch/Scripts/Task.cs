using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ModelMatch {
	
	public class Task : MonoBehaviour
	{
		public Material transparentMat;
		[SerializeField]
		private int m_remainComponentNum = 0;
		private uint m_componentMask = 0;
	
		public int RemainComponentNum => m_remainComponentNum;
		public uint CompMask => m_componentMask;
	
		private ModelData modelData;
		
		private Dictionary<int, ComponentData> compMaskMap;
		private Material solidMat;
		
		public Texture m_FrontTex;
		public Texture m_BackTex;
		
		public float lerpFactor = 0.2f;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Awake()
		{
			transparentMat = Resources.Load<Material>("GunsTransparent");
			modelData = GetComponent<ModelData>();
			m_remainComponentNum = modelData.ComponentNum;
			m_componentMask = ~(0xffffffff << modelData.ComponentNum);
			compMaskMap = new Dictionary<int, ComponentData>();
			var comps = GetComponentsInChildren<ComponentData>();
			comps.Foreach(e => {
				compMaskMap[e.ID] = e;
			});
		}
		
		public void Begin() {
			Utils.ForeachChild(transform, child => {
				var renderer = child.GetComponent<MeshRenderer>();
				if (solidMat == null) {
					solidMat = renderer.material;
				}
				renderer.material = transparentMat;
			});
		}
		
		public bool Done() {
			return m_remainComponentNum == 0;
		}
		
		public void End() {
			//var renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
			//renderer.material = solidMat;
		}
		
		public Transform GetCompoentTransform(int compID) {
			if (compMaskMap.ContainsKey(compID)) {
				return compMaskMap[compID].transform;
			}
			return null;
		}
	
		public IEnumerator AssembleComponent(ComponentData comp) {
			ComponentData targetComp = compMaskMap[comp.ID];
			var targetTransform = targetComp.transform;
			m_componentMask = m_componentMask & ~(1u << comp.ID);
			m_remainComponentNum--;
			comp.GetComponent<Rigidbody>().isKinematic = true;
			yield return Utils.TweenTransform(comp.transform, targetTransform, 0.5f, 0.2f);
			targetComp.GetComponent<MeshRenderer>().material = solidMat;
			Destroy(comp.gameObject);
		}
		
		public bool ComponentAvailable(ComponentData compData) {
			if (compData.ModelID != modelData.ID) {
				return false;
			}
			return ((1u << compData.ID) & m_componentMask) != 0;
		}
	}
}

