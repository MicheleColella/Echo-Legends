using TARGET = UnityEngine.RectTransform;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the RectTransform's sizeDelta.y value.</summary>
	[UnityEngine.HelpURL(LeanTransition.HelpUrlPrefix + "LeanRectTransformSizeDelta_y")]
	[UnityEngine.AddComponentMenu(LeanTransition.MethodsMenuPrefix + "RectTransform/RectTransform.sizeDelta.y" + LeanTransition.MethodsMenuSuffix + "(LeanRectTransformSizeDelta_y)")]
	public class LeanRectTransformSizeDelta_y : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(TARGET);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Value, Data.Duration, Data.Ease);
		}

		public static LeanState Register(TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Value = value;
			
			state.Ease = ease;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<TARGET>
		{
			[UnityEngine.Tooltip("The sizeDelta value will transition to this.")]
			public float Value;

			[UnityEngine.Tooltip("This allows you to control how the transition will look.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private float oldValue;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.sizeDelta.y != Value ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Value = Target.sizeDelta.y;
			}

			public override void BeginWithTarget()
			{
				oldValue = Target.sizeDelta.y;
			}

			public override void UpdateWithTarget(float progress)
			{
				var vector = Target.sizeDelta;
				
				vector.y = UnityEngine.Mathf.LerpUnclamped(oldValue, Value, Smooth(Ease, progress));
				 
				Target.sizeDelta = vector;
			}

			public static System.Collections.Generic.Stack<State> Pool = new System.Collections.Generic.Stack<State>(); public override void Despawn() { Pool.Push(this); }
		}

		public State Data;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		public static TARGET sizeDeltaTransition_y(this TARGET target, float value, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanRectTransformSizeDelta_y.Register(target, value, duration, ease); return target;
		}
	}
}