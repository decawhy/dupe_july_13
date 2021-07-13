using System;

namespace Noidea.API.Callbacks
{
	// Token: 0x0200004C RID: 76
	internal class Subscribe : Attribute
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000214 RID: 532 RVA: 0x000031D6 File Offset: 0x000013D6
		public string Path { get; }

		// Token: 0x06000215 RID: 533 RVA: 0x000031DE File Offset: 0x000013DE
		public Subscribe(string path)
		{
			this.Path = path;
		}
	}
}
