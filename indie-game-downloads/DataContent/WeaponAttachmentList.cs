using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.WeaponAttachmentList, DataContent")]
public class WeaponAttachmentList
{
	public WeaponType weaponType;

	public List<WeaponAttachment> availableAttachments = new List<WeaponAttachment>();
}
