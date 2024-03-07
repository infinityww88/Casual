using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using QFSW.QC;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

using Random = UnityEngine.Random;

namespace ModelMatch {
	
	public class GameLogic : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent<GameObject> _OnPickupComponent;
		
		[TabGroup("Spread")]
		public WallController wallController;
		[TabGroup("Spread")]
		public float m_AreaMargin = 0.5f;
		[TabGroup("Spread")]
		public int oneLayerNum = 10;
		[TabGroup("Spread")]
		public float heightStep = 1f;
		
		[TabGroup("Mount")]
		public Transform ComponentsRoot;
		[TabGroup("Mount")]
		public Transform TaskPivot;
		[TabGroup("Mount")]
		public Transform TaskRoot;
		[TabGroup("Mount")]
		public Transform TaskAnchor;
		
		[TabGroup("UI")]
		[SerializeField]
		private RectTransform rootPanel;
		[TabGroup("UI")]
		[SerializeField]
		private RectTransform interactBlock;
		[TabGroup("UI")]
		[SerializeField]
		private Card taskCard;
		
		public LevelData m_Level;
		
		private List<AssemblyTask> tasks = new	List<AssemblyTask>();
		private AssemblyTask currTask = null;
		
		[SerializeField]
		private ParticleSystem _vfxAssemblySuccess;
		
		#region debug command
		
		#endregion
		
		[TabGroup("TestFunc")]
		[Button]
		private void InitLevel() {
			wallController.AlignWalls();
			InitTask();
			InitComponents();
		}
		
		[TabGroup("TestFunc")]
		[Button]
		async UniTaskVoid OnAssemblySuccess() {
			if (currTask == null) {
				return;
			}
			interactBlock.gameObject.SetActive(true);
			await currTask.transform.DOScale(Vector3.zero, 0.4f).AsyncWaitForCompletion();
			_vfxAssemblySuccess.Play();
			await UniTask.WaitUntil(() => _vfxAssemblySuccess.isStopped);
			await taskCard.OffScreen().AsyncWaitForCompletion();
			taskCard.ToSpawnRect();
			var t0 = taskCard.SpawnNew().AsyncWaitForCompletion();
			NextTask();
			if (currTask == null) {
				await t0;
			} else {
				currTask.transform.localScale = Vector3.zero;
				var t1 = currTask.transform.DOScale(Vector3.one, 0.4f).AsyncWaitForCompletion();
				await UniTask.WhenAll(t0.AsUniTask(), t1.AsUniTask());
			}
			
			interactBlock.gameObject.SetActive(false);
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
	
		[TabGroup("TestFunc")]
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
			AssemblyTask currTask = tasks.First();
			if (currTask.ComponentAvailable(comp)) {
				_OnPickupComponent.Invoke(o);
				StartCoroutine(TweenAssembleComponent(comp));
			}
		}
		
		IEnumerator TweenAssembleComponent(ComponentData comp) {
			yield return currTask.AssembleComponent(comp);
			if (currTask.Done()) {
				OnAssemblySuccess();
			}
		}
		
		[TabGroup("TestFunc")]
		[Button]
		private AssemblyTask NextTask() {
			if (currTask != null) {
				Destroy(currTask.gameObject);
				tasks.RemoveAt(0);
			}
			if (tasks.Count == 0) {
				currTask = null;
				return currTask;
			}
			currTask = tasks.First();
			currTask.gameObject.SetActive(true);
			currTask.transform.localPosition = Vector3.zero;
			currTask.transform.localRotation = Quaternion.identity;
			currTask.transform.localScale = Vector3.one;
			currTask.Begin();
			
			return currTask;
		}
		
		private void InitTask() {
			m_Level.tasks.ForEach(item => {
				var prefab = item.m_Model;
				for (int i = 0; i < item.m_Num; i++) {
					var taskObj = Instantiate(prefab, TaskRoot);
					var task = taskObj.AddComponent<AssemblyTask>();
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

