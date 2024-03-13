using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelMatch {
	
	public class RewardConfirmDialog : MonoBehaviour
	{
		[SerializeField]
		private RewardManager _rewardManager;
		
		private Prop _currentRequestProp;
		
		public void Open(Prop requestProp) {
			_currentRequestProp = requestProp;
			gameObject.SetActive(true);
		}
	
		public void Close() {
			Debug.Log("Close");
			gameObject.SetActive(false);
		}
	
		public void GetByGold() {
			Debug.Log("GetByGold");
			gameObject.SetActive(false);
			_rewardManager.Reward(_currentRequestProp.GetIcon(),
				_currentRequestProp.GetTargetRect(),
				() => _currentRequestProp.Num += 1);
		}
	
		public void GetByAd() {
			gameObject.SetActive(false);
			_rewardManager.Reward(_currentRequestProp.GetIcon(),
				_currentRequestProp.GetTargetRect(),
				() => _currentRequestProp.Num += 1);
		}
	}

}
