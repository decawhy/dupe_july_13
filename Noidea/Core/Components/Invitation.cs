using System;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000020 RID: 32
	public class Invitation : Component
	{
		// Token: 0x060000C9 RID: 201 RVA: 0x000027AD File Offset: 0x000009AD
		public override void Initialize()
		{
			base.Hook<InvitedToGuild>(new Action<InvitedToGuild>(this.OnInvitedToGuild));
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00005880 File Offset: 0x00003A80
		private void OnNewTick(NewTick obj)
		{
			bool flag = obj.TickId == 5;
			if (flag)
			{
				Dupe.Start(base.Client.Name);
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000027D6 File Offset: 0x000009D6
		private void OnInvitedToGuild(InvitedToGuild obj)
		{
			base.Client.Log("Got invited");
			base.Client.Send(new JoinGuild
			{
				Name = obj.GuildName
			});
		}
	}
}
