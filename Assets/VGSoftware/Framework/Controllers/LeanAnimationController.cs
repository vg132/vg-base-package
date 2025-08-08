using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VGSoftware.Framework.Controllers
{
	[RequireComponent(typeof(RectTransform))]
	public class LeanAnimationController : MonoBehaviour
	{
		public enum PositionTypes
		{
			IndexedBased = 0,

			StartingPosition = 10,
			Center = 1,

			Top = 6,
			Bottom = 8,
			Left = 2,
			Right = 4,

			OffScreenTop = 7,
			OffScreenBottom = 9,
			OffscreenLeft = 3,
			OffScreenRight = 5,
		}

		[Serializable]
		public class AnimationPlaceholder
		{
			public PositionTypes positionType;
			public GameObject placeHolderObject;
		}

		[SerializeField]
		private List<AnimationPlaceholder> _positionElements = new List<AnimationPlaceholder>();

		public GameObject GetGameObject(PositionTypes type) => _positionElements.FirstOrDefault(item => item.positionType == type)?.placeHolderObject;
		public GameObject GetGameObject(int index) => _positionElements[index].placeHolderObject;

		public LTDescr MoveToPosition(PositionTypes type, float time, Action onStartCallback = null, Action onCompleteCallback = null) => MoveTo(GetGameObject(type), time, onStartCallback, onCompleteCallback);
		public LTDescr MoveXToPosition(PositionTypes type, float time, Action onStartCallback = null, Action onCompleteCallback = null) => MoveXTo(GetGameObject(type), time, onStartCallback, onCompleteCallback);
		public LTDescr MoveYToPosition(PositionTypes type, float time, Action onStartCallback = null, Action onCompleteCallback = null) => MoveYTo(GetGameObject(type), time, onStartCallback, onCompleteCallback);

		public LTDescr MoveToIndex(int index, float time, Action onStartCallback = null, Action onCompleteCallback = null) => MoveTo(_positionElements[index].placeHolderObject, time, onStartCallback, onCompleteCallback);
		public LTDescr MoveXToIndex(int index, float time, Action onStartCallback = null, Action onCompleteCallback = null) => MoveXTo(_positionElements[index].placeHolderObject, time, onStartCallback, onCompleteCallback);
		public LTDescr MoveYToIndex(int index, float time, Action onStartCallback = null, Action onCompleteCallback = null) => MoveYTo(_positionElements[index]?.placeHolderObject, time, onStartCallback, onCompleteCallback);

		public LTDescr MoveTo(GameObject targetGameObject, float time, Action onStartCallback = null, Action onCompleteCallback = null)
		{
			var transform = targetGameObject.GetComponent<RectTransform>();
			var position = transform.localToWorldMatrix.GetPosition();
			var animation = LeanTween.move(gameObject, position, time);
			if (onStartCallback != null)
			{
				animation = animation.setOnStart(onStartCallback);
			}
			if (onCompleteCallback != null)
			{
				animation = animation.setOnComplete(onCompleteCallback);
			}
			return animation;
		}

		public LTDescr MoveXTo(GameObject targetGameObject, float time, Action onStartCallback = null, Action onCompleteCallback = null)
		{
			var transform = targetGameObject.GetComponent<RectTransform>();
			var position = transform.localToWorldMatrix.GetPosition();
			var animation = LeanTween.moveX(gameObject, position.x, time);
			if (onStartCallback != null)
			{
				animation = animation.setOnStart(onStartCallback);
			}
			if (onCompleteCallback != null)
			{
				animation = animation.setOnComplete(onCompleteCallback);
			}
			return animation;
		}

		public LTDescr MoveYTo(GameObject targetGameObject, float time, Action onStartCallback = null, Action onCompleteCallback = null)
		{
			var transform = targetGameObject.GetComponent<RectTransform>();
			var position = transform.localToWorldMatrix.GetPosition();
			var animation = LeanTween.moveY(gameObject, position.y, time);
			if (onStartCallback != null)
			{
				animation = animation.setOnStart(onStartCallback);
			}
			if (onCompleteCallback != null)
			{
				animation = animation.setOnComplete(onCompleteCallback);
			}
			return animation;
		}
	}
}