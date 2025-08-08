using UnityEngine;
using VGSoftware.Framework;

namespace Assets.VGSoftware.Framework.Controllers
{
	public class SimulationController : MonoBehaviour
	{
		public static SimulationController Instance { get; private set; }

		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				DontDestroyOnLoad(this);
				Instance = this;
			}
		}

		private void Update()
		{
			Simulation.Tick();
		}
	}
}
