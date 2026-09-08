using System;

namespace Microsoft.Xna.Framework.Audio;

[Serializable]
public struct RendererDetail
{
	public string FriendlyName { get; internal set; }

	public string RendererId { get; internal set; }

	public override bool Equals(object obj)
	{
		return obj is RendererDetail && this == (RendererDetail)obj;
	}

	public override int GetHashCode()
	{
		return ((!string.IsNullOrEmpty(RendererId)) ? RendererId.GetHashCode() : 0) ^ ((!string.IsNullOrEmpty(FriendlyName)) ? FriendlyName.GetHashCode() : 0);
	}

	public static bool operator ==(RendererDetail left, RendererDetail right)
	{
		return left.FriendlyName == right.FriendlyName && left.RendererId == right.RendererId;
	}

	public static bool operator !=(RendererDetail left, RendererDetail right)
	{
		return !(left == right);
	}
}
