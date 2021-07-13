using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Noidea.API
{
	// Token: 0x0200003F RID: 63
	internal class DupeRequest : IEquatable<DupeRequest>
	{
		// Token: 0x060001BB RID: 443 RVA: 0x00002DFC File Offset: 0x00000FFC
		public DupeRequest(string guid, string password, int row, int amount, bool backpack, bool consumables, string apiKey)
		{
			this.guid = guid;
			this.password = password;
			this.row = row;
			this.amount = amount;
			this.backpack = backpack;
			this.consumables = consumables;
			this.apiKey = apiKey;
			base..ctor();
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00002E3A File Offset: 0x0000103A
		[Nullable(1)]
		protected virtual Type EqualityContract
		{
			[NullableContext(1)]
			[CompilerGenerated]
			get
			{
				return typeof(DupeRequest);
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001BD RID: 445 RVA: 0x00002E46 File Offset: 0x00001046
		// (set) Token: 0x060001BE RID: 446 RVA: 0x00002E4E File Offset: 0x0000104E
		public string guid { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001BF RID: 447 RVA: 0x00002E57 File Offset: 0x00001057
		// (set) Token: 0x060001C0 RID: 448 RVA: 0x00002E5F File Offset: 0x0000105F
		public string password { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x00002E68 File Offset: 0x00001068
		// (set) Token: 0x060001C2 RID: 450 RVA: 0x00002E70 File Offset: 0x00001070
		public int row { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x00002E79 File Offset: 0x00001079
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x00002E81 File Offset: 0x00001081
		public int amount { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x00002E8A File Offset: 0x0000108A
		// (set) Token: 0x060001C6 RID: 454 RVA: 0x00002E92 File Offset: 0x00001092
		public bool backpack { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001C7 RID: 455 RVA: 0x00002E9B File Offset: 0x0000109B
		// (set) Token: 0x060001C8 RID: 456 RVA: 0x00002EA3 File Offset: 0x000010A3
		public bool consumables { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x00002EAC File Offset: 0x000010AC
		// (set) Token: 0x060001CA RID: 458 RVA: 0x00002EB4 File Offset: 0x000010B4
		public string apiKey { get; set; }

		// Token: 0x060001CB RID: 459 RVA: 0x0000AFFC File Offset: 0x000091FC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DupeRequest");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000B050 File Offset: 0x00009250
		[NullableContext(1)]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			builder.Append("guid");
			builder.Append(" = ");
			builder.Append(this.guid);
			builder.Append(", ");
			builder.Append("password");
			builder.Append(" = ");
			builder.Append(this.password);
			builder.Append(", ");
			builder.Append("row");
			builder.Append(" = ");
			builder.Append(this.row.ToString());
			builder.Append(", ");
			builder.Append("amount");
			builder.Append(" = ");
			builder.Append(this.amount.ToString());
			builder.Append(", ");
			builder.Append("backpack");
			builder.Append(" = ");
			builder.Append(this.backpack.ToString());
			builder.Append(", ");
			builder.Append("consumables");
			builder.Append(" = ");
			builder.Append(this.consumables.ToString());
			builder.Append(", ");
			builder.Append("apiKey");
			builder.Append(" = ");
			builder.Append(this.apiKey);
			return true;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00002EBD File Offset: 0x000010BD
		[NullableContext(2)]
		public static bool operator !=(DupeRequest r1, DupeRequest r2)
		{
			return !(r1 == r2);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00002EC9 File Offset: 0x000010C9
		[NullableContext(2)]
		public static bool operator ==(DupeRequest r1, DupeRequest r2)
		{
			return r1 == r2 || (r1 != null && r1.Equals(r2));
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000B1E4 File Offset: 0x000093E4
		public override int GetHashCode()
		{
			return ((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<guid>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<password>k__BackingField)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<row>k__BackingField)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<amount>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<backpack>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<consumables>k__BackingField)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<apiKey>k__BackingField);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00002EDF File Offset: 0x000010DF
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as DupeRequest);
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000B2A4 File Offset: 0x000094A4
		[NullableContext(2)]
		public virtual bool Equals(DupeRequest other)
		{
			return other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<guid>k__BackingField, other.<guid>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<password>k__BackingField, other.<password>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<row>k__BackingField, other.<row>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<amount>k__BackingField, other.<amount>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<backpack>k__BackingField, other.<backpack>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<consumables>k__BackingField, other.<consumables>k__BackingField) && EqualityComparer<string>.Default.Equals(this.<apiKey>k__BackingField, other.<apiKey>k__BackingField);
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00002EED File Offset: 0x000010ED
		[NullableContext(1)]
		public virtual DupeRequest <Clone>$()
		{
			return new DupeRequest(this);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000B37C File Offset: 0x0000957C
		protected DupeRequest([Nullable(1)] DupeRequest original)
		{
			this.guid = original.<guid>k__BackingField;
			this.password = original.<password>k__BackingField;
			this.row = original.<row>k__BackingField;
			this.amount = original.<amount>k__BackingField;
			this.backpack = original.<backpack>k__BackingField;
			this.consumables = original.<consumables>k__BackingField;
			this.apiKey = original.<apiKey>k__BackingField;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00002EF5 File Offset: 0x000010F5
		public void Deconstruct(out string guid, out string password, out int row, out int amount, out bool backpack, out bool consumables, out string apiKey)
		{
			guid = this.guid;
			password = this.password;
			row = this.row;
			amount = this.amount;
			backpack = this.backpack;
			consumables = this.consumables;
			apiKey = this.apiKey;
		}
	}
}
