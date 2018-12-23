using UnityEngine;

namespace MidcoreTemplate.Game.Utilities
{
	public class ObjectRotator : MonoBehaviour
	{
		public Transform RotateTr;
		public Vector3 rotationSpeed;

		private void LateUpdate()
		{
			RotateTr.localEulerAngles += rotationSpeed * Time.deltaTime;
		}
	}
}
