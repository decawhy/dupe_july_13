using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace Noidea.Services
{
	// Token: 0x02000008 RID: 8
	public class Factory
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00003560 File Offset: 0x00001760
		public static string GetApi()
		{
			bool flag = Factory.CurrentRotation >= Factory.ApiList.Length - 1;
			string result;
			if (flag)
			{
				result = Factory.ApiList[Factory.CurrentRotation = 0];
			}
			else
			{
				result = Factory.ApiList[Factory.CurrentRotation++];
			}
			return result;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000035B0 File Offset: 0x000017B0
		[return: TupleElementNames(new string[]
		{
			"accessToken",
			"tokenExp",
			"tokenTime"
		})]
		public static ValueTuple<string, string, string> Session(string guid, string password)
		{
			bool flag = guid.StartsWith("steamworks:");
			string path = "account/verify";
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["guid"] = guid;
			string name = flag ? "secret" : "password";
			nameValueCollection[name] = password;
			nameValueCollection["clientToken"] = "f6ff89ce8d0498bcfcbdc836072ba9d5119789cc";
			nameValueCollection["game_net"] = (flag ? "Unity_steam" : "Unity");
			nameValueCollection["play_platform"] = (flag ? "Unity_steam" : "Unity");
			nameValueCollection["game_net_user_id"] = "";
			string text;
			bool flag2 = !Factory.Post(path, nameValueCollection, out text);
			ValueTuple<string, string, string> result;
			if (flag2)
			{
				Console.WriteLine("Request failed. {1}");
				result = new ValueTuple<string, string, string>(null, null, null);
			}
			else
			{
				bool flag3 = text.StartsWith("<Error>WebChangePasswordDialog.passwordError</Error>");
				if (flag3)
				{
					result = new ValueTuple<string, string, string>(null, "invalid password", null);
				}
				else
				{
					bool flag4 = text.StartsWith("<Error>");
					if (flag4)
					{
						Console.WriteLine(text);
						result = new ValueTuple<string, string, string>(null, null, "Internal Error");
					}
					else
					{
						XElement xelement = XElement.Parse(text);
						string value = xelement.Element("AccessToken").Value;
						string value2 = xelement.Element("AccessTokenExpiration").Value;
						string value3 = xelement.Element("AccessTokenTimestamp").Value;
						Console.WriteLine("Creating session..");
						string path2 = "account/verifyAccessTokenClient";
						NameValueCollection nameValueCollection2 = new NameValueCollection();
						nameValueCollection2["accessToken"] = value;
						nameValueCollection2["clientToken"] = "f6ff89ce8d0498bcfcbdc836072ba9d5119789cc";
						nameValueCollection2["game_net"] = (flag ? "Unity_steam" : "Unity");
						nameValueCollection2["play_platform"] = (flag ? "Unity_steam" : "Unity");
						nameValueCollection2["game_net_user_id"] = "";
						string text2;
						bool flag5 = !Factory.Post(path2, nameValueCollection2, out text2);
						if (flag5)
						{
							Console.WriteLine("Request failed. {2}");
							result = new ValueTuple<string, string, string>(null, null, null);
						}
						else
						{
							bool flag6 = text2 != "<Success/>";
							if (flag6)
							{
								Console.WriteLine(text2);
								result = new ValueTuple<string, string, string>(null, null, null);
							}
							else
							{
								result = new ValueTuple<string, string, string>(value, value2, value3);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000037FC File Offset: 0x000019FC
		public static bool Post(string path, NameValueCollection parameters, out string response)
		{
			bool result;
			using (WebClient webClient = new WebClient())
			{
				webClient.Headers.Add("user-agent", "UnityPlayer/2019.3.14f1 (UnityWebRequest/1.0, libcurl/7.52.0-DEV)");
				for (;;)
				{
					try
					{
						string api;
						for (;;)
						{
							api = Factory.GetApi();
							bool flag = Factory.Internals.Any(([TupleElementNames(new string[]
							{
								"ip",
								"added"
							})] ValueTuple<string, DateTime> a) => a.Item1 == api);
							if (flag)
							{
								ValueTuple<string, DateTime> valueTuple = Factory.Internals.FirstOrDefault(([TupleElementNames(new string[]
								{
									"ip",
									"added"
								})] ValueTuple<string, DateTime> a) => a.Item1 == api);
								bool flag2 = (DateTime.Now - valueTuple.Item2).TotalMinutes < 6.0;
								if (flag2)
								{
									continue;
								}
								Factory.Internals.RemoveWhere(([TupleElementNames(new string[]
								{
									"ip",
									"added"
								})] ValueTuple<string, DateTime> a) => a.Item1 == api);
							}
							bool flag3 = !api.Contains("realmofthemadgod.com");
							bool flag4 = flag3;
							if (flag4)
							{
								Console.WriteLine("Using Balancer: " + api);
							}
							response = Encoding.UTF8.GetString(webClient.UploadValues(api + "/" + path + (flag3 ? "?apiKey=efd641391f3b7dff00a1ce4294df3a58" : ""), parameters));
							bool flag5 = response.Contains("wait 5 minutes");
							if (!flag5)
							{
								break;
							}
							Factory.Internals.Add(new ValueTuple<string, DateTime>(api, DateTime.Now));
						}
						result = true;
						break;
					}
					catch (Exception value)
					{
						Console.WriteLine(value);
					}
				}
			}
			return result;
		}

		// Token: 0x0400000E RID: 14
		public const string ApiUrl = "https://www.realmofthemadgod.com";

		// Token: 0x0400000F RID: 15
		public const string HWID = "f6ff89ce8d0498bcfcbdc836072ba9d5119789cc";

		// Token: 0x04000010 RID: 16
		public static bool useProxy = false;

		// Token: 0x04000011 RID: 17
		public static int CurrentRotation = 0;

		// Token: 0x04000012 RID: 18
		public static string[] ApiList = new string[]
		{
			"https://www.realmofthemadgod.com",
			"http://www.realmofthemadgod.com"
		};

		// Token: 0x04000013 RID: 19
		[TupleElementNames(new string[]
		{
			"ip",
			"added"
		})]
		public static HashSet<ValueTuple<string, DateTime>> Internals = new HashSet<ValueTuple<string, DateTime>>();
	}
}
