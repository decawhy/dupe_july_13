using System;
using System.Linq;
using PortableRealm.Net.Packets.Incoming;
using PortableRealm.Net.Packets.Outgoing;

namespace Noidea.Core.Components
{
	// Token: 0x02000035 RID: 53
	public class TransferMule : Component
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00002C0E File Offset: 0x00000E0E
		public TransferToMules Transfer { get; }

		// Token: 0x06000171 RID: 369 RVA: 0x00002C16 File Offset: 0x00000E16
		public TransferMule(TransferToMules transfer)
		{
			this.Transfer = transfer;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00009E64 File Offset: 0x00008064
		public override void Initialize()
		{
			base.Hook<TradeRequested>(new Action<TradeRequested>(this.OnTradeRequested));
			base.Hook<TradeStart>(new Action<TradeStart>(this.OnTradeStart));
			base.Hook<TradeAccepted>(new Action<TradeAccepted>(this.OnTradeAccepted));
			base.Hook<TradeDone>(new Action<TradeDone>(this.OnTradeDone));
			base.Hook<NewTick>(new Action<NewTick>(this.OnNewTick));
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00009ED4 File Offset: 0x000080D4
		private void OnNewTick(NewTick tick)
		{
			bool flag = tick.TickId == 2;
			if (flag)
			{
				base.Client.Log("Mule is ready!");
				bool flag2 = base.Client.Inventory.Skip(4).Take(8).All((int item) => item == -1);
				bool flag3 = !flag2;
				if (flag3)
				{
					this.Transfer.ReportError("THIS MULE IS NOT EMPTY WHAT THE FUCK L OLOLKOL: " + base.Client.Account.Email);
				}
				else
				{
					this.Transfer.MuleIsReady();
				}
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00009F80 File Offset: 0x00008180
		private void OnTradeDone(TradeDone trade)
		{
			base.Client.Log("Trade: " + trade.Description + " | " + trade.TradeCode.ToString());
			bool flag = trade.TradeCode == 0;
			if (flag)
			{
				base.Client.GetComponent<GameLoader>().Destroyed = true;
				base.Client.Connection.Close();
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00002C27 File Offset: 0x00000E27
		private void OnTradeAccepted(TradeAccepted accepted)
		{
			base.Client.Send(new AcceptTrade
			{
				ClientOffer = accepted.ClientOffer,
				PartnerOffer = accepted.PartnerOffer
			});
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00002C53 File Offset: 0x00000E53
		private void OnTradeStart(TradeStart obj)
		{
			base.Client.Log("Trade started!");
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00009FEC File Offset: 0x000081EC
		private void OnTradeRequested(TradeRequested req)
		{
			base.Client.Log("Trade request from: " + req.Name);
			bool flag = req.Name.ToLower() == this.Transfer.Client.Name.ToLower();
			if (flag)
			{
				base.Client.Send(new RequestTrade
				{
					Name = req.Name
				});
			}
		}
	}
}
