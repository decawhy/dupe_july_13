using System;
using Noidea.Services;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x0200001C RID: 28
	public class DupeSlicer : Component
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x00005194 File Offset: 0x00003394
		public override void Initialize()
		{
			base.Client.Connection.OnConnect += this.Connection_OnConnect;
			base.Client.Connection.OnDisconnect += this.Connection_OnDisconnect;
			base.Hook<MapInfo>(new Action<MapInfo>(this.OnMapInfo));
			base.Hook<Failure>(new Action<Failure>(this.OnFailure));
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00002610 File Offset: 0x00000810
		private void Connection_OnDisconnect()
		{
			base.Client.Connection.Connect();
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00002624 File Offset: 0x00000824
		private void OnFailure(Failure fail)
		{
			Console.WriteLine(fail.ErrorDescription);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005204 File Offset: 0x00003404
		private void OnMapInfo(MapInfo info)
		{
			Load packet = new Load
			{
				CharId = 148,
				IsChallenger = false,
				IsFromArena = false
			};
			for (int i = 0; i < 50; i++)
			{
				base.Client.Send(packet);
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00005254 File Offset: 0x00003454
		private void Connection_OnConnect()
		{
			base.Client.Log("Connected");
			base.Client.Send(new Hello
			{
				BuildVersion = "1.3.0.2.0",
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
		}
	}
}
