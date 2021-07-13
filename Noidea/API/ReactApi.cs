using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace Noidea.API
{
	// Token: 0x02000049 RID: 73
	public class ReactApi
	{
		// Token: 0x060001FE RID: 510 RVA: 0x000030BB File Offset: 0x000012BB
		public ReactApi()
		{
			this._httpListener = new HttpListener();
			this._httpListener.Prefixes.Add("http://*:8080/");
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000C584 File Offset: 0x0000A784
		public void Start()
		{
			this._httpListener.Start();
			for (;;)
			{
				try
				{
					HttpListenerContext context = this._httpListener.GetContext();
					this.HandleContext(context);
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
				}
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000C5DC File Offset: 0x0000A7DC
		[DebuggerStepThrough]
		private void HandleContext(HttpListenerContext context)
		{
			ReactApi.<HandleContext>d__3 <HandleContext>d__ = new ReactApi.<HandleContext>d__3();
			<HandleContext>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleContext>d__.<>4__this = this;
			<HandleContext>d__.context = context;
			<HandleContext>d__.<>1__state = -1;
			<HandleContext>d__.<>t__builder.Start<ReactApi.<HandleContext>d__3>(ref <HandleContext>d__);
		}

		// Token: 0x04000149 RID: 329
		private HttpListener _httpListener;
	}
}
