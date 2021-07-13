using System;
using Noidea.Services;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x0200002F RID: 47
	public class redacted : Component
	{
		// Token: 0x0600013D RID: 317 RVA: 0x00002AE8 File Offset: 0x00000CE8
		public redacted(int accountIndex)
		{
			this.RankId = accountIndex;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00008938 File Offset: 0x00006B38
		public override void Initialize()
		{
			base.Client.Connection.OnConnect += this.Connection_OnConnect;
			base.Client.Connection.OnDisconnect += this.Connection_OnDisconnect;
			base.Hook<CreateSuccess>(new Action<CreateSuccess>(this.OnCreateSuccess));
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00008994 File Offset: 0x00006B94
		private void OnCreateSuccess(CreateSuccess obj)
		{
			Console.WriteLine(string.Format("[{0}]: Sending", this.RankId));
			base.Client.Send(new ChangeGuildRank
			{
				Rank = redacted.Random.Next(29),
				Name = "Stmuaaac"
			});
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000278A File Offset: 0x0000098A
		private void Connection_OnDisconnect()
		{
			base.Client.Connect();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x000089EC File Offset: 0x00006BEC
		private void Connection_OnConnect()
		{
			base.Client.Send(new Hello
			{
				BuildVersion = "1.3.0.1.0",
				Guid = RSA.Instance.Encrypt(base.Client.Account.Email ?? ""),
				Password = RSA.Instance.Encrypt(base.Client.Account.Password ?? ""),
				Key = new byte[0],
				EntryTag = "",
				GameId = -2,
				GameNet = "rotmg",
				GameNetUserId = "",
				KeyTime = -1,
				LastGuid = "",
				MapJSON = "",
				PlatformToken = "",
				PlayPlatform = "rotmg",
				ClientToken = "8bV53M5ysJdVjU4M97fh2g7BnPXhefnc",
				UserToken = "",
				Secret = RSA.Instance.Encrypt(base.Client.Account.Secret ?? "")
			});
			base.Client.Send(new Load
			{
				CharId = base.Client.Account.CharId,
				IsChallenger = false,
				IsFromArena = false
			});
			base.Client.Send(new Create
			{
				ClassType = -1,
				IsChallenger = false,
				SkinType = -1
			});
		}

		// Token: 0x040000DB RID: 219
		public static Random Random = new Random();

		// Token: 0x040000DC RID: 220
		public int RankId;
	}
}
