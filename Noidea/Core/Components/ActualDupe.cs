using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Noidea.Models;
using Noidea.Services;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000013 RID: 19
	public class ActualDupe : Component
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002354 File Offset: 0x00000554
		// (set) Token: 0x0600005C RID: 92 RVA: 0x0000235C File Offset: 0x0000055C
		public World World { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00002365 File Offset: 0x00000565
		// (set) Token: 0x0600005E RID: 94 RVA: 0x0000236D File Offset: 0x0000056D
		public CoreLoop Core { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00002376 File Offset: 0x00000576
		// (set) Token: 0x06000060 RID: 96 RVA: 0x0000237E File Offset: 0x0000057E
		public GameLoader Loader { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000061 RID: 97 RVA: 0x00002387 File Offset: 0x00000587
		// (set) Token: 0x06000062 RID: 98 RVA: 0x0000238F File Offset: 0x0000058F
		public DupeState State { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00002398 File Offset: 0x00000598
		public string Guid { get; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000023A0 File Offset: 0x000005A0
		public string Password { get; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000023A8 File Offset: 0x000005A8
		public int CharId { get; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000023B0 File Offset: 0x000005B0
		public int CharId2 { get; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000023B8 File Offset: 0x000005B8
		public bool Alt { get; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000068 RID: 104 RVA: 0x000023C0 File Offset: 0x000005C0
		// (set) Token: 0x06000069 RID: 105 RVA: 0x000023C8 File Offset: 0x000005C8
		public Client OtherClient { get; set; }

		// Token: 0x0600006A RID: 106 RVA: 0x000023D1 File Offset: 0x000005D1
		public ActualDupe(string guid, string password, int charId, int charId2, bool backpack, bool alt)
		{
			this.Guid = guid;
			this.Password = password;
			this.Backpack = backpack;
			this.Alt = alt;
			this.CharId = charId;
			this.CharId2 = charId2;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003D58 File Offset: 0x00001F58
		public override void Initialize()
		{
			base.Hook<NewTick>(new Action<NewTick>(this.Tick));
			this.World = base.Client.AddComponent<World>(Array.Empty<object>());
			this.Core = base.Client.AddComponent<CoreLoop>(Array.Empty<object>());
			this.Loader = base.Client.AddComponent<GameLoader>(new object[]
			{
				"1.6.0.0.0",
				-2,
				this.CharId
			});
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003DE0 File Offset: 0x00001FE0
		private void Setup(int tickId)
		{
			string name = this.World.Name;
			string a = name;
			if (!(a == "Nexus") && !(a == "Daily Quest Room"))
			{
				if (a == "Vault")
				{
					bool flag = tickId == 0;
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
										ActualDupe.<<Setup>b__42_0>d <<Setup>b__42_0>d = new ActualDupe.<<Setup>b__42_0>d();
										<<Setup>b__42_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
										<<Setup>b__42_0>d.<>4__this = this;
										<<Setup>b__42_0>d.<>1__state = -1;
										<<Setup>b__42_0>d.<>t__builder.Start<ActualDupe.<<Setup>b__42_0>d>(ref <<Setup>b__42_0>d);
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
									ActualDupe.<<Setup>b__42_2>d <<Setup>b__42_2>d = new ActualDupe.<<Setup>b__42_2>d();
									<<Setup>b__42_2>d.<>t__builder = AsyncVoidMethodBuilder.Create();
									<<Setup>b__42_2>d.<>4__this = this;
									<<Setup>b__42_2>d.<>1__state = -1;
									<<Setup>b__42_2>d.<>t__builder.Start<ActualDupe.<<Setup>b__42_2>d>(ref <<Setup>b__42_2>d);
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

		// Token: 0x0600006D RID: 109 RVA: 0x00003FCC File Offset: 0x000021CC
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
				for (int i = 0; i < (this.Backpack ? 16 : 8); i++)
				{
					bool flag2 = this.World.Vault[i] == -1;
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
								ObjectType = this.World.Vault[i],
								SlotId = i
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

		// Token: 0x0600006E RID: 110 RVA: 0x000040F8 File Offset: 0x000022F8
		private void PutItems()
		{
			Console.WriteLine("Consumingk");
			for (int i = 0; i < (this.Backpack ? 16 : 8); i++)
			{
				bool flag = base.Client.Inventory[i + 4] == -1;
				if (!flag)
				{
					UseItem packet = new UseItem
					{
						ItemUsePos = new WorldPosData(0f, 0f),
						SlotObject = new SlotObjectData
						{
							ObjectId = base.Client.ObjectId,
							ObjectType = base.Client.Inventory[i + 4],
							SlotId = i + 4
						},
						Time = this.Core.Time,
						UseType = 0
					};
					base.Client.Send(packet);
				}
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000041D0 File Offset: 0x000023D0
		private void PrepareNext()
		{
			ValueTuple<string, string, string> valueTuple = Factory.Session(this.Guid, this.Password);
			Client client = new Client(new Server
			{
				Address = "3.86.254.40",
				Name = "EUNorth2"
			}, valueTuple.Item1);
			bool flag = valueTuple.Item1 == null;
			if (flag)
			{
				this.Loader.Destroyed = true;
				base.Client.Connection.Close();
				Thread.Sleep(1000);
				this.Callback(false);
			}
			else
			{
				client.AddComponent<ActualDupe>(new object[]
				{
					this.Guid,
					this.Password,
					this.CharId2,
					this.CharId,
					this.Backpack,
					true
				}).OtherClient = base.Client;
				client.Connect();
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000042C4 File Offset: 0x000024C4
		private void Tick(NewTick tick)
		{
			DupeState state = this.State;
			DupeState dupeState = state;
			if (dupeState == DupeState.Setup)
			{
				this.Setup(tick.TickId);
			}
		}

		// Token: 0x04000046 RID: 70
		public const string DupingServer = "3.86.254.40";

		// Token: 0x04000047 RID: 71
		public const bool Consumables = true;

		// Token: 0x04000050 RID: 80
		public bool Backpack;

		// Token: 0x04000053 RID: 83
		public Action<bool> Callback;

		// Token: 0x04000054 RID: 84
		private int _vaultChest;

		// Token: 0x04000055 RID: 85
		public bool Failed = false;
	}
}
