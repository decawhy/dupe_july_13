using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Noidea.Models;

namespace Noidea.Services
{
	// Token: 0x02000007 RID: 7
	public class AccountService
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000020B7 File Offset: 0x000002B7
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000020BF File Offset: 0x000002BF
		public List<Account> Accounts { get; set; }

		// Token: 0x0600000D RID: 13 RVA: 0x000020C8 File Offset: 0x000002C8
		public AccountService(string path)
		{
			this.Accounts = this.LoadAccounts(path);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00003540 File Offset: 0x00001740
		private List<Account> LoadAccounts(string path)
		{
			return JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(path), null);
		}
	}
}
