using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Controls
{
	[Preserve]
	public class DeltaControl : Vector2Control
	{
		[InputControl(useStateFrom = "y", parameters = "clamp=1,clampMin=0,clampMax=3.402823E+38", synthetic = true, displayName = "Up")]
		[Preserve]
		public AxisControl up { get; set; }

		[InputControl(useStateFrom = "y", parameters = "clamp=1,clampMin=-3.402823E+38,clampMax=0,invert", synthetic = true, displayName = "Down")]
		[Preserve]
		public AxisControl down { get; set; }

		[InputControl(useStateFrom = "x", parameters = "clamp=1,clampMin=-3.402823E+38,clampMax=0,invert", synthetic = true, displayName = "Left")]
		[Preserve]
		public AxisControl left { get; set; }

		[InputControl(useStateFrom = "x", parameters = "clamp=1,clampMin=0,clampMax=3.402823E+38", synthetic = true, displayName = "Right")]
		[Preserve]
		public AxisControl right { get; set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			up = GetChildControl<AxisControl>("up");
			down = GetChildControl<AxisControl>("down");
			left = GetChildControl<AxisControl>("left");
			right = GetChildControl<AxisControl>("right");
		}
	}
}
