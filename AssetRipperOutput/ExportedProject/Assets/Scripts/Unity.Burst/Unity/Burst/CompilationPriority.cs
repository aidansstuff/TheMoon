namespace Unity.Burst
{
	internal enum CompilationPriority
	{
		EagerCompilationSynchronous = 0,
		Asynchronous = 1,
		ILPP = 2,
		EagerCompilationAsynchronous = 3
	}
}
