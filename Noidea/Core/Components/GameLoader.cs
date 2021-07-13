using System;
using System.Net;
using System.Threading.Tasks;
using Noidea.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x0200001F RID: 31
	public class GameLoader : Component
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x0000266E File Offset: 0x0000086E
		public string BuildVersion { get; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00002676 File Offset: 0x00000876
		public Classes DefaultClass { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000267E File Offset: 0x0000087E
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00002686 File Offset: 0x00000886
		public int GameId { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000268F File Offset: 0x0000088F
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00002697 File Offset: 0x00000897
		public byte[] Key { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000026A0 File Offset: 0x000008A0
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x000026A8 File Offset: 0x000008A8
		public int KeyTime { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000BA RID: 186 RVA: 0x000026B1 File Offset: 0x000008B1
		// (set) Token: 0x060000BB RID: 187 RVA: 0x000026B9 File Offset: 0x000008B9
		public bool Destroyed { get; set; } = false;

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000BC RID: 188 RVA: 0x000026C2 File Offset: 0x000008C2
		// (set) Token: 0x060000BD RID: 189 RVA: 0x000026CA File Offset: 0x000008CA
		public int CharId { get; set; }

		// Token: 0x060000BE RID: 190 RVA: 0x000026D3 File Offset: 0x000008D3
		public GameLoader(string buildVersion, int gameId, int charId, Classes defaultClass)
		{
			this.BuildVersion = buildVersion;
			this.GameId = gameId;
			this.CharId = charId;
			this.DefaultClass = defaultClass;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000270A File Offset: 0x0000090A
		public GameLoader(string buildVersion, int gameId, int charId) : this(buildVersion, gameId, charId, Classes.Wizard)
		{
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00005470 File Offset: 0x00003670
		public override void Initialize()
		{
			base.Client.Connection.OnConnect += this.Connection_OnConnect;
			base.Client.Connection.OnDisconnect += this.Connection_OnDisconnect;
			base.Hook<MapInfo>(new Action<MapInfo>(this.OnMapInfo));
			base.Hook<CreateSuccess>(new Action<CreateSuccess>(this.OnCreateSuccess));
			base.Hook<Reconnect>(new Action<Reconnect>(this.OnReconnect));
			base.Hook<Failure>(new Action<Failure>(this.OnFailure));
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00005504 File Offset: 0x00003704
		private void OnFailure(Failure obj)
		{
			base.Client.Log(obj.ErrorId);
			bool flag = obj.ErrorDescription.ToLower() == "character not found";
			if (flag)
			{
				this.CharId = 0;
			}
			else
			{
				bool flag2 = obj.ErrorDescription.ToLower() == "character is dead";
				if (flag2)
				{
					int charId = this.CharId;
					this.CharId = charId + 1;
				}
				else
				{
					bool flag3 = obj.ErrorDescription == "Sorry, server is full [002]";
					if (flag3)
					{
						this.Key = null;
						this.KeyTime = -1;
						this.GameId = -2;
					}
					else
					{
						bool flag4 = obj.ErrorDescription.ToLower().Contains("migration");
						if (flag4)
						{
							using (WebClient webClient = new WebClient())
							{
								webClient.DownloadString("https://www.realmofthemadgod.com/char/list?guid=" + base.Client.Account.Email + "&password=" + base.Client.Account.Password);
							}
						}
					}
				}
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000271C File Offset: 0x0000091C
		private void OnReconnect(Reconnect recon)
		{
			this.Key = recon.Key;
			this.KeyTime = recon.KeyTime;
			this.GameId = recon.GameId;
			base.Client.Connection.Close();
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00005630 File Offset: 0x00003830
		private void Connection_OnDisconnect()
		{
			base.Client.Log("Disconnected");
			bool flag = !this.Destroyed;
			if (flag)
			{
				Task.Delay(this.Slow ? 3000 : 300).ContinueWith(delegate(Task t)
				{
					base.Client.Connect();
				});
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00005688 File Offset: 0x00003888
		private void Connection_OnConnect()
		{
			base.Client.Log("Connected");
			base.Client.Send(new Hello
			{
				BuildVersion = this.BuildVersion,
				Guid = base.Client.AccessToken,
				Key = (this.Key ?? new byte[0]),
				EntryTag = "",
				GameId = this.GameId,
				GameNet = (GameLoader.Steam ? "steam" : "rotmg"),
				GameNetUserId = "",
				KeyTime = this.KeyTime,
				LastGuid = "",
				MapJSON = "",
				PlatformToken = (GameLoader.Steam ? "4893407152832512-2.8dbfeeb5e3e7e555f97e3907980cfec3" : ""),
				PlayPlatform = (GameLoader.Steam ? "steam" : "rotmg"),
				UserToken = "f6ff89ce8d0498bcfcbdc836072ba9d5119789cc",
				Token = "8bV53M5ysJdVjU4M97fh2g7BnPXhefnc"
			});
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00002757 File Offset: 0x00000957
		private void OnCreateSuccess(CreateSuccess success)
		{
			base.Client.Log("Success!");
			base.Client.ObjectId = success.ObjectId;
			this.CharId = success.CharId;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00005794 File Offset: 0x00003994
		private void OnMapInfo(MapInfo info)
		{
			base.Client.Log("Connecting to " + info.Name);
			this.GameId = -2;
			this.Key = null;
			this.KeyTime = -1;
			bool flag = info.Name == "Nexus";
			if (flag)
			{
				Task.Delay(this.Slow ? 2000 : 100).ContinueWith(delegate(Task t)
				{
					base.Client.Send(new GoToQuestRoom());
				});
			}
			else
			{
				bool flag2 = this.CharId == 0;
				if (flag2)
				{
					base.Client.Send(new Create
					{
						ClassType = (short)this.DefaultClass,
						IsChallenger = false,
						SkinType = 0
					});
				}
				else
				{
					base.Client.Send(new Load
					{
						CharId = this.CharId,
						IsChallenger = false,
						IsFromArena = false
					});
				}
			}
		}

		// Token: 0x04000078 RID: 120
		public const string FallbackServer = "35.180.73.63";

		// Token: 0x04000079 RID: 121
		public const string ClientToken = "8bV53M5ysJdVjU4M97fh2g7BnPXhefnc";

		// Token: 0x0400007A RID: 122
		public const bool VaultMode = true;

		// Token: 0x0400007B RID: 123
		public static bool Steam;

		// Token: 0x04000083 RID: 131
		public bool Slow = false;
	}
}
