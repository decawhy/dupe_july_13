using System;
using PortableRealm.Net.Packets;

namespace Noidea.Core
{
	// Token: 0x02000011 RID: 17
	public abstract class Component
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000056 RID: 86 RVA: 0x0000232F File Offset: 0x0000052F
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00002337 File Offset: 0x00000537
		public Client Client { get; set; }

		// Token: 0x06000058 RID: 88
		public abstract void Initialize();

		// Token: 0x06000059 RID: 89 RVA: 0x00002340 File Offset: 0x00000540
		public void Hook<T>(Action<T> callback) where T : IncomingPacket
		{
			this.Client.Connection.Bind<T>(callback);
		}
	}
}
