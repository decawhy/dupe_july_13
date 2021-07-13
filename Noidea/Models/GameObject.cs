using System;
using PortableRealm.Models;

namespace Noidea.Models
{
	// Token: 0x0200000E RID: 14
	public class GameObject
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002198 File Offset: 0x00000398
		// (set) Token: 0x0600002A RID: 42 RVA: 0x000021A0 File Offset: 0x000003A0
		public int ObjectId { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000021A9 File Offset: 0x000003A9
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000021B1 File Offset: 0x000003B1
		public ushort ObjectType { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000021BA File Offset: 0x000003BA
		// (set) Token: 0x0600002E RID: 46 RVA: 0x000021C2 File Offset: 0x000003C2
		public string Name { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000021CB File Offset: 0x000003CB
		// (set) Token: 0x06000030 RID: 48 RVA: 0x000021D3 File Offset: 0x000003D3
		public string AccountId { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000031 RID: 49 RVA: 0x000021DC File Offset: 0x000003DC
		// (set) Token: 0x06000032 RID: 50 RVA: 0x000021E4 File Offset: 0x000003E4
		public int MaxHp { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000021ED File Offset: 0x000003ED
		// (set) Token: 0x06000034 RID: 52 RVA: 0x000021F5 File Offset: 0x000003F5
		public int MaxMp { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000021FE File Offset: 0x000003FE
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002206 File Offset: 0x00000406
		public int Hp { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000037 RID: 55 RVA: 0x0000220F File Offset: 0x0000040F
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00002217 File Offset: 0x00000417
		public int Mp { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002220 File Offset: 0x00000420
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002228 File Offset: 0x00000428
		public int Speed { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002231 File Offset: 0x00000431
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002239 File Offset: 0x00000439
		public int[] Inventory { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002242 File Offset: 0x00000442
		// (set) Token: 0x0600003E RID: 62 RVA: 0x0000224A File Offset: 0x0000044A
		public bool Backpack { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002253 File Offset: 0x00000453
		// (set) Token: 0x06000040 RID: 64 RVA: 0x0000225B File Offset: 0x0000045B
		public WorldPosData Position { get; set; }

		// Token: 0x06000041 RID: 65 RVA: 0x00003A88 File Offset: 0x00001C88
		public void ProcessData(StatData[] stats)
		{
			foreach (StatData statData in stats)
			{
				byte statType = statData.StatType;
				byte b = statType;
				if (b <= 22)
				{
					switch (b)
					{
					case 0:
						this.MaxHp = statData.StatValue;
						break;
					case 1:
						this.Hp = statData.StatValue;
						break;
					case 2:
						break;
					case 3:
						this.MaxMp = statData.StatValue;
						break;
					case 4:
						this.Mp = statData.StatValue;
						break;
					default:
						if (b == 22)
						{
							this.Speed = statData.StatValue;
						}
						break;
					}
				}
				else if (b != 31)
				{
					if (b != 38)
					{
						if (b == 79)
						{
							this.Backpack = (statData.StatValue == 1);
						}
					}
					else
					{
						this.AccountId = statData.StringValue;
					}
				}
				else
				{
					this.Name = statData.StringValue;
				}
				bool flag = statData.StatType >= 8 && statData.StatType <= 19;
				if (flag)
				{
					this.Inventory[(int)(statData.StatType - 8)] = statData.StatValue;
				}
				else
				{
					bool flag2 = statData.StatType >= 71 && statData.StatType <= 78;
					if (flag2)
					{
						this.Inventory[(int)(statData.StatType - 59)] = statData.StatValue;
					}
				}
			}
		}
	}
}
