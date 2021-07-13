using System;
using System.Collections.Generic;
using System.Linq;
using Noidea.Models;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x0200003B RID: 59
	public class World : Component
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x00002D2E File Offset: 0x00000F2E
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x00002D36 File Offset: 0x00000F36
		public string Name { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x00002D3F File Offset: 0x00000F3F
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x00002D47 File Offset: 0x00000F47
		public string RealmName { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x00002D50 File Offset: 0x00000F50
		// (set) Token: 0x060001A5 RID: 421 RVA: 0x00002D58 File Offset: 0x00000F58
		public int Width { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x00002D61 File Offset: 0x00000F61
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x00002D69 File Offset: 0x00000F69
		public int Height { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00002D72 File Offset: 0x00000F72
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x00002D7A File Offset: 0x00000F7A
		public uint Seed { get; private set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00002D83 File Offset: 0x00000F83
		// (set) Token: 0x060001AB RID: 427 RVA: 0x00002D8B File Offset: 0x00000F8B
		public int[] Vault { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001AC RID: 428 RVA: 0x00002D94 File Offset: 0x00000F94
		public Dictionary<int, GameObject> Objects { get; }

		// Token: 0x060001AD RID: 429 RVA: 0x00002D9C File Offset: 0x00000F9C
		public World()
		{
			this.Objects = new Dictionary<int, GameObject>();
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000AC50 File Offset: 0x00008E50
		public GameObject FindObject(ushort objectType)
		{
			return this.Objects.Values.FirstOrDefault(delegate(GameObject o)
			{
				ushort? num = (o != null) ? new ushort?(o.ObjectType) : null;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				int objectType2 = (int)objectType;
				return num2.GetValueOrDefault() == objectType2 & num2 != null;
			});
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000AC88 File Offset: 0x00008E88
		public GameObject FindObjectByName(string name)
		{
			string nameLower = name.ToLower();
			return this.Objects.Values.FirstOrDefault(delegate(GameObject o)
			{
				string a;
				if (o == null)
				{
					a = null;
				}
				else
				{
					string name2 = o.Name;
					a = ((name2 != null) ? name2.ToLower() : null);
				}
				return a == nameLower;
			});
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000ACC8 File Offset: 0x00008EC8
		public override void Initialize()
		{
			base.Hook<Failure>(new Action<Failure>(this.OnFailure));
			base.Hook<MapInfo>(new Action<MapInfo>(this.OnMapInfo));
			base.Hook<Update>(new Action<Update>(this.OnUpdate));
			base.Hook<VaultUpdate>(new Action<VaultUpdate>(this.OnVaultUpdate));
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000AD24 File Offset: 0x00008F24
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

		// Token: 0x060001B2 RID: 434 RVA: 0x00002DB1 File Offset: 0x00000FB1
		private void OnFailure(Failure obj)
		{
			base.Client.Log("Error: " + obj.ErrorDescription);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000AD6C File Offset: 0x00008F6C
		private void OnUpdate(Update update)
		{
			base.Client.Send(new UpdateAck());
			foreach (ObjectData objectData in update.NewObjects)
			{
				bool flag = objectData.Status.ObjectId == base.Client.ObjectId;
				if (flag)
				{
					base.Client.ObjectType = objectData.ObjectType;
					base.Client.Position = objectData.Status.Position;
					base.Client.Inventory = new int[20];
					base.Client.ProcessData(objectData.Status.Stats);
				}
				else
				{
					GameObject gameObject = new GameObject
					{
						ObjectType = objectData.ObjectType,
						ObjectId = objectData.Status.ObjectId,
						Position = objectData.Status.Position,
						Inventory = new int[20]
					};
					gameObject.ProcessData(objectData.Status.Stats);
					Dictionary<int, GameObject> objects = this.Objects;
					lock (objects)
					{
						this.Objects.Add(gameObject.ObjectId, gameObject);
					}
				}
			}
			foreach (int key in update.Drops)
			{
				Dictionary<int, GameObject> objects2 = this.Objects;
				lock (objects2)
				{
					this.Objects.Remove(key);
				}
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000AF34 File Offset: 0x00009134
		private void OnMapInfo(MapInfo info)
		{
			this.Name = info.Name;
			this.Width = info.Width;
			this.Height = info.Height;
			this.Seed = info.Seed;
			this.RealmName = info.RealmName;
			this.Objects.Clear();
			this.Vault = null;
		}
	}
}
