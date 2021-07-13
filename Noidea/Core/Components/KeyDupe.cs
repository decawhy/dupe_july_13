using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Noidea.Models;
using Noidea.Services;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000023 RID: 35
	public class KeyDupe : Component
	{
		// Token: 0x060000CD RID: 205 RVA: 0x000058B0 File Offset: 0x00003AB0
		public static string GetServer()
		{
			string[] servers = KeyDupe.Servers;
			int num;
			if (KeyDupe.ServerIndex < KeyDupe.Servers.Length)
			{
				KeyDupe.ServerIndex = (num = KeyDupe.ServerIndex) + 1;
			}
			else
			{
				num = (KeyDupe.ServerIndex = 0);
			}
			return servers[num];
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00002807 File Offset: 0x00000A07
		public string BuildVersion { get; } = "1.3.2.1.0";

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000CF RID: 207 RVA: 0x0000280F File Offset: 0x00000A0F
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x00002817 File Offset: 0x00000A17
		public int GameId { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00002820 File Offset: 0x00000A20
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x00002828 File Offset: 0x00000A28
		public byte[] Key { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00002831 File Offset: 0x00000A31
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00002839 File Offset: 0x00000A39
		public int KeyTime { get; set; }

		// Token: 0x060000D5 RID: 213 RVA: 0x000058EC File Offset: 0x00003AEC
		public KeyDupe(bool setup, Client ghostClient)
		{
			this.GameId = -2;
			this.Key = null;
			this.KeyTime = -1;
			this.Setup = setup;
			this.GhostClient = ghostClient;
			bool flag = !setup;
			if (flag)
			{
				this.StateKey = StateKey.Duping;
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00005954 File Offset: 0x00003B54
		public override void Initialize()
		{
			this.Core = base.Client.GetComponent<CoreLoop>();
			this.World = base.Client.GetComponent<World>();
			base.Client.Connection.OnConnect += this.Connection_OnConnect;
			base.Client.Connection.OnDisconnect += this.Connection_OnDisconnect;
			base.Hook<MapInfo>(new Action<MapInfo>(this.OnMapInfo));
			base.Hook<Reconnect>(new Action<Reconnect>(this.OnReconnect));
			base.Hook<Update>(new Action<Update>(this.OnUpdate));
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
			base.Hook<VaultUpdate>(new Action<VaultUpdate>(this.OnVaultUpdate));
			base.Hook<QuestRedeemResponse>(delegate(QuestRedeemResponse response)
			{
				base.Client.Log("Response: " + response.Message + " | " + response.Ok.ToString());
				this.Destroyed = true;
				base.Client.Connection.Close();
				this.GhostClient.GetComponent<KeyDupe>().Destroyed = true;
				Task.Delay(1000).ContinueWith(delegate(Task t)
				{
					this.GhostClient.Connection.Close();
				});
				Task.Delay(2000).ContinueWith(delegate(Task t)
				{
					Account account = new Account
					{
						CharId = base.Client.Account.CharId,
						Email = base.Client.Account.Email,
						Password = base.Client.Account.Password,
						Secret = base.Client.Account.Secret,
						Server = new Server
						{
							Address = KeyDupe.GetServer(),
							Name = "Unknown"
						}
					};
					Client client = new Client(account);
					client.AddComponent<World>(Array.Empty<object>());
					client.AddComponent<CoreLoop>(Array.Empty<object>());
					Client client2 = client;
					object[] array = new object[2];
					array[0] = true;
					client2.AddComponent<KeyDupe>(array);
					client.Connect();
				});
			});
			base.Hook<Failure>(new Action<Failure>(this.OnFailure));
			base.Hook<CreateSuccess>(new Action<CreateSuccess>(this.OnCreateSuccess));
			base.Hook<Text>(delegate(Text text)
			{
				base.Client.Log("Message: " + text.Message);
			});
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00005A6C File Offset: 0x00003C6C
		private void OnFailure(Failure obj)
		{
			bool flag = obj.ErrorDescription.Contains("More than one instance");
			if (flag)
			{
				base.Client.Account.Server = new Server
				{
					Address = KeyDupe.GetServer(),
					Name = "Unknown"
				};
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00005AC0 File Offset: 0x00003CC0
		private void OnCreateSuccess(CreateSuccess success)
		{
			this.Vault = null;
			base.Client.Log("Success!");
			base.Client.ObjectId = success.ObjectId;
			base.Client.Account.CharId = success.CharId;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00005B10 File Offset: 0x00003D10
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

		// Token: 0x060000DA RID: 218 RVA: 0x00005B58 File Offset: 0x00003D58
		private void OnNewTick(NewTick obj)
		{
			bool flag = obj.TickId == 2;
			if (flag)
			{
				bool flag2 = this.StateKey == StateKey.GettingStuff;
				if (flag2)
				{
					bool flag3 = this.World.Name == "Nexus";
					if (flag3)
					{
						for (int i = 0; i < 3; i++)
						{
							InvSwap packet = new InvSwap
							{
								Position = base.Client.Position,
								SlotObject1 = new SlotObjectData
								{
									ObjectId = base.Client.ObjectId,
									ObjectType = base.Client.Inventory[12 + i],
									SlotId = 12 + i
								},
								SlotObject2 = new SlotObjectData
								{
									ObjectId = base.Client.ObjectId,
									ObjectType = -1,
									SlotId = 4 + i
								},
								Time = this.Core.Time
							};
							base.Client.Send(packet);
							Thread.Sleep(550);
						}
						this.StateKey = StateKey.GhostClienting;
						base.Client.Send(new GoToQuestRoom());
					}
					else
					{
						bool flag4 = this.World.Name == "Daily Quest Room";
						if (flag4)
						{
							base.Client.Send(new UsePortal
							{
								ObjectId = this.VaultPortal
							});
						}
					}
				}
				else
				{
					bool flag5 = this.StateKey == StateKey.GhostClienting && this.World.Name == "Daily Quest Room";
					if (flag5)
					{
						base.Client.Send(new QuestFetchAsk());
						ClaimDailyReward packet2 = new ClaimDailyReward
						{
							ClaimKey = "",
							ClaimType = ""
						};
						for (int j = 0; j < 750; j++)
						{
							base.Client.Send(packet2);
						}
						base.Client.Log("Ghosting!");
						this.StateKey = StateKey.Ghosted;
						Task.Delay(120000).ContinueWith(delegate(Task t)
						{
							this.StateKey = StateKey.GhostingDone;
							base.Client.Log("Ghost client Done!");
							Account account = new Account
							{
								CharId = base.Client.Account.CharId,
								Email = base.Client.Account.Email,
								Password = base.Client.Account.Password,
								Secret = base.Client.Account.Secret,
								Server = new Server
								{
									Address = KeyDupe.GetServer(),
									Name = "Unknown"
								}
							};
							Client client = new Client(account);
							client.AddComponent<World>(Array.Empty<object>());
							client.AddComponent<CoreLoop>(Array.Empty<object>());
							client.AddComponent<KeyDupe>(new object[]
							{
								false,
								base.Client
							});
							this.GhostClient = client;
							client.Connect();
						});
					}
					else
					{
						bool flag6 = this.StateKey == StateKey.Duping;
						if (flag6)
						{
							bool flag7 = this.World.Name == "Daily Quest Room";
							if (flag7)
							{
								base.Client.Send(new UsePortal
								{
									ObjectId = this.VaultPortal
								});
							}
							else
							{
								bool flag8 = this.World.Name == "Vault";
								if (flag8)
								{
									this.GhostClient.GetComponent<CoreLoop>().Target = new WorldPosData(20f, 6.5f);
									Task.Delay(3000).ContinueWith(delegate(Task k)
									{
										for (int m = 0; m < 3; m++)
										{
											InvSwap packet4 = new InvSwap
											{
												Position = base.Client.Position,
												SlotObject1 = new SlotObjectData
												{
													ObjectId = base.Client.ObjectId,
													ObjectType = base.Client.Inventory[4 + m],
													SlotId = 4 + m
												},
												SlotObject2 = new SlotObjectData
												{
													ObjectId = this.VaultChest,
													ObjectType = -1,
													SlotId = m
												},
												Time = this.Core.Time
											};
											base.Client.Send(packet4);
											Thread.Sleep(550);
										}
										base.Client.Log("Dupe is done");
										this.StateKey = StateKey.Nil;
									});
								}
								else
								{
									bool flag9 = this.World.Name == "Nexus";
									if (flag9)
									{
										this.GhostClient.GetComponent<KeyDupe>().Claim();
										for (int l = 0; l < 3; l++)
										{
											InvSwap packet3 = new InvSwap
											{
												Position = base.Client.Position,
												SlotObject1 = new SlotObjectData
												{
													ObjectId = base.Client.ObjectId,
													ObjectType = base.Client.Inventory[4 + l],
													SlotId = 4 + l
												},
												SlotObject2 = new SlotObjectData
												{
													ObjectId = base.Client.ObjectId,
													ObjectType = -1,
													SlotId = 12 + l
												},
												Time = this.Core.Time
											};
											base.Client.Send(packet3);
											Thread.Sleep(550);
										}
										base.Client.Log("Dupe is done");
										this.StateKey = StateKey.Nil;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00005F48 File Offset: 0x00004148
		public void Claim()
		{
			Console.WriteLine("T: " + string.Join<int>(",", base.Client.Inventory) + " | " + base.Client.ObjectId.ToString());
			base.Client.Send(new QuestRedeem
			{
				Item = -1,
				QuestId = "6038591765086208",
				Slots = new SlotObjectData[]
				{
					new SlotObjectData
					{
						ObjectId = base.Client.ObjectId,
						ObjectType = 2613,
						SlotId = 4
					},
					new SlotObjectData
					{
						ObjectId = base.Client.ObjectId,
						ObjectType = 2613,
						SlotId = 5
					},
					new SlotObjectData
					{
						ObjectId = base.Client.ObjectId,
						ObjectType = 2613,
						SlotId = 6
					}
				}
			});
			base.Client.Log("Redeem sent.." + this.World.Name);
			this.StateKey = StateKey.Nil;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00006070 File Offset: 0x00004270
		private void OnUpdate(Update update)
		{
			bool flag = this.World.Name == "Daily Quest Room";
			if (flag)
			{
				foreach (ObjectData objectData in update.NewObjects)
				{
					bool flag2 = objectData.Status.ObjectId == base.Client.ObjectId;
					if (flag2)
					{
						base.Client.Position = new WorldPosData(15.5f, 9.5f);
					}
					else
					{
						bool flag3 = objectData.ObjectType == 1824;
						if (flag3)
						{
							this.VaultPortal = objectData.Status.ObjectId;
						}
					}
				}
			}
			else
			{
				bool flag4 = this.World.Name == "Vault";
				if (flag4)
				{
					foreach (ObjectData objectData2 in update.NewObjects)
					{
						bool flag5 = objectData2.Status.ObjectId == base.Client.ObjectId;
						if (flag5)
						{
							base.Client.Position = new WorldPosData(40.5f, 66.5f);
						}
						else
						{
							bool flag6 = objectData2.ObjectType == 1284;
							if (flag6)
							{
								this.VaultChest = objectData2.Status.ObjectId;
							}
							else
							{
								bool flag7 = objectData2.ObjectType == 5974;
								if (flag7)
								{
									this.DailyPortal = objectData2.Status.ObjectId;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00002842 File Offset: 0x00000A42
		private void OnReconnect(Reconnect recon)
		{
			this.Key = recon.Key;
			this.KeyTime = recon.KeyTime;
			this.GameId = recon.GameId;
			base.Client.Connection.Close();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000061FC File Offset: 0x000043FC
		private void OnMapInfo(MapInfo info)
		{
			base.Client.Log("Connecting to " + info.Name);
			base.Client.Send(new Load
			{
				CharId = base.Client.Account.CharId,
				IsChallenger = false,
				IsFromArena = false
			});
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000625C File Offset: 0x0000445C
		private void Connection_OnDisconnect()
		{
			base.Client.Log("Disconnected");
			bool destroyed = this.Destroyed;
			if (!destroyed)
			{
				Task.Delay(1000).ContinueWith(delegate(Task t)
				{
					base.Client.Connect();
				});
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000062A4 File Offset: 0x000044A4
		private void Connection_OnConnect()
		{
			base.Client.Log("Connected");
			base.Client.Send(new Hello
			{
				BuildVersion = this.BuildVersion,
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

		// Token: 0x0400008D RID: 141
		public static string[] Servers = new string[]
		{
			"18.184.202.127",
			"18.159.133.120"
		};

		// Token: 0x0400008E RID: 142
		public static int ServerIndex = 0;

		// Token: 0x0400008F RID: 143
		public const string ClientToken = "8bV53M5ysJdVjU4M97fh2g7BnPXhefnc";

		// Token: 0x04000094 RID: 148
		public bool Setup;

		// Token: 0x04000095 RID: 149
		public Client GhostClient;

		// Token: 0x04000096 RID: 150
		public CoreLoop Core;

		// Token: 0x04000097 RID: 151
		public World World;

		// Token: 0x04000098 RID: 152
		public StateKey StateKey;

		// Token: 0x04000099 RID: 153
		public int VaultPortal;

		// Token: 0x0400009A RID: 154
		public int DailyPortal;

		// Token: 0x0400009B RID: 155
		public int VaultChest;

		// Token: 0x0400009C RID: 156
		public int[] Vault;

		// Token: 0x0400009D RID: 157
		public int Increment = 0;

		// Token: 0x0400009E RID: 158
		public bool Destroyed = false;
	}
}
