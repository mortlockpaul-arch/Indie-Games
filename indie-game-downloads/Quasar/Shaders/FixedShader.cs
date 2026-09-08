using System.Collections.Generic;
using Quasar.Shaders.Fixed;

namespace Quasar.Shaders;

public static class FixedShader
{
	private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

	public static void RegisterShader(string name, Shader shader)
	{
		shaders[name] = shader;
	}

	public static Shader GetShader(string name)
	{
		if (shaders.TryGetValue(name, out var value))
		{
			return value;
		}
		return name switch
		{
			"SimpleAdd" => new SimpleAddShader(), 
			"GUI" => new GUIShader(), 
			"Particles/BillboardParticleAlpha" => new ParticleAlphaShader(), 
			"Particles/BillboardParticleAdditive" => new ParticleAdditiveShader(), 
			"Particles/BillboardParticleMultiply" => new ParticleMultiplyShader(), 
			"SimpleVertexColor" => new SimpleVertexColorShader(), 
			"VertexColor" => new VertexColorShader(), 
			"Instancing" => new InstancedShader(), 
			_ => new SimpleShader(), 
		};
	}
}
