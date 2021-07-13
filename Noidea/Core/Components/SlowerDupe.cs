using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;
using Noidea.API;
using Noidea.Models;
using Noidea.Services;
using PortableRealm;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000031 RID: 49
	public class SlowerDupe : Component
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00002B79 File Offset: 0x00000D79
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00002B81 File Offset: 0x00000D81
		public World World { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00002B8A File Offset: 0x00000D8A
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00002B92 File Offset: 0x00000D92
		public CoreLoop Core { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00002B9B File Offset: 0x00000D9B
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00002BA3 File Offset: 0x00000DA3
		public GameLoader Loader { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00002BAC File Offset: 0x00000DAC
		// (set) Token: 0x06000150 RID: 336 RVA: 0x00002BB4 File Offset: 0x00000DB4
		public DupeState State { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00002BBD File Offset: 0x00000DBD
		public string Guid { get; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00002BC5 File Offset: 0x00000DC5
		public string Password { get; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00002BCD File Offset: 0x00000DCD
		public int CharId { get; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00002BD5 File Offset: 0x00000DD5
		public int CharId2 { get; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00002BDD File Offset: 0x00000DDD
		public bool Alt { get; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00002BE5 File Offset: 0x00000DE5
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00002BED File Offset: 0x00000DED
		public Client OtherClient { get; set; }

		// Token: 0x06000158 RID: 344 RVA: 0x00008CE8 File Offset: 0x00006EE8
		public SlowerDupe(TcpClient tcp, string guid, string password, int charId, int charId2, bool backpack, bool alt)
		{
			this.Tcp = tcp;
			this.Guid = guid;
			this.Password = password;
			this.Backpack = backpack;
			this.Alt = alt;
			this.CharId = charId;
			this.CharId2 = charId2;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00008D48 File Offset: 0x00006F48
		public override void Initialize()
		{
			base.Hook<NewTick>(new Action<NewTick>(this.Tick));
			base.Hook<Failure>(delegate(Failure f)
			{
				this.errorCount++;
				bool flag = this.errorCount > 20;
				bool flag2 = flag;
				if (flag2)
				{
					TcpClient tcp = this.Tcp;
					bool flag3 = tcp != null;
					if (flag3)
					{
						tcp.SendStringAsync(JsonConvert.SerializeObject(new Response("Dupe failed, unexpected error, try again")));
					}
					TcpClient tcp2 = this.Tcp;
					bool flag4 = tcp2 != null;
					if (flag4)
					{
						tcp2.Close();
					}
					Client client = ApiServer.Client1;
					GameLoader gameLoader = (client != null) ? client.GetComponent<GameLoader>() : null;
					bool flag5 = gameLoader != null;
					bool flag6 = flag5;
					if (flag6)
					{
						gameLoader.Destroyed = true;
					}
					Client client2 = ApiServer.Client2;
					GameLoader gameLoader2 = (client2 != null) ? client2.GetComponent<GameLoader>() : null;
					bool flag7 = gameLoader2 != null;
					bool flag8 = flag7;
					if (flag8)
					{
						gameLoader2.Destroyed = true;
					}
					Client client3 = ApiServer.Client1;
					bool flag9 = client3 != null;
					if (flag9)
					{
						RealmConnection connection = client3.Connection;
						bool flag10 = connection != null;
						if (flag10)
						{
							connection.Close();
						}
					}
					Client client4 = ApiServer.Client2;
					bool flag11 = client4 != null;
					if (flag11)
					{
						RealmConnection connection2 = client4.Connection;
						bool flag12 = connection2 != null;
						if (flag12)
						{
							connection2.Close();
						}
					}
				}
			});
			this.World = base.Client.AddComponent<World>(Array.Empty<object>());
			this.Core = base.Client.AddComponent<CoreLoop>(Array.Empty<object>());
			this.Loader = base.Client.AddComponent<GameLoader>(new object[]
			{
				"1.6.2.0.0",
				-2,
				this.CharId
			});
			this.Loader.Slow = false;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00008DF0 File Offset: 0x00006FF0
		private void Setup(int tickId)
		{
			string name = this.World.Name;
			string a = name;
			if (!(a == "Nexus") && !(a == "Daily Quest Room"))
			{
				if (a == "Vault")
				{
					bool flag = tickId == 1;
					if (flag)
					{
						GameObject gameObject = this.World.FindObject(1284);
						bool flag2 = gameObject == null;
						if (flag2)
						{
							Console.WriteLine("VAULT NOT FOUND? ALT: " + this.Alt.ToString());
						}
						else
						{
							this._vaultChest = gameObject.ObjectId;
							bool flag3 = !this.Alt;
							if (flag3)
							{
								bool flag4 = !base.Client.Inventory.Skip(4).Take(this.Backpack ? 16 : 8).All((int i) => i == -1);
								if (flag4)
								{
									base.Client.Log("Inventory not empty, dumping.");
									this.Core.Target = gameObject.Position;
									this.Core.TargetCallback = delegate()
									{
										SlowerDupe.<<Setup>b__45_0>d <<Setup>b__45_0>d = new SlowerDupe.<<Setup>b__45_0>d();
										<<Setup>b__45_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
										<<Setup>b__45_0>d.<>4__this = this;
										<<Setup>b__45_0>d.<>1__state = -1;
										<<Setup>b__45_0>d.<>t__builder.Start<SlowerDupe.<<Setup>b__45_0>d>(ref <<Setup>b__45_0>d);
									};
								}
								else
								{
									this.PrepareNext();
									this.Core.Target = gameObject.Position;
									this.Core.TargetCallback = delegate()
									{
										base.Client.Log("Arrived at chest");
									};
								}
							}
							else
							{
								this.Core.Target = gameObject.Position;
								this.Core.TargetCallback = delegate()
								{
									SlowerDupe.<<Setup>b__45_2>d <<Setup>b__45_2>d = new SlowerDupe.<<Setup>b__45_2>d();
									<<Setup>b__45_2>d.<>t__builder = AsyncVoidMethodBuilder.Create();
									<<Setup>b__45_2>d.<>4__this = this;
									<<Setup>b__45_2>d.<>1__state = -1;
									<<Setup>b__45_2>d.<>t__builder.Start<SlowerDupe.<<Setup>b__45_2>d>(ref <<Setup>b__45_2>d);
								};
							}
						}
					}
				}
			}
			else
			{
				bool flag5 = tickId == 0;
				if (flag5)
				{
					GameObject gameObject2 = this.World.FindObject(1824);
					bool flag6 = gameObject2 == null;
					if (flag6)
					{
						Console.WriteLine("OK MAJOR FUCK UP! AT TICK[0], DAILY, NO VAULT FOUND.");
					}
					else
					{
						this.Core.UsePortal(gameObject2);
					}
				}
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00008FDC File Offset: 0x000071DC
		public bool TakeItems()
		{
			bool flag = this.World.Vault == null;
			bool result;
			if (flag)
			{
				this.Failed = true;
				result = false;
			}
			else
			{
				int num = Math.Max(0, this.Row - 1);
				int num2 = num * 8;
				for (int i = 0; i < (this.Backpack ? 16 : 8); i++)
				{
					bool flag2 = this.World.Vault[num2 + i] == -1;
					if (!flag2)
					{
						InvSwap packet = new InvSwap
						{
							Position = base.Client.Position,
							SlotObject2 = new SlotObjectData
							{
								ObjectId = base.Client.ObjectId,
								ObjectType = -1,
								SlotId = 4 + i
							},
							SlotObject1 = new SlotObjectData
							{
								ObjectId = this._vaultChest,
								ObjectType = this.World.Vault[num2 + i],
								SlotId = num2 + i
							},
							Time = this.Core.Time + this.Core.OffsetTime
						};
						base.Client.Send(packet);
						this.Core.OffsetTime += 585;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000912C File Offset: 0x0000732C
		private void PutItems()
		{
			bool consumables = this.Consumables;
			if (consumables)
			{
				TcpClient tcp = this.Tcp;
				if (tcp != null)
				{
					tcp.SendStringAsync(JsonConvert.SerializeObject(new Response("Popping vaults...")));
				}
				for (int l = 0; l < (this.Backpack ? 16 : 8); l++)
				{
					bool flag = base.Client.Inventory[l + 4] == -1;
					if (!flag)
					{
						UseItem packet = new UseItem
						{
							ItemUsePos = new WorldPosData(0f, 0f),
							SlotObject = new SlotObjectData
							{
								ObjectId = base.Client.ObjectId,
								ObjectType = base.Client.Inventory[l + 4],
								SlotId = l + 4
							},
							Time = this.Core.Time,
							UseType = 0
						};
						base.Client.Send(packet);
					}
				}
			}
			else
			{
				bool flag2 = this.World.Vault.Count((int slot) => slot == -1) < base.Client.Inventory.Skip(4).Take(this.Backpack ? 16 : 8).Count((int i) => i != -1);
				if (flag2)
				{
					base.Client.Log("FAILED! Not enough vault slots for dupe: " + string.Join<int>(",", this.World.Vault));
					TcpClient tcp2 = this.Tcp;
					if (tcp2 != null)
					{
						tcp2.SendStringAsync(JsonConvert.SerializeObject(new Response("Vault is full. Dupe stopped. [ERROR]")));
					}
					TcpClient tcp3 = this.Tcp;
					if (tcp3 != null)
					{
						tcp3.Abort();
					}
					Client client = ApiServer.Client1;
					GameLoader gameLoader = (client != null) ? client.GetComponent<GameLoader>() : null;
					bool flag3 = gameLoader != null;
					if (flag3)
					{
						gameLoader.Destroyed = true;
					}
					Client client2 = ApiServer.Client2;
					GameLoader gameLoader2 = (client2 != null) ? client2.GetComponent<GameLoader>() : null;
					bool flag4 = gameLoader2 != null;
					if (flag4)
					{
						gameLoader2.Destroyed = true;
					}
					Client client3 = ApiServer.Client1;
					if (client3 != null)
					{
						RealmConnection connection = client3.Connection;
						if (connection != null)
						{
							connection.Close();
						}
					}
					Client client4 = ApiServer.Client2;
					if (client4 != null)
					{
						RealmConnection connection2 = client4.Connection;
						if (connection2 != null)
						{
							connection2.Close();
						}
					}
				}
				else
				{
					for (int j = 0; j < (this.Backpack ? 16 : 8); j++)
					{
						bool flag5 = base.Client.Inventory[4 + j] == -1;
						if (!flag5)
						{
							int num = -1;
							for (int k = 0; k < this.World.Vault.Length; k++)
							{
								bool flag6 = this.World.Vault[k] == -1;
								if (flag6)
								{
									num = k;
									break;
								}
							}
							this.World.Vault[num] = base.Client.Inventory[4 + j];
							InvSwap packet2 = new InvSwap
							{
								Position = base.Client.Position,
								SlotObject1 = new SlotObjectData
								{
									ObjectId = base.Client.ObjectId,
									ObjectType = base.Client.Inventory[4 + j],
									SlotId = 4 + j
								},
								SlotObject2 = new SlotObjectData
								{
									ObjectId = this._vaultChest,
									ObjectType = -1,
									SlotId = num
								},
								Time = this.Core.Time + this.Core.OffsetTime
							};
							base.Client.Send(packet2);
							this.Core.OffsetTime += 585;
						}
					}
				}
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00009500 File Offset: 0x00007700
		private void PrepareNext()
		{
			Client client;
			for (;;)
			{
				TcpClient tcp = this.Tcp;
				if (tcp != null)
				{
					tcp.SendStringAsync(JsonConvert.SerializeObject(new Response("Attempting dupe...")));
				}
				ValueTuple<string, string, string> valueTuple = Factory.Session(this.Guid, this.Password);
				client = new Client(new Server
				{
					Address = "3.86.254.40",
					Name = "EUNorth2"
				}, valueTuple.Item1);
				bool flag = valueTuple.Item1 == null;
				if (!flag)
				{
					break;
				}
				TcpClient tcp2 = this.Tcp;
				if (tcp2 != null)
				{
					tcp2.SendStringAsync(JsonConvert.SerializeObject(new Response("Something fucked up.")));
				}
				bool flag2 = valueTuple.Item2 != null;
				if (flag2)
				{
					TcpClient tcp3 = this.Tcp;
					if (tcp3 != null)
					{
						tcp3.SendStringAsync(JsonConvert.SerializeObject(new Response("Error: " + valueTuple.Item2)));
					}
				}
				else
				{
					bool flag3 = valueTuple.Item3 != null;
					if (flag3)
					{
						TcpClient tcp4 = this.Tcp;
						if (tcp4 != null)
						{
							tcp4.SendStringAsync(JsonConvert.SerializeObject(new Response("Error: " + valueTuple.Item3)));
						}
					}
				}
				TcpClient tcp5 = this.Tcp;
				if (tcp5 != null)
				{
					tcp5.SendStringAsync(JsonConvert.SerializeObject(new Response("Trying again in 2 minutes... DONT CLOSE")));
				}
				Thread.Sleep(120000);
			}
			client.AddComponent<SlowerDupe>(new object[]
			{
				this.Tcp,
				this.Guid,
				this.Password,
				this.CharId2,
				this.CharId,
				this.Backpack,
				true
			}).OtherClient = base.Client;
			ApiServer.Client2 = client;
			client.Connect();
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000096BC File Offset: 0x000078BC
		private void Tick(NewTick tick)
		{
			DupeState state = this.State;
			DupeState dupeState = state;
			if (dupeState == DupeState.Setup)
			{
				this.Setup(tick.TickId);
			}
		}

		// Token: 0x040000DE RID: 222
		public const string DupingServer = "3.86.254.40";

		// Token: 0x040000E7 RID: 231
		public bool Backpack;

		// Token: 0x040000EA RID: 234
		public Action<bool> Callback;

		// Token: 0x040000EB RID: 235
		private int _vaultChest;

		// Token: 0x040000EC RID: 236
		public TcpClient Tcp;

		// Token: 0x040000ED RID: 237
		public int Row;

		// Token: 0x040000EE RID: 238
		public bool Consumables = false;

		// Token: 0x040000EF RID: 239
		private int errorCount = 0;

		// Token: 0x040000F0 RID: 240
		public bool Failed = false;

		// Token: 0x040000F1 RID: 241
		private static Random random = new Random();
	}
}
