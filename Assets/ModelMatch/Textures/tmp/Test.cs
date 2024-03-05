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
using System.IO;
using QFSW.QC;
using UnityEngine.Networking;
using YooAsset;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
	public Transform target;
	public float duration;
	private CancellationTokenSource cts = null;
	public GameObject resTarget;
	public Texture texture;
	
    // Start is called before the first frame update
    void Start()
	{
		InitRes();
	}
	
	void InitRes() {
		YooAssets.Initialize();
	}
	
	[Command]
	[Button]
	void DownloadRes() {
		ResUtils.DownloadRes("DefaultPackage");
	}
	
	[Command]
	[Button]
	async UniTask RunScene(int index) {
		if (index != 0) index = 1;
		var package = YooAssets.GetPackage("DefaultPackage");
		var sceneHandle = package.LoadSceneAsync($"Scenes_Level{index}",	LoadSceneMode.Single, false);
		await sceneHandle;
	}

	[Command]
	[Button]
	async UniTask RunRes() {
		var package = YooAssets.GetPackage("DefaultPackage");
		var handle = package.LoadAssetAsync<Texture>($"Texture_TestTexture");
		await handle;
		var texture = handle.AssetObject as Texture;
		Debug.Log($"downlaod texture {texture}");
		resTarget.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", texture);
	}
	
	[Command]
	[Button]
	async UniTask RunCode() {
		var package = YooAssets.GetPackage("DefaultPackage");
		var handle = package.LoadAssetAsync<TextAsset>($"HotUpdateAssembly.dll");
		await handle;
		var data = handle.AssetObject as TextAsset;
		Debug.Log($"download {data.bytes.Length}");
		Assembly hotUpdateAss = Assembly.Load(data.bytes);
		Type type = hotUpdateAss.GetType("Hello");
		type.GetMethod("Run").Invoke(null, null);
	}
	
	[Command]
	void RunLocal() {
		// Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
		Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdateAssembly.dll"));
#else
		// Editor下无需加载，直接查找获得HotUpdate程序集
		Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdateAssembly");
#endif
    
		Type type = hotUpdateAss.GetType("Hello");
		type.GetMethod("Run").Invoke(null, null);
	}
	
	[Command]
	async UniTask RunRemote(string ip = "192.168.0.102") {
		string url = $"http://{ip}:7777/HotUpdateAssembly.dll";
		Debug.Log($"download {url}");
		using (UnityWebRequest request = UnityWebRequest.Get(url)) {
			request.timeout = 10;
			await request.SendWebRequest();
			Debug.Log($"complete: {request.result}");
			if (request.result != UnityWebRequest.Result.Success) {
				Debug.Log($"error: {request.result}");
			}
			else {
				Debug.Log($"get data");
				try {
					var data = request.downloadHandler.data;
					Debug.Log($"download {data.Length}");
					Assembly hotUpdateAss = Assembly.Load(data);
					Type type = hotUpdateAss.GetType("Hello");
					type.GetMethod("Run").Invoke(null, null);
				} catch (Exception e) {
					Debug.Log(e);
				}
				
			}
		}
	}	
}
