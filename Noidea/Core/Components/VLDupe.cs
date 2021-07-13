using System;
using System.Collections.Generic;
using Noidea.Models;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x0200003A RID: 58
	public class VLDupe : Component
	{
		// Token: 0x0600019B RID: 411 RVA: 0x0000A9C4 File Offset: 0x00008BC4
		public override void Initialize()
		{
			this.Core = base.Client.GetComponent<CoreLoop>();
			this.World = base.Client.GetComponent<World>();
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
			base.Hook<Text>(delegate(Text text)
			{
				bool flag = text.Recipient == base.Client.Name && text.Message == "lagg";
				if (flag)
				{
					List<Client> clients = Program.Clients;
					int num = 0;
					foreach (Client client in clients)
					{
						client.GetComponent<VLDupe>().Dupe(num);
						num++;
					}
				}
			});
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000AA1C File Offset: 0x00008C1C
		private void Dupe(int index)
		{
			try
			{
				bool flag = base.Client.Connected && base.Client.Inventory != null;
				if (flag)
				{
					UseItem packet = new UseItem
					{
						ItemUsePos = base.Client.Position,
						SlotObject = new SlotObjectData
						{
							ObjectId = base.Client.ObjectId,
							ObjectType = base.Client.Inventory[1],
							SlotId = 1
						},
						Time = this.Core.Time,
						UseType = 0
					};
					for (int i = 0; i < 5000; i++)
					{
						base.Client.Send(packet);
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000AAF4 File Offset: 0x00008CF4
		private void OnNewTick(NewTick tick)
		{
			bool flag = tick.TickId == 5 && this.World.Name == "Nexus";
			if (flag)
			{
				GameObject gameObject = this.World.FindObject(1839);
				bool flag2 = gameObject != null;
				if (flag2)
				{
					Client client = base.Client;
					string str = "Guild Hall Found. Going in: ";
					string str2 = gameObject.ObjectId.ToString();
					string str3 = ", ";
					WorldPosData position = gameObject.Position;
					client.Log(str + str2 + str3 + ((position != null) ? position.ToString() : null));
					this.Core.UsePortal(gameObject);
				}
			}
			else
			{
				bool flag3 = this.World.Name.Contains("Guild Hall") && this.Duping;
				if (flag3)
				{
				}
			}
		}

		// Token: 0x04000112 RID: 274
		public CoreLoop Core;

		// Token: 0x04000113 RID: 275
		public World World;

		// Token: 0x04000114 RID: 276
		public bool Duping = false;
	}
}
