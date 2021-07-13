using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Noidea.API;
using Noidea.Core;
using Noidea.Core.Components;
using Noidea.Models;
using Noidea.Services;
using PortableRealm.Net.Packets;

namespace Noidea
{
	// Token: 0x02000005 RID: 5
	internal class Program
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000031F0 File Offset: 0x000013F0
		private static void Main(string[] args)
		{
			PacketManager.LoadPackets("Packets.json");
			Program.Timer.Start();
			new ApiServer().Start();
			for (;;)
			{
				Console.ReadKey();
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000322C File Offset: 0x0000142C
		private static void Dupe(string guid, string pass, int charId, int charId2, int count, bool backpack = false)
		{
			bool flag = count == 0;
			if (flag)
			{
				Program.Timer.Stop();
				TimeSpan elapsed = Program.Timer.Elapsed;
				Console.WriteLine();
				Console.WriteLine(" *************** ");
				Console.WriteLine();
				Console.WriteLine(string.Format(" - Dupe took ({0})", elapsed));
				Console.WriteLine(string.Format(" - {0} hours, {1} minutes, {2} seconds", elapsed.Hours, elapsed.Minutes, elapsed.Seconds));
				Console.WriteLine();
				Console.WriteLine(" *************** ");
			}
			else
			{
				Program.GlobalCount++;
				bool flag2 = Program.GlobalCount > 20;
				if (flag2)
				{
					bool flag3 = Program.Timer.ElapsedMilliseconds - (long)Program.LastQueueTime <= 600000L;
					if (flag3)
					{
						Console.WriteLine("Internal error prevention Started.");
						Console.WriteLine("Current Delta: " + (Program.Timer.ElapsedMilliseconds - (long)Program.LastQueueTime).ToString());
						long num = 600000L - (Program.Timer.ElapsedMilliseconds - (long)Program.LastQueueTime);
						Thread.Sleep((int)num);
					}
					else
					{
						Console.WriteLine("Internal error is over.");
						Program.GlobalCount = 0;
						Program.LastQueueTime = (int)Program.Timer.ElapsedMilliseconds;
					}
				}
				Console.WriteLine("Dupes Left: " + count.ToString());
				Client client;
				for (;;)
				{
					ValueTuple<string, string, string> valueTuple = Factory.Session(guid, pass);
					client = new Client(new Server
					{
						Address = "52.87.248.5",
						Name = "EUNorth"
					}, valueTuple.Item1);
					bool flag4 = valueTuple.Item1 == null;
					if (!flag4)
					{
						break;
					}
					bool flag5 = valueTuple.Item2 != null;
					if (flag5)
					{
						Console.WriteLine(valueTuple.Item2);
					}
					else
					{
						bool flag6 = valueTuple.Item3 != null;
						if (flag6)
						{
							Console.WriteLine(valueTuple.Item3);
						}
					}
					Thread.Sleep(120000);
				}
				ActualDupe actualDupe = client.AddComponent<ActualDupe>(new object[]
				{
					guid,
					pass,
					charId,
					charId2,
					backpack,
					false
				});
				actualDupe.Callback = delegate(bool success)
				{
					Console.WriteLine("Callback called");
					Program.Dupe(guid, pass, charId, charId2, success ? (count - 1) : count, backpack);
				};
				client.Connect();
			}
		}

		// Token: 0x04000003 RID: 3
		public static List<Client> Clients = new List<Client>();

		// Token: 0x04000004 RID: 4
		public static Stopwatch Timer = new Stopwatch();

		// Token: 0x04000005 RID: 5
		public static int GlobalCount = 0;

		// Token: 0x04000006 RID: 6
		public static int LastQueueTime;
	}
}
