using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Noidea.Core;
using Noidea.Core.Components;
using Noidea.Models;
using Noidea.Services;
using PortableRealm;

namespace Noidea.API
{
	// Token: 0x02000041 RID: 65
	public class ApiServer
	{
		// Token: 0x060001E3 RID: 483 RVA: 0x00003039 File Offset: 0x00001239
		public ApiServer()
		{
			this._listener = new TcpListener(IPAddress.Any, 43594);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00003058 File Offset: 0x00001258
		public void Start()
		{
			new Thread(delegate()
			{
				this._listener.Start();
				for (;;)
				{
					try
					{
						TcpClient client = this._listener.AcceptTcpClient();
						this.HandleClient(client);
					}
					catch (Exception value)
					{
						Console.WriteLine(value);
					}
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000B438 File Offset: 0x00009638
		private void HandleClient(TcpClient client)
		{
			string text = client.ReadString();
			DupeRequest hello = JsonConvert.DeserializeObject<DupeRequest>(text);
			Console.WriteLine(hello);
			string @string = Encoding.UTF8.GetString(Convert.FromBase64String(hello.apiKey));
			bool flag = @string != "a2a551f1290186a5d0b1ce0f18fd4cf40bfb1dec646807c7fa5522ab27f4e77c0c5d68a6459bd64bb24dab5f88f9908856dd064d918a8de220505c8383fd19e8fd720a7750a394088a9ac8659aea2dd6729de67b7738a57a1446cc76d639a9e924f8e41cf4ea056982abff5e88e83007e1c5ea103042a63a7020ef757158e2bf";
			if (flag)
			{
				File.WriteAllText("REDALERT.txt", text + "\nIP = " + client.Client.RemoteEndPoint.ToString());
			}
			else
			{
				Console.WriteLine("API KEY confirmed");
				bool flag2 = hello.amount < 1;
				if (flag2)
				{
					client.SendStringAsync(JsonConvert.SerializeObject(new Response("Amount is negative")));
					client.Abort();
				}
				else
				{
					bool flag3 = hello.row < 0;
					if (flag3)
					{
						client.SendStringAsync(JsonConvert.SerializeObject(new Response("Row is negative")));
						client.Abort();
					}
					else
					{
						File.AppendAllText("log.txt", text + "\r\n\r\n");
						bool flag4 = hello.guid == "DESTROY_FAILSAFE";
						if (flag4)
						{
							Client client2 = ApiServer.Client1;
							GameLoader gameLoader = (client2 != null) ? client2.GetComponent<GameLoader>() : null;
							bool flag5 = gameLoader != null;
							if (flag5)
							{
								gameLoader.Destroyed = true;
							}
							Client client3 = ApiServer.Client2;
							GameLoader gameLoader2 = (client3 != null) ? client3.GetComponent<GameLoader>() : null;
							bool flag6 = gameLoader2 != null;
							if (flag6)
							{
								gameLoader2.Destroyed = true;
							}
							Client client4 = ApiServer.Client1;
							if (client4 != null)
							{
								RealmConnection connection = client4.Connection;
								if (connection != null)
								{
									connection.Close();
								}
							}
							Client client5 = ApiServer.Client2;
							if (client5 != null)
							{
								RealmConnection connection2 = client5.Connection;
								if (connection2 != null)
								{
									connection2.Close();
								}
							}
							TcpClient client6 = client;
							if (client6 != null)
							{
								client6.SendStringAsync(JsonConvert.SerializeObject(new Response("All clients terminated.")));
							}
							client.Abort();
						}
						else
						{
							try
							{
								ValueTuple<string, string, string> valueTuple = Factory.Session(hello.guid, hello.password);
								bool flag7 = valueTuple.Item1 == null;
								if (flag7)
								{
									bool flag8 = valueTuple.Item2 != null;
									if (flag8)
									{
										client.SendStringAsync(JsonConvert.SerializeObject(new Response("Invalid login details nigga")));
										client.Abort();
									}
									else
									{
										client.SendStringAsync(JsonConvert.SerializeObject(new Response("Deca being stupid, wait 5 min")));
										client.Abort();
									}
								}
								else
								{
									client.SendStringAsync(JsonConvert.SerializeObject(new Response("Starting duping process")));
									string path = "char/list";
									NameValueCollection nameValueCollection = new NameValueCollection();
									nameValueCollection["accessToken"] = valueTuple.Item1;
									string text2;
									bool flag9 = !Factory.Post(path, nameValueCollection, out text2);
									if (flag9)
									{
										bool flag10 = text2.ToLower().Contains("internal error");
										if (flag10)
										{
											client.SendStringAsync(JsonConvert.SerializeObject(new Response("Too many invalid details, deca got mad and gave internal error, wait 10 minutes")));
										}
										client.Abort();
									}
									else
									{
										Console.WriteLine(text2);
										IEnumerable<XElement> enumerable = XElement.Parse(text2).Elements("Char");
										XElement[] array = enumerable.ToArray<XElement>();
										int charId1 = -1;
										int charId2 = -1;
										bool flag11 = !hello.backpack;
										if (flag11)
										{
											foreach (XElement xelement in enumerable)
											{
												XElement xelement2 = xelement.Element("Equipment");
												int[] source = Array.ConvertAll<string, int>((xelement2 != null) ? xelement2.Value.Split(',', StringSplitOptions.None) : null, new Converter<string, int>(int.Parse));
												bool flag12 = source.Skip(4).Take(8).All((int i) => i == -1);
												if (flag12)
												{
													charId1 = int.Parse(xelement.Attribute("id").Value);
													break;
												}
											}
											bool flag13 = charId1 == -1;
											if (flag13)
											{
												charId1 = int.Parse(array[0].Attribute("id").Value);
											}
										}
										else
										{
											XElement[] array2 = (from ch in enumerable
											where ch.Element("HasBackpack").Value == "1"
											select ch).ToArray<XElement>();
											bool flag14 = array2.Length == 0;
											if (flag14)
											{
												client.SendStringAsync(JsonConvert.SerializeObject(new Response("You cant use backpack dupe, without a FUCKING BACKPACK, stupid rice cooker")));
												return;
											}
											XElement xelement3 = array2.FirstOrDefault(delegate(XElement ch)
											{
												XElement xelement5 = ch.Element("Equipment");
												return Array.ConvertAll<string, int>((xelement5 != null) ? xelement5.Value.Split(',', StringSplitOptions.None) : null, new Converter<string, int>(int.Parse)).Skip(4).Take(16).All((int i) => i == -1);
											});
											bool flag15 = xelement3 == null;
											if (flag15)
											{
												xelement3 = array2[0];
											}
											charId1 = int.Parse(xelement3.Attribute("id").Value);
										}
										foreach (XElement xelement4 in enumerable)
										{
											int num = int.Parse(xelement4.Attribute("id").Value);
											bool flag16 = num != charId1;
											if (flag16)
											{
												charId2 = num;
												break;
											}
										}
										bool flag17 = charId2 == -1;
										if (flag17)
										{
											client.SendStringAsync(JsonConvert.SerializeObject(new Response("Account doesnt have 2 character slots, fucking spasticated anal cancer victim")));
											client.Abort();
										}
										else
										{
											Console.WriteLine("Found char ids: {0}, {1}", charId1, charId2);
											Client client7 = new Client(new Server
											{
												Address = "35.180.73.63",
												Name = "EUNorth"
											}, valueTuple.Item1);
											bool flag18 = valueTuple.Item1 == null;
											if (flag18)
											{
												TcpClient client8 = client;
												if (client8 != null)
												{
													client8.SendStringAsync(JsonConvert.SerializeObject(new Response("Something fucked up.")));
												}
												bool flag19 = valueTuple.Item2 != null;
												if (flag19)
												{
													TcpClient client9 = client;
													if (client9 != null)
													{
														client9.SendStringAsync(JsonConvert.SerializeObject(new Response("Error: " + valueTuple.Item2)));
													}
												}
												else
												{
													bool flag20 = valueTuple.Item3 != null;
													if (flag20)
													{
														TcpClient client10 = client;
														if (client10 != null)
														{
															client10.SendStringAsync(JsonConvert.SerializeObject(new Response("Error: " + valueTuple.Item3)));
														}
													}
												}
												TcpClient client11 = client;
												if (client11 != null)
												{
													client11.Abort();
												}
											}
											else
											{
												SlowerDupe slowerDupe = client7.AddComponent<SlowerDupe>(new object[]
												{
													client,
													hello.guid,
													hello.password,
													charId1,
													charId2,
													hello.backpack,
													false
												});
												slowerDupe.Row = hello.row;
												slowerDupe.Consumables = hello.consumables;
												slowerDupe.Callback = delegate(bool success)
												{
													TcpClient client13 = client;
													if (client13 != null)
													{
														client13.SendStringAsync(JsonConvert.SerializeObject(new Response("Dupe success!")));
													}
													ApiServer.Dupe(client, hello.guid, hello.password, charId1, charId2, hello.backpack, hello.amount - 1, hello.row, hello.consumables);
												};
												ApiServer.Client1 = client7;
												client7.Connect();
											}
										}
									}
								}
							}
							catch (Exception)
							{
								TcpClient client12 = client;
								if (client12 != null)
								{
									client12.Close();
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000BD04 File Offset: 0x00009F04
		public static void Dupe(TcpClient cl, string guid, string pass, int charId, int charId2, bool backpack, int count, int row, bool consumables)
		{
			try
			{
				bool flag = count == 0;
				if (flag)
				{
					TcpClient cl2 = cl;
					if (cl2 != null)
					{
						cl2.SendStringAsync(JsonConvert.SerializeObject(new Response("FINAL_ALLDONE")));
					}
					Task.Delay(1500).ContinueWith(delegate(Task t)
					{
						TcpClient cl8 = cl;
						if (cl8 != null)
						{
							cl8.Close();
						}
					});
				}
				else
				{
					Console.WriteLine("Dupes left: " + count.ToString());
					TcpClient cl3 = cl;
					if (cl3 != null)
					{
						cl3.SendStringAsync(JsonConvert.SerializeObject(new Response("Dupes left: " + count.ToString())));
					}
					ValueTuple<string, string, string> valueTuple;
					for (;;)
					{
						valueTuple = Factory.Session(guid, pass);
						bool flag2 = valueTuple.Item1 == null;
						if (!flag2)
						{
							break;
						}
						TcpClient cl4 = cl;
						if (cl4 != null)
						{
							cl4.SendStringAsync(JsonConvert.SerializeObject(new Response("Something fucked up.")));
						}
						bool flag3 = valueTuple.Item2 != null;
						if (flag3)
						{
							TcpClient cl5 = cl;
							if (cl5 != null)
							{
								cl5.SendStringAsync(JsonConvert.SerializeObject(new Response("Error: " + valueTuple.Item2)));
							}
						}
						else
						{
							bool flag4 = valueTuple.Item3 != null;
							if (flag4)
							{
								TcpClient cl6 = cl;
								if (cl6 != null)
								{
									cl6.SendStringAsync(JsonConvert.SerializeObject(new Response("Error: " + valueTuple.Item3)));
								}
							}
						}
						TcpClient cl7 = cl;
						if (cl7 != null)
						{
							cl7.SendStringAsync(JsonConvert.SerializeObject(new Response("Trying again in 2 minutes.. DONT CLOSE")));
						}
						Thread.Sleep(120000);
					}
					Client client = new Client(new Server
					{
						Address = "52.87.248.5",
						Name = "EUNorth"
					}, valueTuple.Item1);
					SlowerDupe slowerDupe = client.AddComponent<SlowerDupe>(new object[]
					{
						cl,
						guid,
						pass,
						charId,
						charId2,
						backpack,
						false
					});
					slowerDupe.Row = row;
					slowerDupe.Consumables = consumables;
					slowerDupe.Callback = delegate(bool success)
					{
						TcpClient cl8 = cl;
						if (cl8 != null)
						{
							cl8.SendStringAsync(JsonConvert.SerializeObject(new Response("Dupe success!")));
						}
						ApiServer.Dupe(cl, guid, pass, charId, charId2, backpack, success ? (count - 1) : count, row, consumables);
					};
					ApiServer.Client1 = client;
					client.Connect();
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				cl.SendStringAsync(JsonConvert.SerializeObject(new Response("Dupe fucked up, ABORT")));
			}
		}

		// Token: 0x04000126 RID: 294
		public const string Hash = "a2a551f1290186a5d0b1ce0f18fd4cf40bfb1dec646807c7fa5522ab27f4e77c0c5d68a6459bd64bb24dab5f88f9908856dd064d918a8de220505c8383fd19e8fd720a7750a394088a9ac8659aea2dd6729de67b7738a57a1446cc76d639a9e924f8e41cf4ea056982abff5e88e83007e1c5ea103042a63a7020ef757158e2bf";

		// Token: 0x04000127 RID: 295
		public static Client Client1;

		// Token: 0x04000128 RID: 296
		public static Client Client2;

		// Token: 0x04000129 RID: 297
		private TcpListener _listener;
	}
}
