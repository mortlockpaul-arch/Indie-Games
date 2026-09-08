using System;

namespace Quasar;

public abstract class Shader : IDisposable
{
	public const string SHADER_FOLDER = "Shaders/";

	public abstract int PassNumber(Material material);

	public abstract void ApplyPass(int pass);

	public abstract void BeginRender(Transform motion, Material material);

	public abstract void BeginRender(Transform motion, Material material, string technique);

	public abstract bool HasTechnique(string technique);

	public void BeginRender(Element element, Material material)
	{
		BeginRender(element.Transform, material);
	}

	public abstract void EndRender();

	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	~Shader()
	{
		Dispose();
	}
}
