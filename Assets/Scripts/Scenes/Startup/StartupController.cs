using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes.Startup
{
  public class StartupController : MonoBehaviour
  {
		private void Update()
		{
			SceneManager.LoadScene(Constants.Scenes.MainMenu);
		}
	}
}
