using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides an implementation specific list of buffers stored
/// and maintained by a FrameBuffers object.
///
/// Allows using buffers of a different type, format, and size
/// from FrameBuffers normal implementation.
/// </summary>
public class CustomFrameBufferCollection : List<RenderTarget2D>
{
	/// <summary>
	/// Creates a new CustomFrameBufferCollection instance.
	/// </summary>
	public CustomFrameBufferCollection()
		: base(8)
	{
	}
}
