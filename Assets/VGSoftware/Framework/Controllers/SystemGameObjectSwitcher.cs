using UnityEngine;

namespace VGSoftware.Framework.Controllers
{
	public class SystemGameObjectSwitcher : MonoBehaviour
	{
		[SerializeField]
		private GameObject _default;

		[SerializeField]
		private GameObject _ps4;

		[SerializeField]
		private GameObject _ps5;

		[SerializeField]
		private GameObject _xboxOne;

		[SerializeField]
		private GameObject _xboxSeries;

		[SerializeField]
		private GameObject _switch;

		private void OnEnable()
		{
			GameObject activeGameObject = null;
#if UNITY_PS4
			activeGameObject = _ps4;
#elif UNITY_PS5
			activeGameObject = _ps5;
#elif UNITY_XBOXONE
			activeGameObject = _xboxOne;
#elif UNITY_XBOX_SERIES_X
			activeGameObject = _xboxSeries;
#endif
			if (activeGameObject == null)
			{
				activeGameObject = _default;
			}
			if (_ps4 != null)
			{
				_ps4.SetActive(activeGameObject == _ps4);
			}
			if (_ps5 != null)
			{
				_ps5.SetActive(activeGameObject == _ps5);
			}
			if (_xboxOne != null)
			{
				_xboxOne.SetActive(activeGameObject == _xboxOne);
			}
			if (_xboxSeries != null)
			{
				_xboxSeries.SetActive(activeGameObject == _xboxSeries);
			}
			if (_switch != null)
			{
				_switch.SetActive(activeGameObject == _switch);
			}
			if (_default != null)
			{
				_default.SetActive(activeGameObject == _default);
			}
		}
	}
}