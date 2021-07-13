using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noidea.Models;
using Noidea.Services;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000029 RID: 41
	public class PetFeed : Component
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00002964 File Offset: 0x00000B64
		public int PetInstanceId { get; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000FE RID: 254 RVA: 0x0000296C File Offset: 0x00000B6C
		public int FeedCount { get; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000FF RID: 255 RVA: 0x00002974 File Offset: 0x00000B74
		// (set) Token: 0x06000100 RID: 256 RVA: 0x0000297C File Offset: 0x00000B7C
		public PetFeedState State { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00002985 File Offset: 0x00000B85
		// (set) Token: 0x06000102 RID: 258 RVA: 0x0000298D File Offset: 0x00000B8D
		public int GameId { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00002996 File Offset: 0x00000B96
		// (set) Token: 0x06000104 RID: 260 RVA: 0x0000299E File Offset: 0x00000B9E
		public byte[] Key { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000105 RID: 261 RVA: 0x000029A7 File Offset: 0x00000BA7
		// (set) Token: 0x06000106 RID: 262 RVA: 0x000029AF File Offset: 0x00000BAF
		public int KeyTime { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000107 RID: 263 RVA: 0x000029B8 File Offset: 0x00000BB8
		// (set) Token: 0x06000108 RID: 264 RVA: 0x000029C0 File Offset: 0x00000BC0
		public int[] Vault { get; set; }

		// Token: 0x06000109 RID: 265 RVA: 0x000029C9 File Offset: 0x00000BC9
		public PetFeed(int petInstanceId, int feedCount, Client ghostClient)
		{
			this.PetInstanceId = petInstanceId;
			this.FeedCount = feedCount;
			this.GhostClient = ghostClient;
			this.State = ((this.GhostClient == null) ? PetFeedState.GettingItems : PetFeedState.Duping);
			this.ResetKey();
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00006F78 File Offset: 0x00005178
		public override void Initialize()
		{
			base.Client.Connection.OnConnect += this.Connection_OnConnect;
			base.Client.Connection.OnDisconnect += this.Connection_OnDisconnect;
			this.Core = base.Client.GetComponent<CoreLoop>();
			this.World = base.Client.GetComponent<World>();
			base.Hook<VaultUpdate>(new Action<VaultUpdate>(this.OnVaultUpdate));
			base.Hook<MapInfo>(new Action<MapInfo>(this.OnMapInfo));
			base.Hook<CreateSuccess>(new Action<CreateSuccess>(this.OnCreateSuccess));
			base.Hook<Reconnect>(new Action<Reconnect>(this.OnReconnect));
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
			base.Hook<Update>(new Action<Update>(this.OnUpdate));
			base.Hook<Failure>(delegate(Failure fail)
			{
				base.Client.Log("Error: " + fail.ErrorDescription);
				bool flag = fail.ErrorDescription.ToLower().Contains("server is full");
				if (flag)
				{
					this.Key = null;
					this.KeyTime = -1;
					this.GameId = -2;
				}
			});
			base.Hook<Text>(delegate(Text tex)
			{
				base.Client.Log("Message: " + tex.Message);
			});
			base.Hook<BuyResult>(delegate(BuyResult result)
			{
				base.Client.Log("Result: " + result.ResultString);
				bool flag = result.Result == 7;
				if (flag)
				{
					base.Client.Connection.Close();
					this.Delayed(delegate
					{
						this.GhostClient.Connection.Close();
					}, 6000);
					this.Delayed(delegate
					{
						bool flag2 = this.FeedCount == 0;
						if (flag2)
						{
							base.Client.Log("All done! Feeding is DONEzo!");
						}
						else
						{
							base.Client.Log("Restarting process.");
							KeyDupe.ServerIndex = 0;
							Account account = new Account
							{
								Email = base.Client.Account.Email,
								Password = base.Client.Account.Password,
								CharId = base.Client.Account.CharId,
								Server = new Server
								{
									Name = "EUNorth",
									Address = KeyDupe.GetServer()
								}
							};
							Client client = new Client(account);
							client.AddComponent<World>(Array.Empty<object>());
							client.AddComponent<CoreLoop>(Array.Empty<object>());
							Client client2 = client;
							object[] array = new object[3];
							array[0] = this.PetInstanceId;
							array[1] = this.FeedCount - 1;
							client2.AddComponent<PetFeed>(array);
							client.Connect();
						}
					}, 10000);
				}
			});
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00007090 File Offset: 0x00005290
		private void OnUpdate(Update update)
		{
			bool flag = this.World.Name == "Vault";
			if (flag)
			{
				foreach (ObjectData objectData in update.NewObjects)
				{
					bool flag2 = objectData.Status.ObjectId == base.Client.ObjectId;
					if (flag2)
					{
						base.Client.Position = new WorldPosData(40.5f, 66.5f);
					}
				}
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00007110 File Offset: 0x00005310
		private void GetInPortal(ushort type)
		{
			base.Client.Log(string.Format("Trying to find portal: {0}", type));
			GameObject gameObject = this.World.FindObject(type);
			bool flag = gameObject == null;
			if (flag)
			{
				base.Client.Log("Couldn't find portal.");
				this.Delayed(delegate
				{
					this.GetInPortal(type);
				}, 500);
			}
			else
			{
				base.Client.Log("Portal found!");
				this.Core.UsePortal(gameObject);
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000071B8 File Offset: 0x000053B8
		private void FirstDailyQuestRoom()
		{
			PetFeedState state = this.State;
			PetFeedState petFeedState = state;
			if (petFeedState == PetFeedState.GettingItems || petFeedState == PetFeedState.Duping)
			{
				this.GetInPortal(1824);
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000071E8 File Offset: 0x000053E8
		private void FirstVault()
		{
			PetFeedState state = this.State;
			PetFeedState petFeedState = state;
			if (petFeedState != PetFeedState.GettingItems)
			{
				if (petFeedState == PetFeedState.Duping)
				{
					GameObject gameObject = this.World.FindObject(1284);
					bool flag = gameObject == null;
					if (flag)
					{
						Console.WriteLine("NO VAULT FOUND WHAT THE FUCK LOL SCRIPT FUCKEDF UP XDDD 22");
					}
					else
					{
						for (int i = 0; i < 16; i++)
						{
							bool flag2 = base.Client.Inventory[4 + i] != -1;
							if (flag2)
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
										ObjectId = gameObject.ObjectId,
										SlotId = i,
										ObjectType = -1
									},
									Time = this.Core.Time + this.Core.OffsetTime
								};
								this.Core.OffsetTime += 550;
								base.Client.Send(packet);
							}
						}
						for (int j = 0; j < 4; j++)
						{
							bool flag3 = base.Client.Inventory[j] != -1;
							if (flag3)
							{
								InvSwap packet2 = new InvSwap
								{
									Position = base.Client.Position,
									SlotObject1 = new SlotObjectData
									{
										ObjectId = base.Client.ObjectId,
										ObjectType = base.Client.Inventory[j],
										SlotId = j
									},
									SlotObject2 = new SlotObjectData
									{
										ObjectId = gameObject.ObjectId,
										SlotId = 16 + j,
										ObjectType = -1
									},
									Time = this.Core.Time + this.Core.OffsetTime
								};
								this.Core.OffsetTime += 550;
								base.Client.Send(packet2);
							}
						}
						this.State = PetFeedState.Null;
						bool connected = base.Client.Connected;
						if (connected)
						{
							this.GhostClient.GetComponent<PetFeed>().Feed();
						}
					}
				}
			}
			else
			{
				GameObject gameObject2 = this.World.FindObject(1284);
				bool flag4 = gameObject2 == null;
				if (flag4)
				{
					Console.WriteLine("NO VAULT FOUND WHAT THE FUCK LOL SCRIPT FUCKEDF UP XDDD");
				}
				else
				{
					for (int k = 0; k < 16; k++)
					{
						bool flag5 = this.Vault[k] != -1;
						if (flag5)
						{
							InvSwap packet3 = new InvSwap
							{
								Position = base.Client.Position,
								SlotObject1 = new SlotObjectData
								{
									ObjectId = gameObject2.ObjectId,
									ObjectType = this.Vault[k],
									SlotId = k
								},
								SlotObject2 = new SlotObjectData
								{
									ObjectId = base.Client.ObjectId,
									SlotId = k + 4,
									ObjectType = -1
								},
								Time = this.Core.Time + this.Core.OffsetTime
							};
							this.Core.OffsetTime += 550;
							base.Client.Send(packet3);
						}
					}
					for (int l = 0; l < 4; l++)
					{
						bool flag6 = this.Vault[16 + l] != -1;
						if (flag6)
						{
							InvSwap packet4 = new InvSwap
							{
								Position = base.Client.Position,
								SlotObject1 = new SlotObjectData
								{
									ObjectId = gameObject2.ObjectId,
									ObjectType = this.Vault[16 + l],
									SlotId = 16 + l
								},
								SlotObject2 = new SlotObjectData
								{
									ObjectId = base.Client.ObjectId,
									SlotId = l,
									ObjectType = -1
								},
								Time = this.Core.Time + this.Core.OffsetTime
							};
							this.Core.OffsetTime += 550;
							base.Client.Send(packet4);
						}
					}
					this.State = PetFeedState.GoingInPetYard;
					this.GetInPortal(1875);
				}
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000768C File Offset: 0x0000588C
		public void Feed()
		{
			this.State = PetFeedState.Null;
			List<SlotObjectData> list = new List<SlotObjectData>();
			for (int i = 0; i < 20; i++)
			{
				bool flag = base.Client.Inventory[i] != -1;
				if (flag)
				{
					list.Add(new SlotObjectData
					{
						ObjectId = base.Client.ObjectId,
						ObjectType = base.Client.Inventory[i],
						SlotId = i
					});
				}
			}
			FeedPet packet = new FeedPet
			{
				ObjectId = 0,
				PaymentType = 1,
				PetTransType = 2,
				PIDOne = this.PetInstanceId,
				PIDTwo = -1,
				Slots = list.ToArray()
			};
			base.Client.Send(packet);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00007754 File Offset: 0x00005954
		private void FirstPetYard()
		{
			bool flag = this.State == PetFeedState.GoingInPetYard;
			if (flag)
			{
				base.Client.Log("Sending ddos");
				FeedPet packet = new FeedPet
				{
					ObjectId = 0,
					PaymentType = 1,
					PetTransType = 2,
					PIDOne = -1,
					PIDTwo = -1,
					Slots = new SlotObjectData[0]
				};
				this.State = PetFeedState.Ghosting;
				for (int i = 0; i < 615; i++)
				{
					base.Client.Send(packet);
				}
				this.Delayed(delegate
				{
					this.State = PetFeedState.GhostingDone;
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
					client.AddComponent<PetFeed>(new object[]
					{
						0,
						0,
						base.Client
					});
					this.GhostClient = client;
					client.Connect();
				}, 121000);
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00007804 File Offset: 0x00005A04
		private void FirstTick()
		{
			string name = this.World.Name;
			string text = name;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1248728004U)
			{
				if (num <= 1198395147U)
				{
					if (num != 1181617528U)
					{
						if (num != 1198395147U)
						{
							return;
						}
						if (!(text == "Pet Yard 3"))
						{
							return;
						}
					}
					else if (!(text == "Pet Yard 2"))
					{
						return;
					}
				}
				else if (num != 1231950385U)
				{
					if (num != 1248728004U)
					{
						return;
					}
					if (!(text == "Pet Yard 6"))
					{
						return;
					}
				}
				else if (!(text == "Pet Yard 1"))
				{
					return;
				}
			}
			else if (num <= 1299060861U)
			{
				if (num != 1282283242U)
				{
					if (num != 1299060861U)
					{
						return;
					}
					if (!(text == "Pet Yard 5"))
					{
						return;
					}
				}
				else if (!(text == "Pet Yard 4"))
				{
					return;
				}
			}
			else if (num != 1506976487U)
			{
				if (num != 3560083791U)
				{
					return;
				}
				if (!(text == "Vault"))
				{
					return;
				}
				this.FirstVault();
				return;
			}
			else
			{
				if (!(text == "Daily Quest Room"))
				{
					return;
				}
				this.FirstDailyQuestRoom();
				return;
			}
			this.FirstPetYard();
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000793C File Offset: 0x00005B3C
		private void OnNewTick(NewTick tick)
		{
			bool flag = tick.TickId == 1;
			if (flag)
			{
				this.FirstTick();
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00007960 File Offset: 0x00005B60
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

		// Token: 0x06000114 RID: 276 RVA: 0x000079A8 File Offset: 0x00005BA8
		private void OnCreateSuccess(CreateSuccess success)
		{
			this.Vault = null;
			base.Client.Log("Success!");
			base.Client.ObjectId = success.ObjectId;
			base.Client.Account.CharId = success.CharId;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000079F8 File Offset: 0x00005BF8
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
				base.Client.Send(new Load
				{
					CharId = base.Client.Account.CharId,
					IsChallenger = false,
					IsFromArena = false
				});
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00002A02 File Offset: 0x00000C02
		private void OnReconnect(Reconnect recon)
		{
			this.Key = recon.Key;
			this.KeyTime = recon.KeyTime;
			this.GameId = recon.GameId;
			base.Client.Connection.Close();
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00007A8C File Offset: 0x00005C8C
		private void Connection_OnDisconnect()
		{
			base.Client.Log("Disconnected");
			bool flag = this.State == PetFeedState.Null;
			if (!flag)
			{
				this.Delayed(delegate
				{
					base.Client.Connect();
				}, 1000);
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00007AD4 File Offset: 0x00005CD4
		private void Connection_OnConnect()
		{
			base.Client.Log("Connected");
			base.Client.Send(new Hello
			{
				BuildVersion = "1.3.2.2.0",
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

		// Token: 0x06000119 RID: 281 RVA: 0x00002A3D File Offset: 0x00000C3D
		private void ResetKey()
		{
			this.GameId = -2;
			this.Key = null;
			this.KeyTime = -1;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00007C20 File Offset: 0x00005E20
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

		// Token: 0x040000B2 RID: 178
		public const string BuildVersion = "1.3.2.2.0";

		// Token: 0x040000B3 RID: 179
		public const string ClientToken = "8bV53M5ysJdVjU4M97fh2g7BnPXhefnc";

		// Token: 0x040000BB RID: 187
		public World World;

		// Token: 0x040000BC RID: 188
		public CoreLoop Core;

		// Token: 0x040000BD RID: 189
		public Client GhostClient;
	}
}
