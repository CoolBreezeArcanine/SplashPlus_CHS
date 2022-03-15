using System.Collections.Generic;
using System.Linq;

namespace Manager
{
	public class NoteDataList : List<NoteData>
	{
		public bool Empty()
		{
			return !this.Any();
		}
	}
}
