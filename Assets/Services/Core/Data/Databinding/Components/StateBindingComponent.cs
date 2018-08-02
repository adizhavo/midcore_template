using UnityEngine;

namespace Services.Core.Databinding.Components
{
	public class StateBindingComponent : MonoBindingComponent<MonoBehaviour, string>
	{
		[System.Serializable]
		public class States
		{
			public string id;
			public GameObject[] enable;
		}

		public States[] states;
		
		public override void OnValueChanged(string branch, string value)
		{
			foreach (var state in states)
			{
				foreach (var o in state.enable)
				{
					o.SetActive(false);
				}
			}

			foreach (var state in states)
			{
				if (string.Equals(state.id, value))
				{
					foreach (var o in state.enable)
					{
						o.SetActive(true);
					}
					break;
				}
			}
		}
	}
}
