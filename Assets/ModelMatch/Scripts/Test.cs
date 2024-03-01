using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Sirenix.OdinInspector;
using System;
using System.Reflection;
using System.Linq;
using HybridCLR;
using TMPro;
using System.IO;

public class Test : MonoBehaviour
{
	public Transform target;
	public float duration;
	private CancellationTokenSource cts = null;
	public TextMeshProUGUI text;
	
    // Start is called before the first frame update
    void Start()
	{
		//cts = new CancellationTokenSource();
		//cts.CancelAfter(new TimeSpan(0, 0, 5));
		//Move(cts);
		
		// Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
		Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes"));
#else
		// Editor下无需加载，直接查找获得HotUpdate程序集
		Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdateAssembly");
#endif
    
		Type type = hotUpdateAss.GetType("Hello");
		type.GetMethod("Run").Invoke(null, new object[] {text});
	}
	
	[Button]
	void HotUpdate() {
		
	}
    
	[Button]
	void Cancel() {
		if (cts != null) {
			if (cts.IsCancellationRequested) {
				Debug.Log("Already Cancelled");
			} else {
				cts.Cancel();
			}
			cts = null;
		}
	}
    
	async UniTask Move(CancellationTokenSource cts) {
		float startTime = Time.time;
		Vector3 startPos = transform.position;
		Vector3 targetPos = target.position;
		float factor = 0;
		while (!cts.IsCancellationRequested) {
			float f = Mathf.PingPong(Time.time - startTime, duration) / duration;
			transform.position = Vector3.Lerp(startPos, targetPos	, f);
			if (f >= 1) {
				var t = startPos;
				startPos = targetPos;
				targetPos = t;
			}
			
			await UniTask.NextFrame();
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
