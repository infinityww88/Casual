using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class RewardManager : MonoBehaviour
{
	[SerializeField]
	private Image _icon;
	
	[SerializeField]
	private float duration = 0.3f;
	
	[SerializeField]
	private float targetScale = 0.2f;
	
	[Button]
	public void Reward(Sprite rewardIcon,
		RectTransform targetRect,
		TweenCallback onTweenEnd = null) {
		_icon.sprite = rewardIcon;
		Debug.Log($"Reward {targetRect.gameObject.name}");
		gameObject.SetActive(true);
		_icon.transform.localPosition = Vector3.zero;
		_icon.transform.localScale = Vector3.zero;
		DOTween.Sequence()
			.Append(_icon.rectTransform.DOScale(1, duration).SetEase(Ease.OutBounce))
			.AppendInterval(duration)
			.Append(
			_icon.transform.DOMove(targetRect.position, duration))
			.Join(
			_icon.transform.DOScale(targetScale, duration)).OnComplete(() => {
				gameObject.SetActive(false);
				onTweenEnd?.Invoke();
			});
	}
}
