using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerVector3 : DebugUIHandlerWidget
	{
		public Text nameLabel;

		public UIFoldout valueToggle;

		public DebugUIHandlerIndirectFloatField fieldX;

		public DebugUIHandlerIndirectFloatField fieldY;

		public DebugUIHandlerIndirectFloatField fieldZ;

		private DebugUI.Vector3Field m_Field;

		private DebugUIHandlerContainer m_Container;

		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			m_Field = CastWidget<DebugUI.Vector3Field>();
			m_Container = GetComponent<DebugUIHandlerContainer>();
			nameLabel.text = m_Field.displayName;
			fieldX.getter = () => m_Field.GetValue().x;
			fieldX.setter = delegate(float v)
			{
				SetValue(v, x: true);
			};
			fieldX.nextUIHandler = fieldY;
			SetupSettings(fieldX);
			fieldY.getter = () => m_Field.GetValue().y;
			fieldY.setter = delegate(float v)
			{
				SetValue(v, x: false, y: true);
			};
			fieldY.previousUIHandler = fieldX;
			fieldY.nextUIHandler = fieldZ;
			SetupSettings(fieldY);
			fieldZ.getter = () => m_Field.GetValue().z;
			fieldZ.setter = delegate(float v)
			{
				SetValue(v, x: false, y: false, z: true);
			};
			fieldZ.previousUIHandler = fieldY;
			SetupSettings(fieldZ);
		}

		private void SetValue(float v, bool x = false, bool y = false, bool z = false)
		{
			Vector3 value = m_Field.GetValue();
			if (x)
			{
				value.x = v;
			}
			if (y)
			{
				value.y = v;
			}
			if (z)
			{
				value.z = v;
			}
			m_Field.SetValue(value);
		}

		private void SetupSettings(DebugUIHandlerIndirectFloatField field)
		{
			field.parentUIHandler = this;
			field.incStepGetter = () => m_Field.incStep;
			field.incStepMultGetter = () => m_Field.incStepMult;
			field.decimalsGetter = () => m_Field.decimals;
			field.Init();
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			if (fromNext || !valueToggle.isOn)
			{
				nameLabel.color = colorSelected;
			}
			else if (valueToggle.isOn)
			{
				if (m_Container.IsDirectChild(previous))
				{
					nameLabel.color = colorSelected;
				}
				else
				{
					DebugUIHandlerWidget lastItem = m_Container.GetLastItem();
					DebugManager.instance.ChangeSelection(lastItem, fromNext: false);
				}
			}
			return true;
		}

		public override void OnDeselection()
		{
			nameLabel.color = colorDefault;
		}

		public override void OnIncrement(bool fast)
		{
			valueToggle.isOn = true;
		}

		public override void OnDecrement(bool fast)
		{
			valueToggle.isOn = false;
		}

		public override void OnAction()
		{
			valueToggle.isOn = !valueToggle.isOn;
		}

		public override DebugUIHandlerWidget Next()
		{
			if (!valueToggle.isOn || m_Container == null)
			{
				return base.Next();
			}
			DebugUIHandlerWidget firstItem = m_Container.GetFirstItem();
			if (firstItem == null)
			{
				return base.Next();
			}
			return firstItem;
		}
	}
}
