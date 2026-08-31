using System;

namespace Z
{
	internal interface w
	{
		string ProjectFile { get; }
	}
}
namespace z
{
	internal interface w : IDisposable
	{
		bool CanReuseTransform { get; }

		bool CanTransformMultipleBlocks { get; }

		int InputBlockSize { get; }

		int OutputBlockSize { get; }

		int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);

		byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
	}
}
