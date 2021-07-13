using System;
using System.Threading.Tasks;
using Noidea.Services;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000017 RID: 23
	public class BertsDupe : Component
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00002440 File Offset: 0x00000640
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00002448 File Offset: 0x00000648
		public int GameId { get; set; } = -2;

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00002451 File Offset: 0x00000651
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00002459 File Offset: 0x00000659
		public byte[] Key { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00002462 File Offset: 0x00000662
		// (set) Token: 0x06000084 RID: 132 RVA: 0x0000246A File Offset: 0x0000066A
		public int KeyTime { get; set; }

		// Token: 0x06000085 RID: 133 RVA: 0x00004868 File Offset: 0x00002A68
		public override void Initialize()
		{
			base.Client.Connection.OnConnect += this.Connection_OnConnect;
			base.Client.Connection.OnDisconnect += this.Connection_OnDisconnect;
			base.Hook<MapInfo>(new Action<MapInfo>(this.OnMapInfo));
			base.Hook<CreateSuccess>(new Action<CreateSuccess>(this.OnCreateSuccess));
			base.Hook<Reconnect>(new Action<Reconnect>(this.OnReconnect));
			base.Hook<Text>(delegate(Text text)
			{
				base.Client.Log(text.Message);
			});
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00002473 File Offset: 0x00000673
		private void OnCreateSuccess(CreateSuccess success)
		{
			base.Client.Log("Success!");
			base.Client.ObjectId = success.ObjectId;
			base.Client.Account.CharId = success.CharId;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000048FC File Offset: 0x00002AFC
		private void OnMapInfo(MapInfo info)
		{
			base.Client.Log("Connecting to " + info.Name + "!");
			bool flag = info.Name == "Nexus";
			if (flag)
			{
				this.Delayed(delegate
				{
					base.Client.Send(new GoToQuestRoom());
				}, 500);
			}
			else
			{
				bool flag2 = info.Name == "Daily Quest Room";
				if (flag2)
				{
					this.Destroyed = true;
					for (int i = 0; i < 500; i++)
					{
						base.Client.Send(new Load
						{
							CharId = 1,
							IsChallenger = false
						});
						base.Client.Send(new ClaimDailyReward
						{
							ClaimKey = "",
							ClaimType = ""
						});
					}
				}
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000049D4 File Offset: 0x00002BD4
		private void OnReconnect(Reconnect recon)
		{
			Console.WriteLine("Recon: " + recon.Name);
			this.Key = recon.Key;
			this.KeyTime = recon.KeyTime;
			this.GameId = recon.GameId;
			base.Client.Connection.Close();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004A30 File Offset: 0x00002C30
		private void Connection_OnDisconnect()
		{
			Console.WriteLine("Disconnect");
			bool flag = !this.Destroyed;
			if (flag)
			{
				this.Delayed(delegate
				{
					base.Client.Connect();
				}, 1000);
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004A70 File Offset: 0x00002C70
		private void Connection_OnConnect()
		{
			base.Client.Log("Connected");
			base.Client.Send(new Hello
			{
				BuildVersion = "1.3.2.1.0",
				Guid = RSA.Instance.Encrypt(base.Client.Account.Email ?? ""),
				Password = RSA.Instance.Encrypt(base.Client.Account.Password ?? ""),
				Key = (this.Key ?? new byte[0]),
				EntryTag = "",
				GameId = this.GameId,
				GameNet = "rotmg",
				GameNetUserId = "",
				KeyTime = this.KeyTime,
				LastGuid = "",
				MapJSON = "",
				PlatformToken = "",
				PlayPlatform = "rotmg",
				ClientToken = "8bV53M5ysJdVjU4M97fh2g7BnPXhefnc",
				UserToken = "",
				Secret = RSA.Instance.Encrypt(base.Client.Account.Secret ?? "")
			});
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004BBC File Offset: 0x00002DBC
		private void Delayed(Action action, int delay)
		{
			Task.Delay(delay).ContinueWith(delegate(Task t)
			{
				try
				{
					action();
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
				}
			});
		}

		// Token: 0x04000062 RID: 98
		public const string BuildVersion = "1.3.2.1.0";

		// Token: 0x04000063 RID: 99
		public const string ClientToken = "8bV53M5ysJdVjU4M97fh2g7BnPXhefnc";

		// Token: 0x04000067 RID: 103
		private bool Destroyed = false;
	}
}
