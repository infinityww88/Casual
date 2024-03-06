using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using QFSW.QC;
using UnityEngine.Events;
using UnityEngine.UI;

using Random = UnityEngine.Random;

namespace ModelMatch {
	
	public class GameLogic : MonoBehaviour
	{
		[TabGroup("Events")]
		[SerializeField]
		private UnityEvent<GameObject> _OnPickupComponent;
		
		[TabGroup("Config")]
		public WallController wallController;
		
		public Transform ComponentsRoot;
		
		public Transform TaskPivot;
		public Transform TaskRoot;
		
		public Transform TaskAnchor;
		
		public Material solidMaterial;
		
		public float m_AreaMargin = 0.5f;
		
		public LevelData m_Level;
		public PrefabTexData m_PrefabTaxMap;
		public Image m_Card;
		
		private List<Task> tasks = new	List<Task>();
		private Task currTask = null;
		
		public int oneLayerNum = 10;
		public float heightStep = 1f;
		public RectTransform rootPanel;
		
		#region debug command
		
		#endregion
		
		[Button]
		private void InitLevel() {
			wallController.AlignWalls();
			InitTask();
			InitComponents();
		}
		
		[Button]
		void CanvasInfo() {
			Debug.Log($"{Camera.main.pixelRect}");
		}
		
		void SetCardImage(GameObject prefab, Sprite frontTex, Sprite backTex) {
			m_Card.sprite = frontTex;
		}
		
		protected IEnumerator Start()
		{
			var offY =  Screen.safeArea.y;
			var off = rootPanel.offsetMax;
			off.y = -offY;
			rootPanel.offsetMax = off;
			
			yield return new WaitForSeconds(0.2f);
			wallController.AlignWalls();
			TaskPivot.position = TaskAnchor.position;
			yield return 0;
			InitLevel();
			NextTask();
		}
	
		[Button]
		public void Blow() {
			var compRigids = ComponentsRoot.GetComponentsInChildren<Rigidbody>();
			foreach (var body in compRigids) {
				var xz = UnityEngine.Random.insideUnitCircle;
				float mag = UnityEngine.Random.Range(3, 15);
				var angle = UnityEngine.Random.Range(0, 60);
				var rot = Quaternion.AngleAxis(angle, new Vector3(xz.x, 0, xz.y));
				var force = rot * Vector3.up;
				force = force.normalized * mag;
				body.AddForce(force, ForceMode.Impulse);
			}
		}
		
		// This function is called when the object becomes enabled and active.
		protected void OnEnable()
		{
			GlobalManager.Instance.OnPickupComponent += OnPickupComponent;
			GlobalManager.Instance.OnBlow += Blow;
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			GlobalManager.Instance.OnPickupComponent -= OnPickupComponent;
			GlobalManager.Instance.OnBlow -= Blow;
		}
		
		void OnPickupComponent(GameObject o) {
			ComponentData comp = o.GetComponent<ComponentData>();
			Task currTask = tasks.First();
			if (currTask.ComponentAvailable(comp)) {
				_OnPickupComponent.Invoke(o);
				StartCoroutine(TweenAssembleComponent(comp));
			}
		}
		
		IEnumerator TweenAssembleComponent(ComponentData comp) {
			yield return currTask.AssembleComponent(comp);
			if (currTask.Done()) {
				NextTask();
			}
		}
		
		[Button]
		private Task NextTask() {
			if (currTask != null) {
				Destroy(currTask.gameObject);
				tasks.RemoveAt(0);
			}
			if (tasks.Count == 0) {
				currTask = null;
				return currTask;
			}
			currTask = tasks.First();
			SetCardImage(currTask.gameObject, currTask.m_FrontTex, currTask.m_BackTex);
			currTask.gameObject.SetActive(true);
			currTask.transform.localPosition = Vector3.zero;
			currTask.transform.localRotation = Quaternion.identity;
			currTask.transform.localScale = Vector3.one;
			//currTask.transform.SetParent(TaskRoot);
			//currTask.transform.localPosition = Vector3.zero;
			currTask.Begin();
			
			return currTask;
		}
		
		private void InitTask() {
			m_Level.tasks.ForEach(item => {
				var prefab = item.m_Model;
				for (int i = 0; i < item.m_Num; i++) {
					var taskObj = Instantiate(prefab, TaskRoot);
					var task = taskObj.AddComponent<Task>();
					task.m_FrontTex = task.m_BackTex = m_PrefabTaxMap.map[prefab];
					tasks.Add(task);
					taskObj.SetActive(false);
				}
			});
		}
		
		private void InitComponents() {
			List<GameObject> allComps = new List<GameObject>();
			
			m_Level.tasks.ForEach(item => {
				AddModelComponents(allComps, item.m_Model, item.m_Num);
			});
			m_Level.intersperses.ForEach(item => {
				AddModelComponents(allComps, item.m_Model, item.m_Num);
			});
			Spread(allComps);
		}
		
		private void AddModelComponents(List<GameObject> comps, GameObject prefab, int num) {
			for (int i = 0; i < num; i++) {
				var model = Instantiate(prefab);
				int n = model.transform.childCount;
				for (int j = 0; j < n; j++){
					var c = model.transform.GetChild(0);
					c.gameObject.AddComponent<Rigidbody>();
					var collider = c.gameObject.AddComponent<MeshCollider>();
					collider.convex = true;
					comps.Add(c.gameObject);
					c.transform.SetParent(ComponentsRoot);
				}
				Destroy(model);
			}
		}

		private void Spread(List<GameObject> allComps) {
			var posGenerator = SpreadPosGenerator(oneLayerNum, heightStep);
			allComps.ForEach(comp => {
				posGenerator.MoveNext();
				comp.transform.position = posGenerator.Current;
				comp.transform.rotation = Quaternion.Euler(
					Random.Range(0, 360f),
					Random.Range(0, 360f),
					Random.Range(0, 360f));
			});
		}
		
		// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
		protected void OnDrawGizmos()
		{
			Vector4 area = wallController.GetPlayArea();
			Bounds bound = new Bounds(new Vector3((area.x + area.y) / 2, 0, (area.z + area.w) / 2),
				new Vector3(area.y - area.x - 2 * m_AreaMargin, 0.01f, area.z - area.w - 2 * m_AreaMargin));
			
			DebugExtension.DrawBounds(bound, Color.green);
		}
		
		IEnumerator<Vector3> SpreadPosGenerator(int oneLayerNum, float heightStep) {
			Vector4 area = wallController.GetPlayArea();
			float y = 0;
			while (true) {
				for (int j = 0; j < oneLayerNum; j++) {
					float x = Random.Range(area.x + m_AreaMargin, area.y - m_AreaMargin);
					float z = Random.Range(area.z - m_AreaMargin, area.w + m_AreaMargin);
					Vector3 pos = new Vector3(x, y, z) + transform.position;
					yield return pos;
				}
				y += heightStep;
			}
		}
		
	}
}

