using System.Collections.Generic;

namespace BureauNewPDA.Data;

public class RefDumpClass
{
	public class VideoData
	{
		public enum VideoPlayType
		{
			ConversationPause
		}

		public int refId { get; set; }

		public string refName { get; set; }

		public string refGroupName { get; set; }

		public VideoPlayType videoPlayType { get; set; }

		public bool isLoop { get; set; }
	}

	public List<VideoData> myDumpList = new List<VideoData>();
}
