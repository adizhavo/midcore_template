using UnityEngine.UI;

namespace Services.Core.Databinding.Components
{
	public class SliderBindingComponent : MonoBindingComponent<Slider, float>
	{
		#region BindingComponent implementation

		public override void OnValueChanged(string branch, float value)
		{
			component.value = value;
		}

		#endregion
	}
}
