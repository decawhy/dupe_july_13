using System;

namespace Noidea.Models
{
	// Token: 0x0200000C RID: 12
	public class Account
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002143 File Offset: 0x00000343
		// (set) Token: 0x0600001F RID: 31 RVA: 0x0000214B File Offset: 0x0000034B
		public string Email { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002154 File Offset: 0x00000354
		// (set) Token: 0x06000021 RID: 33 RVA: 0x0000215C File Offset: 0x0000035C
		public string Password { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002165 File Offset: 0x00000365
		// (set) Token: 0x06000023 RID: 35 RVA: 0x0000216D File Offset: 0x0000036D
		public string Secret { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002176 File Offset: 0x00000376
		// (set) Token: 0x06000025 RID: 37 RVA: 0x0000217E File Offset: 0x0000037E
		public int CharId { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002187 File Offset: 0x00000387
		// (set) Token: 0x06000027 RID: 39 RVA: 0x0000218F File Offset: 0x0000038F
		public Server Server { get; set; }
	}
}
