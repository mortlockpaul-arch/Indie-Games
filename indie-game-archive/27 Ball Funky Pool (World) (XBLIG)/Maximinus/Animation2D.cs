using MaximinusDataTypes;

namespace Maximinus;

public class Animation2D
{
	private Layer[] layers;

	private float frameTime;

	private bool isLooping;

	public Layer[] Layers => layers;

	public float FrameTime => frameTime;

	public bool IsLooping => isLooping;

	public int FrameCount => Layers.Length;

	public int FrameWidth => Layers[0].Tex.Width;

	public int FrameHeight => Layers[0].Tex.Height;

	public Animation2D(Layer[] layers, float frameTime, bool isLooping)
	{
		this.layers = layers;
		this.frameTime = frameTime;
		this.isLooping = isLooping;
	}
}
