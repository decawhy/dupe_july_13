using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Noidea.API
{
	// Token: 0x02000040 RID: 64
	internal class Response : IEquatable<Response>
	{
		// Token: 0x060001D5 RID: 469 RVA: 0x00002F33 File Offset: 0x00001133
		public Response(string text)
		{
			this.text = text;
			base..ctor();
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x00002F43 File Offset: 0x00001143
		[Nullable(1)]
		protected virtual Type EqualityContract
		{
			[NullableContext(1)]
			[CompilerGenerated]
			get
			{
				return typeof(Response);
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x00002F4F File Offset: 0x0000114F
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x00002F57 File Offset: 0x00001157
		public string text { get; set; }

		// Token: 0x060001D9 RID: 473 RVA: 0x0000B3E4 File Offset: 0x000095E4
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Response");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00002F60 File Offset: 0x00001160
		[NullableContext(1)]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			builder.Append("text");
			builder.Append(" = ");
			builder.Append(this.text);
			return true;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00002F88 File Offset: 0x00001188
		[NullableContext(2)]
		public static bool operator !=(Response r1, Response r2)
		{
			return !(r1 == r2);
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00002F94 File Offset: 0x00001194
		[NullableContext(2)]
		public static bool operator ==(Response r1, Response r2)
		{
			return r1 == r2 || (r1 != null && r1.Equals(r2));
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00002FAA File Offset: 0x000011AA
		public override int GetHashCode()
		{
			return EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<text>k__BackingField);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00002FD3 File Offset: 0x000011D3
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as Response);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00002FE1 File Offset: 0x000011E1
		[NullableContext(2)]
		public virtual bool Equals(Response other)
		{
			return other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<text>k__BackingField, other.<text>k__BackingField);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00003012 File Offset: 0x00001212
		[NullableContext(1)]
		public virtual Response <Clone>$()
		{
			return new Response(this);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000301A File Offset: 0x0000121A
		protected Response([Nullable(1)] Response original)
		{
			this.text = original.<text>k__BackingField;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000302F File Offset: 0x0000122F
		public void Deconstruct(out string text)
		{
			text = this.text;
		}
	}
}
