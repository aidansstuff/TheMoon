using UnityEngine;
using UnityEngine.Rendering;

public class HDDynamicResolution : MonoBehaviour
{
	[Min(1f)]
	[Tooltip("Sets the desired target frame rate in FPS. If Application.targetFrameRate is already set, Application.targetFrameRate overrides this parameter.")]
	public float DefaultTargetFrameRate = 60f;

	[Min(1f)]
	[Tooltip("Per how many frames we evaluate GPU performance against the target frame rate, using the averaged GPU frame time over frames.")]
	public int EvaluationFrameCount = 15;

	[Tooltip("Sets the number of consecutive times where the GPU performance is above the target to increase dynamic resolution by one step.")]
	public uint ScaleUpDuration = 8u;

	[Tooltip("Sets the number of consecutive times where the GPU performance is below the target to decrease dynamic resolution by one step.")]
	public uint ScaleDownDuration = 4u;

	[Min(1f)]
	[Tooltip("Sets the number of steps to upscale from minimum screen percentage to maximum screen percentage set in the current HDRP Asset.")]
	public int ScaleUpStepCount = 5;

	[Min(1f)]
	[Tooltip("Sets the number of steps to downscale from maximum screen percentage to minimum screen percentage set in the current HDRP Asset.")]
	public int ScaleDownStepCount = 2;

	[Tooltip("Enables the debug view of dynamic resolution.")]
	public bool EnableDebugView;

	private const uint InitialFramesToSkip = 1u;

	private float m_AccumGPUFrameTime;

	private int m_CurrentFrameSlot;

	private float m_GPUFrameTime;

	private uint m_ScaleUpCounter;

	private uint m_ScaleDownCounter;

	private static float s_CurrentScaleFraction = 1f;

	private bool m_Initialized;

	private uint m_InitialFrameCounter;

	private void Update()
	{
		if (!FrameTimingManager.IsFeatureEnabled())
		{
			return;
		}
		if (!m_Initialized)
		{
			if (m_InitialFrameCounter >= 1)
			{
				DynamicResolutionHandler.SetDynamicResScaler(() => s_CurrentScaleFraction);
				m_Initialized = true;
			}
			else
			{
				m_InitialFrameCounter++;
			}
		}
		if (!m_Initialized || !UpdateFrameStats())
		{
			return;
		}
		m_GPUFrameTime = m_AccumGPUFrameTime / (float)EvaluationFrameCount;
		float num = ((Application.targetFrameRate > 0) ? ((float)Application.targetFrameRate) : DefaultTargetFrameRate);
		if (1000f / num - m_GPUFrameTime < 0f)
		{
			m_ScaleUpCounter = 0u;
			m_ScaleDownCounter++;
			if (m_ScaleDownCounter >= ScaleDownDuration)
			{
				m_ScaleDownCounter = 0u;
				s_CurrentScaleFraction = Mathf.Clamp01(s_CurrentScaleFraction - 1f / (float)ScaleDownStepCount);
			}
		}
		else
		{
			m_ScaleDownCounter = 0u;
			m_ScaleUpCounter++;
			if (m_ScaleUpCounter >= ScaleUpDuration)
			{
				m_ScaleUpCounter = 0u;
				s_CurrentScaleFraction = Mathf.Clamp01(s_CurrentScaleFraction + 1f / (float)ScaleUpStepCount);
			}
		}
	}

	private static void ResetScale()
	{
		s_CurrentScaleFraction = 1f;
	}

	private void ResetCounters()
	{
		m_ScaleUpCounter = 0u;
		m_ScaleDownCounter = 0u;
		m_CurrentFrameSlot = 0;
	}

	private bool UpdateFrameStats()
	{
		FrameTimingManager.CaptureFrameTimings();
		FrameTiming[] array = new FrameTiming[1];
		if (FrameTimingManager.GetLatestTimings(1u, array) == 0)
		{
			ResetCounters();
			return false;
		}
		if (array[0].gpuFrameTime == 0.0)
		{
			return false;
		}
		if (array[0].cpuTimeFrameComplete < array[0].cpuTimePresentCalled)
		{
			return false;
		}
		if (m_CurrentFrameSlot == 0)
		{
			m_AccumGPUFrameTime = 0f;
		}
		m_AccumGPUFrameTime += (float)array[0].gpuFrameTime;
		UpdateGUIData(array[0]);
		m_CurrentFrameSlot = (m_CurrentFrameSlot + 1) % EvaluationFrameCount;
		return m_CurrentFrameSlot == 0;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		ResetScale();
		ResetCounters();
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		ResetScale();
	}

	private void UpdateGUIData(FrameTiming timing)
	{
	}

	private void OnGUI()
	{
	}
}
