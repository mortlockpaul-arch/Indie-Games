using Microsoft.Xna.Framework.Input;
using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_KEY : CParam
{
	public Keys key;

	public short mouseKey;

	public override void load(CRunApp app)
	{
		short pcKey = app.file.readAShort();
		key = CKeyConvert.getXnaKey(pcKey);
		mouseKey = pcKey;
	}
}
