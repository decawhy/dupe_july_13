using System;
using System.Collections.Generic;
using Noidea.Models;
using PortableRealm;
using PortableRealm.Net.Packets;

namespace Noidea.Core
{
	// Token: 0x02000010 RID: 16
	public class Client : GameObject
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00002286 File Offset: 0x00000486
		public Dictionary<Type, Component> Components { get; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000049 RID: 73 RVA: 0x0000228E File Offset: 0x0000048E
		public RealmConnection Connection { get; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002296 File Offset: 0x00000496
		public Account Account { get; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600004B RID: 75 RVA: 0x0000229E File Offset: 0x0000049E
		public bool Connected
		{
			get
			{
				return this.Connection.Connected;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600004C RID: 76 RVA: 0x000022AB File Offset: 0x000004AB
		// (set) Token: 0x0600004D RID: 77 RVA: 0x000022B3 File Offset: 0x000004B3
		public Server Server { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600004E RID: 78 RVA: 0x000022BC File Offset: 0x000004BC
		public string AccessToken { get; }

		// Token: 0x0600004F RID: 79 RVA: 0x000022C4 File Offset: 0x000004C4
		[Obsolete]
		public Client(Account account)
		{
			this.Connection = new RealmConnection();
			this.Components = new Dictionary<Type, Component>();
			this.Account = account;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000022EB File Offset: 0x000004EB
		public Client(Server server, string accessToken)
		{
			this.Connection = new RealmConnection();
			this.Components = new Dictionary<Type, Component>();
			this.Server = server;
			this.AccessToken = accessToken;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003BEC File Offset: 0x00001DEC
		public void Connect()
		{
			Console.WriteLine("Connecting");
			this.Connection.Connect(this.Server.Address, (this.Server.Address == "127.0.0.1") ? 2051 : 2050);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000231A File Offset: 0x0000051A
		public void Send(OutgoingPacket packet)
		{
			RealmConnection connection = this.Connection;
			if (connection != null)
			{
				connection.Send(packet);
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003C40 File Offset: 0x00001E40
		public void Log(object message)
		{
			string format = "[{0}]: {1}";
			string arg;
			if ((arg = base.Name) == null)
			{
				string accessToken = this.AccessToken;
				arg = ((accessToken != null) ? accessToken.Substring(0, Math.Min(10, this.AccessToken.Length)) : null);
			}
			Console.WriteLine(string.Format(format, arg, message));
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003C90 File Offset: 0x00001E90
		public T AddComponent<T>(params object[] args) where T : Component
		{
			Component component = Activator.CreateInstance(typeof(T), args) as Component;
			component.Client = this;
			component.Initialize();
			Dictionary<Type, Component> components = this.Components;
			lock (components)
			{
				this.Components.Add(typeof(T), component);
			}
			return component as T;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003D1C File Offset: 0x00001F1C
		public T GetComponent<T>() where T : Component
		{
			Component component;
			return this.Components.TryGetValue(typeof(T), out component) ? (component as T) : default(T);
		}
	}
}
