using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;
using Cysharp.Threading.Tasks;
using System;
using QFSW.QC;

public class ResUtils
{
	[Command("ServerIp")]
	static string ip = "192.168.0.102";
	
	private class QueryServices : IBuildinQueryServices {
		public bool Query(string packageName, string fileName, string fileCRC) {
			return false;
		}
	}
	
	private class RemoteServices : IRemoteServices {
		public string GetRemoteMainURL(string fileName) {
			return $"http://{ResUtils.ip}:7777/{fileName}";
		}

		public string GetRemoteFallbackURL(string fileName) {
			return $"http://{ResUtils.ip}:7777/{fileName}";
		}
	}
	
	public static async UniTask InitPackage(string pkgName) {
		var package = YooAssets.GetPackage(pkgName);
		var initParameters = new HostPlayModeParameters();
		initParameters.BuildinQueryServices = new QueryServices();
		initParameters.RemoteServices = new RemoteServices();
		
		var initOperation = package.InitializeAsync(initParameters);
		await initOperation;
		if (initOperation.Status == EOperationStatus.Succeed) {
			Debug.Log($"Init Success");
		}
		else {
			Debug.LogError($"Init faild {initOperation.Error}");
		}
	}
	
	public static async UniTask<string> UpdatePackageVersion() {
		var package = YooAssets.GetPackage("DefaultPackage");
		var operation = package.UpdatePackageVersionAsync();
		await operation;
		if (operation.Status == EOperationStatus.Succeed) {
			string pkgVersion = operation.PackageVersion;
			Debug.Log($"Update package Version: {pkgVersion}");
			return pkgVersion;
		}
		else {
			throw new Exception($"Update Package Version faild: {operation.Error}");
		}
	}
	
	public static async UniTask UpdatePackageManifest(string pkgName, string pkgVersion) {
		//bool savePackageVersion = true;
		var package = YooAssets.GetPackage(pkgName);
		var operation = package.UpdatePackageManifestAsync(pkgVersion, true);
		await operation;
		if (operation.Status == EOperationStatus.Succeed) {
			Debug.Log($"Update Package Manifest success. Version {package.GetPackageVersion()}");
		}
		else {
			throw new Exception($"Update package manifest failed: {operation.Error}");
		}
	}
	
	public static async UniTask DownloadPackagePatch(string pkgName) {
		int downloadingMaxNum = 10;
		int failedTryAgain = 3;
		var package = YooAssets.GetPackage(pkgName);
		var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
		
		if (downloader.TotalDownloadCount == 0) {
			return;
		}
		
		int totalDownloadCount = downloader.TotalDownloadCount;
		long totalDownloadBytes = downloader.TotalDownloadBytes;
		
		downloader.BeginDownload();
		
		await downloader;
		
		if (downloader.Status == EOperationStatus.Succeed) {
			Debug.Log($"Download package patch successed");
		}
		else {
			Debug.Log($"Download package patch failed: {downloader.Error}");
		}
	}
	
	public static async UniTask DownloadRes(string pkgName) {
		try {
			if (!YooAssets.ContainsPackage(pkgName)) {
				YooAssets.CreatePackage(pkgName);
			}
			await InitPackage(pkgName);
			string pkgVersion = await UpdatePackageVersion();
			await UpdatePackageManifest(pkgName, pkgVersion);
			Debug.Log("Don't download res");
			//await DownloadPackagePatch(pkgName);
		} catch (Exception e) {
			Debug.Log($"TestTexture Failed: {e}");
		}
	}
}
