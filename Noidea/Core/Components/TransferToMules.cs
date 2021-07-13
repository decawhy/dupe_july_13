using System;
using System.Linq;
using System.Threading.Tasks;
using Noidea.Models;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000038 RID: 56
	public class TransferToMules : Component
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600017B RID: 379 RVA: 0x00002C73 File Offset: 0x00000E73
		public string[] Accounts { get; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00002C7B File Offset: 0x00000E7B
		public int ObjectType { get; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00002C83 File Offset: 0x00000E83
		// (set) Token: 0x0600017E RID: 382 RVA: 0x00002C8B File Offset: 0x00000E8B
		public int AccountIndex { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00002C94 File Offset: 0x00000E94
		// (set) Token: 0x06000180 RID: 384 RVA: 0x00002C9C File Offset: 0x00000E9C
		public ScatterState State { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000181 RID: 385 RVA: 0x00002CA5 File Offset: 0x00000EA5
		public int TotalTransfered
		{
			get
			{
				return this.TransferCount - this.ItemsToTransfer;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000182 RID: 386 RVA: 0x00002CB4 File Offset: 0x00000EB4
		// (set) Token: 0x06000183 RID: 387 RVA: 0x00002CBC File Offset: 0x00000EBC
		public int ItemsToTransfer { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00002CC5 File Offset: 0x00000EC5
		// (set) Token: 0x06000185 RID: 389 RVA: 0x00002CCD File Offset: 0x00000ECD
		public int TransferCount { get; set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000186 RID: 390 RVA: 0x00002CD6 File Offset: 0x00000ED6
		// (set) Token: 0x06000187 RID: 391 RVA: 0x00002CDE File Offset: 0x00000EDE
		public Client Mule { get; set; }

		// Token: 0x06000188 RID: 392 RVA: 0x0000A060 File Offset: 0x00008260
		public TransferToMules(string[] accounts, int objectType, int itemsToTransfer)
		{
			this.Accounts = accounts;
			this.ObjectType = objectType;
			this.ItemsToTransfer = itemsToTransfer;
			this.TransferCount = itemsToTransfer;
			this.AccountIndex = 0;
			this.State = ScatterState.Start;
			this._muleIsReady = false;
			this._imReady = false;
			Task.Delay(1000).ContinueWith(delegate(Task t)
			{
				this.LoadNext();
			});
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000A0D0 File Offset: 0x000082D0
		public override void Initialize()
		{
			this.Core = base.Client.GetComponent<CoreLoop>();
			this.World = base.Client.GetComponent<World>();
			base.Hook<MapInfo>(delegate(MapInfo m)
			{
				this.Vault = null;
			});
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
			base.Hook<Update>(new Action<Update>(this.OnUpdate));
			base.Hook<TradeStart>(new Action<TradeStart>(this.OnTradeStart));
			base.Hook<VaultUpdate>(new Action<VaultUpdate>(this.VaultUpdate));
			base.Hook<TradeDone>(new Action<TradeDone>(this.OnTradeDone));
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000A174 File Offset: 0x00008374
		private void OnTradeDone(TradeDone trade)
		{
			base.Client.Log("Trade: " + trade.Description + " | " + trade.TradeCode.ToString());
			this._muleIsReady = false;
			this._imReady = false;
			bool flag = trade.TradeCode == 0;
			if (flag)
			{
				this.ItemsToTransfer = Math.Max(this.ItemsToTransfer - 8, 0);
				base.Client.Log(string.Format("Done: (TotalTransfered: {0}, ItemsToTransfer: {1})", this.TotalTransfered, this.ItemsToTransfer));
				this.LoadNext();
				GameObject gameObject = this.World.FindObject(1824);
				bool flag2 = gameObject == null;
				if (flag2)
				{
					this.ReportError("no vault portal");
				}
				else
				{
					base.Client.Log("Going to vault portal");
					this.Core.UsePortal(gameObject);
				}
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000A25C File Offset: 0x0000845C
		private void OnTradeStart(TradeStart obj)
		{
			base.Client.Log("Trade started!");
			int num = Math.Min(8, this.ItemsToTransfer);
			ChangeTrade changeTrade = new ChangeTrade
			{
				Offer = new bool[12 + (base.Client.Backpack ? 8 : 0)]
			};
			for (int i = 0; i < changeTrade.Offer.Length; i++)
			{
				changeTrade.Offer[i] = false;
			}
			for (int j = 0; j < num; j++)
			{
				changeTrade.Offer[4 + j] = true;
			}
			base.Client.Send(changeTrade);
			AcceptTrade accept = new AcceptTrade
			{
				ClientOffer = changeTrade.Offer,
				PartnerOffer = new bool[20]
			};
			for (int k = 0; k < accept.PartnerOffer.Length; k++)
			{
				accept.PartnerOffer[k] = false;
			}
			Task.Delay(2000).ContinueWith(delegate(Task t)
			{
				this.Client.Send(accept);
			});
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000A390 File Offset: 0x00008590
		private void OnNewTick(NewTick tick)
		{
			bool flag = tick.TickId == 2;
			if (flag)
			{
				this.FirstTick();
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000A3B4 File Offset: 0x000085B4
		private void FirstTick()
		{
			bool flag = this.World.Name == "Nexus";
			if (flag)
			{
				bool flag2 = this.State == ScatterState.Start;
				if (flag2)
				{
					bool flag3 = base.Client.Inventory.Skip(4).Take(8).All((int item) => item == this.ObjectType);
					base.Client.Log("Inventory: " + string.Join<int>(",", base.Client.Inventory));
					bool flag4 = !flag3;
					if (flag4)
					{
						GameObject gameObject = this.World.FindObject(1824);
						bool flag5 = gameObject == null;
						if (flag5)
						{
							this.ReportError("no vault portal");
						}
						else
						{
							base.Client.Log("Going to vault portal");
							this.Core.UsePortal(gameObject);
						}
					}
					else
					{
						this.ImReady();
					}
				}
			}
			else
			{
				bool flag6 = this.World.Name == "Vault";
				if (flag6)
				{
					this.Core.Target = new WorldPosData(40.5f, 66.5f);
					this.Core.TargetCallback = delegate()
					{
						Console.WriteLine("Got here.");
						int i = 0;
						while (i < 8)
						{
							bool flag7 = base.Client.Inventory[i + 4] != -1 && base.Client.Inventory[i + 4] != this.ObjectType;
							if (flag7)
							{
								this.ReportError("Integrity compromised? Invalid item in inventory: " + string.Join<int>(",", base.Client.Inventory));
							}
							else
							{
								int getSlot = this.GetSlot;
								bool flag8 = getSlot == -1;
								if (!flag8)
								{
									InvSwap packet = new InvSwap
									{
										Position = base.Client.Position,
										SlotObject1 = new SlotObjectData
										{
											ObjectId = this.VaultChest,
											ObjectType = this.Vault[getSlot],
											SlotId = getSlot
										},
										SlotObject2 = new SlotObjectData
										{
											ObjectId = base.Client.ObjectId,
											ObjectType = -1,
											SlotId = 4 + i
										},
										Time = this.Core.Time + this.Core.OffsetTime
									};
									this.Core.OffsetTime += 550;
									base.Client.Send(packet);
									this.Vault[getSlot] = -1;
									i++;
									continue;
								}
								this.ReportError("Not enough items in vault? Stock is empty LOL");
							}
							return;
						}
						base.Client.Send(new Escape());
					};
				}
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600018E RID: 398 RVA: 0x0000A4F4 File Offset: 0x000086F4
		private int GetSlot
		{
			get
			{
				for (int i = 0; i < this.Vault.Length; i++)
				{
					bool flag = this.Vault[i] == this.ObjectType;
					if (flag)
					{
						return i;
					}
				}
				return -1;
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000A53C File Offset: 0x0000873C
		private void VaultUpdate(VaultUpdate update)
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

		// Token: 0x06000190 RID: 400 RVA: 0x0000A584 File Offset: 0x00008784
		private void OnUpdate(Update update)
		{
			foreach (ObjectData objectData in update.NewObjects)
			{
				bool flag = objectData.ObjectType == 1284;
				if (flag)
				{
					this.VaultChest = objectData.Status.ObjectId;
					break;
				}
			}
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000A5D4 File Offset: 0x000087D4
		public void ReportError(string err)
		{
			Console.WriteLine(string.Format("SCRIPT FUCKED UP! AT [{0}], Data: {{ AccountIndex: {1}, Total: {2}, ItemsToTransfer: {3} }}", new object[]
			{
				err,
				this.AccountIndex,
				this.TotalTransfered,
				this.ItemsToTransfer
			}));
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000A628 File Offset: 0x00008828
		public void MuleIsReady()
		{
			this._muleIsReady = true;
			bool imReady = this._imReady;
			if (imReady)
			{
				base.Client.Log("Request sent to: " + this.Mule.Name);
				base.Client.Send(new RequestTrade
				{
					Name = this.Mule.Name
				});
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000A68C File Offset: 0x0000888C
		private void ImReady()
		{
			base.Client.Log("Main is ready");
			this._imReady = true;
			bool muleIsReady = this._muleIsReady;
			if (muleIsReady)
			{
				base.Client.Log("Request sent to: " + this.Mule.Name);
				base.Client.Send(new RequestTrade
				{
					Name = this.Mule.Name
				});
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000A704 File Offset: 0x00008904
		private void LoadNext()
		{
			bool flag = this.AccountIndex >= this.Accounts.Length;
			if (flag)
			{
				for (int i = 0; i < 10; i++)
				{
					Console.WriteLine("ALL DONE?");
				}
				this.ReportError("ALL DONE TEST!");
			}
			else
			{
				Account account = new Account
				{
					CharId = 1,
					Email = this.Accounts[this.AccountIndex],
					Password = "",
					Secret = null,
					Server = new Server
					{
						Address = base.Client.Account.Server.Address,
						Name = base.Client.Account.Server.Name
					}
				};
				int accountIndex = this.AccountIndex;
				this.AccountIndex = accountIndex + 1;
				Client client = new Client(account);
				this.Mule = client;
				client.AddComponent<World>(Array.Empty<object>());
				client.AddComponent<CoreLoop>(Array.Empty<object>());
				client.AddComponent<GameLoader>(new object[]
				{
					"1.3.1.0.0",
					-2
				});
				client.AddComponent<TransferMule>(new object[]
				{
					this
				});
				client.Connect();
			}
		}

		// Token: 0x0400010A RID: 266
		private bool _muleIsReady;

		// Token: 0x0400010B RID: 267
		private bool _imReady;

		// Token: 0x0400010C RID: 268
		private World World;

		// Token: 0x0400010D RID: 269
		private CoreLoop Core;

		// Token: 0x0400010E RID: 270
		private int[] Vault;

		// Token: 0x0400010F RID: 271
		private int VaultChest;
	}
}
