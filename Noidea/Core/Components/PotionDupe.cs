using System;
using System.Linq;
using System.Threading.Tasks;
using Noidea.Models;
using Noidea.Services;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x0200002D RID: 45
	public class PotionDupe : Component
	{
		// Token: 0x06000127 RID: 295 RVA: 0x00007F7C File Offset: 0x0000617C
		public PotionDupe(bool ghost)
		{
			this.Ghost = ghost;
			if (ghost)
			{
				this.State = PotState.Ghosted;
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00002A80 File Offset: 0x00000C80
		public PotionDupe() : this(false)
		{
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00007FDC File Offset: 0x000061DC
		public override void Initialize()
		{
			this.Core = base.Client.GetComponent<CoreLoop>();
			this.World = base.Client.GetComponent<World>();
			base.Client.Connection.OnConnect += this.Connection_OnConnect;
			base.Client.Connection.OnDisconnect += this.Connection_OnDisconnect;
			base.Hook<VaultUpdate>(new Action<VaultUpdate>(this.OnVaultUpdate));
			base.Hook<MapInfo>(new Action<MapInfo>(this.OnMapInfo));
			base.Hook<CreateSuccess>(new Action<CreateSuccess>(this.OnCreateSuccess));
			base.Hook<Reconnect>(new Action<Reconnect>(this.OnReconnect));
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
			base.Hook<Text>(new Action<Text>(this.OnText));
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000080B8 File Offset: 0x000062B8
		private void OnText(Text text)
		{
			bool flag = text.Message == "Internal error -- try again";
			if (flag)
			{
				bool flag2 = this.FirstInternalError == 0;
				if (flag2)
				{
					this.FirstInternalError = Environment.TickCount;
				}
				bool flag3 = this.LastInternalTime == 0;
				if (flag3)
				{
					this.LastInternalTime = Environment.TickCount;
				}
				this.InternalErrorCount++;
				this.LastInternalError = Environment.TickCount;
				this.CalculateRemainingSpams();
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00008130 File Offset: 0x00006330
		private void DailyQuestLag()
		{
			ClaimDailyReward packet = new ClaimDailyReward
			{
				ClaimKey = "",
				ClaimType = ""
			};
			this._claimsSent += this._claimSpamPer;
			this._lastClaimsSentTime = Environment.TickCount;
			for (int i = 0; i < this._claimSpamPer; i++)
			{
				base.Client.Send(packet);
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000819C File Offset: 0x0000639C
		public void SaveInventory()
		{
			for (int i = 0; i < 3; i++)
			{
				InvSwap packet = new InvSwap
				{
					Position = base.Client.Position,
					SlotObject1 = new SlotObjectData
					{
						ObjectId = base.Client.ObjectId,
						ObjectType = base.Client.Inventory[4 + i],
						SlotId = 4 + i
					},
					SlotObject2 = new SlotObjectData
					{
						ObjectId = base.Client.ObjectId,
						ObjectType = -1,
						SlotId = 8 + i
					},
					Time = this.Core.Time + this.Core.OffsetTime
				};
				base.Client.Send(packet);
				this.Core.OffsetTime += 550;
			}
			base.Client.Send(new Move
			{
				TickId = -1,
				NewPosition = base.Client.Position,
				RealTimeMS = 0,
				Records = new MoveRecord[0],
				Time = 0
			});
			this.Delayed(new Action(this.GhostClient.GetComponent<PotionDupe>().ClaimNow), 2000);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00002633 File Offset: 0x00000833
		public void ClaimNow()
		{
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00002A8B File Offset: 0x00000C8B
		private void ClaimDupe()
		{
			base.Client.Log("Tryna claim.");
			this.GhostClient.GetComponent<PotionDupe>().SaveInventory();
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000082E8 File Offset: 0x000064E8
		private void CalculateRemainingSpams()
		{
			bool flag = this.InternalErrorCount < 10;
			if (!flag)
			{
				int num = this.Start + 120000 + 2000 - Environment.TickCount;
				bool flag2 = num < 0;
				if (flag2)
				{
					this._claimSpamPer = 0;
					this._claimSpamTotal = 0;
					this._claimSpamDelay = 0;
					bool flag3 = this.InternalErrorCount >= this._claimsSent;
					if (flag3)
					{
						base.Client.Log("Spam stopped");
						this.Delayed(new Action(this.ClaimDupe), 1000);
					}
				}
				else
				{
					double num2 = (double)(this.LastInternalError - this.FirstInternalError) / (double)this.InternalErrorCount;
					double a = (double)num / num2;
					this._claimSpamTotal = (int)Math.Ceiling(a);
					base.Client.Log("Total: " + this._claimSpamTotal.ToString());
				}
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x000083D4 File Offset: 0x000065D4
		private void Enter()
		{
			base.Client.Log("Entering with ghost client");
			Client client = new Client(new Account
			{
				CharId = base.Client.Account.CharId,
				Password = base.Client.Account.Password,
				Email = base.Client.Account.Email,
				Secret = base.Client.Account.Secret,
				Server = new Server
				{
					Address = KeyDupe.GetServer(),
					Name = "Unknown"
				}
			});
			client.AddComponent<World>(Array.Empty<object>());
			client.AddComponent<CoreLoop>(Array.Empty<object>());
			client.AddComponent<PotionDupe>(new object[]
			{
				true
			});
			client.GetComponent<PotionDupe>().GhostClient = base.Client;
			this.GhostClient = client;
			client.Connect();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000084CC File Offset: 0x000066CC
		private void OnNewTick(NewTick tick)
		{
			bool flag = tick.TickId == 0;
			if (!flag)
			{
				bool flag2 = tick.TickId == 1;
				if (flag2)
				{
					bool flag3 = this.World.Name == "Daily Quest Room";
					if (flag3)
					{
						bool flag4 = this.State == PotState.Start;
						if (flag4)
						{
							this.Delayed(new Action(this.Enter), 122000);
							this.Start = Environment.TickCount;
							this.State = PotState.Ghosting;
							base.Client.Log("Starting to DDoS");
						}
					}
				}
				else
				{
					bool flag5 = this.State == PotState.Ghosting;
					if (flag5)
					{
						bool flag6 = Environment.TickCount - this._lastClaimsSentTime > this._claimSpamDelay && this._claimsSent < this._claimSpamTotal;
						if (flag6)
						{
							this.DailyQuestLag();
						}
						else
						{
							bool flag7 = this._claimsSent == this._claimSpamTotal;
							if (flag7)
							{
								base.Client.Log(string.Format("Took: {0}ms", Environment.TickCount - this.Start));
								this._claimsSent++;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000085FC File Offset: 0x000067FC
		private void OnCreateSuccess(CreateSuccess success)
		{
			this.Vault = null;
			base.Client.Log("Success!");
			base.Client.ObjectId = success.ObjectId;
			base.Client.Account.CharId = success.CharId;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000864C File Offset: 0x0000684C
		private void OnMapInfo(MapInfo info)
		{
			base.Client.Log("Connecting to " + info.Name + "!");
			bool flag = info.Name == "Nexus" && this.State == PotState.Start;
			if (flag)
			{
				this.Delayed(delegate
				{
					base.Client.Send(new GoToQuestRoom());
				}, 750);
			}
			else
			{
				base.Client.Send(new Load
				{
					CharId = base.Client.Account.CharId,
					IsChallenger = false,
					IsFromArena = false
				});
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00002AB0 File Offset: 0x00000CB0
		private void OnReconnect(Reconnect recon)
		{
			this.Key = recon.Key;
			this.KeyTime = recon.KeyTime;
			this.GameId = recon.GameId;
			base.Client.Connection.Close();
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000086F0 File Offset: 0x000068F0
		private void OnVaultUpdate(VaultUpdate update)
		{
			bool flag = this.Vault != null;
			if (flag)
			{
				this.Vault = this.Vault.Concat(update.VaultContests).ToArray<int>();
			}
			else
			{
				this.Vault = update.VaultContests;
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00008738 File Offset: 0x00006938
		private void Connection_OnDisconnect()
		{
			base.Client.Log("Disconnected");
			bool flag = !this.Destroyed;
			if (flag)
			{
				this.Delayed(delegate
				{
					base.Client.Connect();
				}, 1000);
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000877C File Offset: 0x0000697C
		private void Connection_OnConnect()
		{
			base.Client.Log("Connected");
			base.Client.Send(new Hello
			{
				BuildVersion = "1.3.1.0.0",
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

		// Token: 0x06000138 RID: 312 RVA: 0x000088C8 File Offset: 0x00006AC8
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

		// Token: 0x040000C5 RID: 197
		private const string BuildVersion = "1.3.1.0.0";

		// Token: 0x040000C6 RID: 198
		private World World;

		// Token: 0x040000C7 RID: 199
		private CoreLoop Core;

		// Token: 0x040000C8 RID: 200
		private int GameId = -2;

		// Token: 0x040000C9 RID: 201
		private byte[] Key;

		// Token: 0x040000CA RID: 202
		private int KeyTime;

		// Token: 0x040000CB RID: 203
		private int[] Vault;

		// Token: 0x040000CC RID: 204
		private PotState State = PotState.Start;

		// Token: 0x040000CD RID: 205
		public bool Destroyed = false;

		// Token: 0x040000CE RID: 206
		public Client GhostClient;

		// Token: 0x040000CF RID: 207
		private bool Ghost;

		// Token: 0x040000D0 RID: 208
		private int InternalErrorCount;

		// Token: 0x040000D1 RID: 209
		private int LastInternalError;

		// Token: 0x040000D2 RID: 210
		private int FirstInternalError;

		// Token: 0x040000D3 RID: 211
		private int Start;

		// Token: 0x040000D4 RID: 212
		private int LastInternalTime;

		// Token: 0x040000D5 RID: 213
		private int _claimsSent;

		// Token: 0x040000D6 RID: 214
		private int _lastClaimsSentTime;

		// Token: 0x040000D7 RID: 215
		private int _claimSpamPer = 100;

		// Token: 0x040000D8 RID: 216
		private int _claimSpamTotal = 3000;

		// Token: 0x040000D9 RID: 217
		private int _claimSpamDelay = 500;
	}
}
