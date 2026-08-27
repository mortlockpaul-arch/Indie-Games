namespace MaxScriptDefines;

public struct PredefinedEmitterStruct(string n, int i, bool enabledE, int emitterE, float scaleE)
{
	public string name = n;

	public int typeIndex = i;

	public bool isEnabled = enabledE;

	public int emitterType = emitterE;

	public float emitterScale = scaleE;

	public void SetParameters(bool enabled, int emitter, float scale)
	{
		isEnabled = enabled;
		emitterType = emitter;
		emitterScale = scale;
	}
}
