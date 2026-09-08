using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.AttachmentList, DataContent")]
public class AttachmentList
{
	public List<WeaponAttachment> attachments = new List<WeaponAttachment>();
}
