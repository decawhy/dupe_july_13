using System;
using System.Collections.Generic;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x0200001B RID: 27
	public class Dupe : Component
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x0000507C File Offset: 0x0000327C
		public static void Start(string name)
		{
			Dupe.NAME = name;
			Queue<string> obj = Dupe.invitequeue;
			lock (obj)
			{
				Dupe.invitequeue.Enqueue(name);
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000025D0 File Offset: 0x000007D0
		public override void Initialize()
		{
			Dupe.CLIENT = base.Client;
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
			base.Hook<Text>(delegate(Text text)
			{
				base.Client.Log(text.Message);
			});
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000050CC File Offset: 0x000032CC
		private void OnNewTick(NewTick obj)
		{
			bool flag = obj.TickId == 5;
			if (flag)
			{
				base.Client.Send(new GuildInvite
				{
					Name = "stmuaaac"
				});
			}
			bool flag2 = obj.TickId % 15 == 0;
			if (flag2)
			{
				Queue<string> obj2 = Dupe.invitequeue;
				lock (obj2)
				{
					bool flag4 = Dupe.invitequeue.Count > 0;
					if (flag4)
					{
						string text = Dupe.invitequeue.Dequeue();
						Console.WriteLine("Inviting: " + text);
						base.Client.Send(new GuildInvite
						{
							Name = text
						});
					}
				}
			}
		}

		// Token: 0x04000073 RID: 115
		private static Client CLIENT;

		// Token: 0x04000074 RID: 116
		private static string NAME;

		// Token: 0x04000075 RID: 117
		private static Queue<string> invitequeue = new Queue<string>();
	}
}
