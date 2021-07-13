using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Noidea.API.Callbacks
{
	// Token: 0x0200004B RID: 75
	public class GetLogin : IEquatable<GetLogin>
	{
		// Token: 0x06000204 RID: 516 RVA: 0x000030E6 File Offset: 0x000012E6
		public GetLogin(string token, bool success)
		{
			this.token = token;
			this.success = success;
			base..ctor();
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000205 RID: 517 RVA: 0x000030FD File Offset: 0x000012FD
		[Nullable(1)]
		protected virtual Type EqualityContract
		{
			[NullableContext(1)]
			[CompilerGenerated]
			get
			{
				return typeof(GetLogin);
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000206 RID: 518 RVA: 0x00003109 File Offset: 0x00001309
		// (set) Token: 0x06000207 RID: 519 RVA: 0x00003111 File Offset: 0x00001311
		public string token { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000208 RID: 520 RVA: 0x0000311A File Offset: 0x0000131A
		// (set) Token: 0x06000209 RID: 521 RVA: 0x00003122 File Offset: 0x00001322
		public bool success { get; set; }

		// Token: 0x0600020A RID: 522 RVA: 0x0000C6F8 File Offset: 0x0000A8F8
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("GetLogin");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000C74C File Offset: 0x0000A94C
		[NullableContext(1)]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			builder.Append("token");
			builder.Append(" = ");
			builder.Append(this.token);
			builder.Append(", ");
			builder.Append("success");
			builder.Append(" = ");
			builder.Append(this.success.ToString());
			return true;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000312B File Offset: 0x0000132B
		[NullableContext(2)]
		public static bool operator !=(GetLogin r1, GetLogin r2)
		{
			return !(r1 == r2);
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00003137 File Offset: 0x00001337
		[NullableContext(2)]
		public static bool operator ==(GetLogin r1, GetLogin r2)
		{
			return r1 == r2 || (r1 != null && r1.Equals(r2));
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000314D File Offset: 0x0000134D
		public override int GetHashCode()
		{
			return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<token>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<success>k__BackingField);
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000318D File Offset: 0x0000138D
		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as GetLogin);
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000C7C0 File Offset: 0x0000A9C0
		[NullableContext(2)]
		public virtual bool Equals(GetLogin other)
		{
			return other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<token>k__BackingField, other.<token>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<success>k__BackingField, other.<success>k__BackingField);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000319B File Offset: 0x0000139B
		[NullableContext(1)]
		public virtual GetLogin <Clone>$()
		{
			return new GetLogin(this);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x000031A3 File Offset: 0x000013A3
		protected GetLogin([Nullable(1)] GetLogin original)
		{
			this.token = original.<token>k__BackingField;
			this.success = original.<success>k__BackingField;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x000031C4 File Offset: 0x000013C4
		public void Deconstruct(out string token, out bool success)
		{
			token = this.token;
			success = this.success;
		}
	}
}
