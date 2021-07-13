using System;

namespace Noidea.Services
{
	// Token: 0x0200000A RID: 10
	public class NameService
	{
		// Token: 0x06000018 RID: 24 RVA: 0x000039A4 File Offset: 0x00001BA4
		public static string GenerateName(int len)
		{
			string text = "";
			for (int i = 0; i < len; i++)
			{
				text += ((char)(97 + NameService.Random.Next(26))).ToString();
			}
			return text;
		}

		// Token: 0x04000015 RID: 21
		public static Random Random = new Random();
	}
}
