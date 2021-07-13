using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Noidea.API
{
	// Token: 0x02000046 RID: 70
	public static class ApiExtensions
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x0000C1EC File Offset: 0x0000A3EC
		[DebuggerStepThrough]
		public static void SendStringAsync(this TcpClient client, string text)
		{
			ApiExtensions.<SendStringAsync>d__0 <SendStringAsync>d__ = new ApiExtensions.<SendStringAsync>d__0();
			<SendStringAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SendStringAsync>d__.client = client;
			<SendStringAsync>d__.text = text;
			<SendStringAsync>d__.<>1__state = -1;
			<SendStringAsync>d__.<>t__builder.Start<ApiExtensions.<SendStringAsync>d__0>(ref <SendStringAsync>d__);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000C22C File Offset: 0x0000A42C
		[DebuggerStepThrough]
		public static void Abort(this TcpClient client)
		{
			ApiExtensions.<Abort>d__1 <Abort>d__ = new ApiExtensions.<Abort>d__1();
			<Abort>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Abort>d__.client = client;
			<Abort>d__.<>1__state = -1;
			<Abort>d__.<>t__builder.Start<ApiExtensions.<Abort>d__1>(ref <Abort>d__);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000C268 File Offset: 0x0000A468
		public static string ReadString(this TcpClient client)
		{
			byte[] array = new byte[2];
			client.GetStream().ReadAll(array);
			int num = (int)array[1] | (int)array[0] << 8;
			byte[] array2 = new byte[num];
			client.GetStream().ReadAll(array2);
			return Encoding.UTF8.GetString(array2);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000C2B8 File Offset: 0x0000A4B8
		public static void ReadAll(this NetworkStream stream, byte[] buffer)
		{
			int num;
			for (int i = 0; i < buffer.Length; i += num)
			{
				num = stream.Read(buffer, i, buffer.Length - i);
				bool flag = num == 0;
				if (flag)
				{
					throw new Exception("End of stream");
				}
			}
		}
	}
}
