using System;
using System.Threading;
using System.Threading.Tasks;
using Noidea.Models;
using Noidea.Services;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000025 RID: 37
	public class NewDupe : Component
	{
		// Token: 0x060000E9 RID: 233 RVA: 0x000028D1 File Offset: 0x00000AD1
		public NewDupe(DupeType type, int dupeTries)
		{
			this.Type = type;
			this.DupeTries = dupeTries;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00006734 File Offset: 0x00004934
		public override void Initialize()
		{
			base.Client.Connection.OnConnect += this.Connection_OnConnect;
			base.Client.Connection.OnDisconnect += this.Connection_OnDisconnect;
			this.World = base.Client.GetComponent<World>();
			base.Hook<MapInfo>(new Action<MapInfo>(this.OnMapInfo));
			base.Hook<CreateSuccess>(new Action<CreateSuccess>(this.OnCreateSuccess));
			base.Hook<Reconnect>(new Action<Reconnect>(this.OnReconnect));
			base.Hook<Update>(new Action<Update>(this.OnUpdate));
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000067EC File Offset: 0x000049EC
		private void OnNewTick(NewTick tick)
		{
			bool flag = tick.TickId == 2 && this.World.Name.Contains("Pet Yard");
			if (flag)
			{
				base.Client.Log("Sending the packets");
				FeedPet packet = new FeedPet
				{
					ObjectId = 0,
					PaymentType = 1,
					PetTransType = 2,
					PIDOne = 1,
					PIDTwo = -1,
					Slots = new SlotObjectData[0]
				};
				for (int i = 0; i < 1400; i++)
				{
					base.Client.Send(packet);
				}
				Task.Delay(126500).ContinueWith(delegate(Task t)
				{
					this.NewClient();
				});
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000028F8 File Offset: 0x00000AF8
		public void Start()
		{
			Task.Run(delegate()
			{
				base.Client.Log("Sending..");
				ActivePetUpdateRequest packet = new ActivePetUpdateRequest
				{
					CommandType = 1,
					InstanceId = 1
				};
				ActivePetUpdateRequest packet2 = new ActivePetUpdateRequest
				{
					CommandType = 2,
					InstanceId = 1
				};
				for (;;)
				{
					base.Client.Log("Cycling.");
					base.Client.Send(packet);
					Thread.Sleep(1200);
					base.Client.Send(packet2);
					Thread.Sleep(1200);
				}
			});
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000068B0 File Offset: 0x00004AB0
		private void NewClient()
		{
			bool flag = this.DupeTries >= NewDupe.ips.Length;
			if (flag)
			{
				for (int i = 0; i < 3; i++)
				{
					Console.WriteLine("###### --------- ######");
				}
				Console.WriteLine("\tDUPE IS DONE");
				for (int j = 0; j < 3; j++)
				{
					Console.WriteLine("###### --------- ######");
				}
				foreach (Client client in Program.Clients)
				{
					client.GetComponent<NewDupe>().Start();
				}
			}
			else
			{
				Console.WriteLine("New client, going deep AF + " + this.DupeTries.ToString());
				Account account = new Account
				{
					Email = base.Client.Account.Email,
					CharId = base.Client.Account.CharId,
					Password = base.Client.Account.Password,
					Secret = base.Client.Account.Secret,
					Server = base.Client.Account.Server
				};
				account.Server = new Server
				{
					Address = NewDupe.ips[this.DupeTries],
					Name = "Lol"
				};
				Client client2 = new Client(account);
				client2.AddComponent<World>(Array.Empty<object>());
				client2.AddComponent<CoreLoop>(Array.Empty<object>());
				client2.AddComponent<NewDupe>(new object[]
				{
					DupeType.Spammer,
					this.DupeTries + 1
				});
				Program.Clients.Add(client2);
				client2.Connect();
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006A90 File Offset: 0x00004C90
		private void OnUpdate(Update update)
		{
			ObjectData[] newObjects = update.NewObjects;
			for (int i = 0; i < newObjects.Length; i++)
			{
				ObjectData obj = newObjects[i];
				bool flag = obj.Status.ObjectId == base.Client.ObjectId;
				if (flag)
				{
					bool flag2 = this.World.Name == "Daily Quest Room";
					if (flag2)
					{
						base.Client.Position = new WorldPosData(15.5f, 9.5f);
					}
					else
					{
						bool flag3 = this.World.Name == "Vault";
						if (flag3)
						{
							base.Client.Position = new WorldPosData(39.5f, 71.5f);
						}
					}
				}
				else
				{
					bool flag4 = obj.ObjectType == 1824 && this.World.Name == "Daily Quest Room";
					if (flag4)
					{
						Task.Delay(1000).ContinueWith(delegate(Task t)
						{
							this.Client.Send(new UsePortal
							{
								ObjectId = obj.Status.ObjectId
							});
						});
					}
					else
					{
						bool flag5 = obj.ObjectType == 1875 && this.World.Name == "Vault";
						if (flag5)
						{
							Task.Delay(1000).ContinueWith(delegate(Task t)
							{
								this.Client.Send(new UsePortal
								{
									ObjectId = obj.Status.ObjectId
								});
							});
						}
					}
				}
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000290D File Offset: 0x00000B0D
		private void Connection_OnDisconnect()
		{
			Task.Delay(1000).ContinueWith(delegate(Task t)
			{
				base.Client.Connect();
			});
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006C0C File Offset: 0x00004E0C
		private void OnReconnect(Reconnect recon)
		{
			this.Key = recon.Key;
			this.KeyTime = recon.KeyTime;
			this.GameId = recon.GameId;
			base.Client.Send(new Create
			{
				ClassType = -1,
				IsChallenger = false,
				SkinType = -1
			});
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00006C64 File Offset: 0x00004E64
		private void Connection_OnConnect()
		{
			base.Client.Log("Connected");
			base.Client.Send(new Hello
			{
				BuildVersion = "1.3.0.1.0",
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

		// Token: 0x060000F2 RID: 242 RVA: 0x00006DB0 File Offset: 0x00004FB0
		private void OnCreateSuccess(CreateSuccess success)
		{
			base.Client.ObjectId = success.ObjectId;
			base.Client.Account.CharId = success.CharId;
			this.GameId = -2;
			this.Key = null;
			this.KeyTime = -1;
			base.Client.Log("Success!");
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006E10 File Offset: 0x00005010
		private void OnMapInfo(MapInfo info)
		{
			base.Client.Log("Map load: " + info.Name);
			bool flag = info.Name == "Nexus";
			if (flag)
			{
				base.Client.Send(new GoToQuestRoom());
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

		// Token: 0x040000A2 RID: 162
		public static string[] ips = new string[]
		{
			"35.175.246.38",
			"54.219.75.186",
			"3.18.236.163",
			"3.249.136.218",
			"35.180.55.233",
			"35.180.73.116",
			"35.180.207.183"
		};

		// Token: 0x040000A3 RID: 163
		public byte[] Key;

		// Token: 0x040000A4 RID: 164
		public int KeyTime = -1;

		// Token: 0x040000A5 RID: 165
		public int GameId = -2;

		// Token: 0x040000A6 RID: 166
		public World World;

		// Token: 0x040000A7 RID: 167
		public DupeType Type;

		// Token: 0x040000A8 RID: 168
		public int DupeTries;
	}
}
