using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class ColorChannel : ChannelEffect
{
	private Vector4 zeroCol;

	private Vector4 oneCol;

	private Vector3 zCol3;

	private Vector3 oCol3;

	public ColorChannel(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		zeroCol = LevelLoader.GetVector4FromAtt(attributes, "col1");
		oneCol = LevelLoader.GetVector4FromAtt(attributes, "col2");
		zCol3 = new Vector3(zeroCol.X, zeroCol.Y, zeroCol.Z);
		oCol3 = new Vector3(oneCol.X, oneCol.Y, oneCol.Z);
	}

	public override void Update(GameTime gametime)
	{
		base.Update(gametime);
	}

	public override void Draw(GameTime gametime, float curVal, ref EffectParameterCollectionRedux[] _toUpdate)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.SetAllEPCs(_toUpdate, "DiffuseColor", curVal * oCol3 + (1f - curVal) * zCol3);
		base.Draw(gametime, curVal, ref _toUpdate);
	}
}
