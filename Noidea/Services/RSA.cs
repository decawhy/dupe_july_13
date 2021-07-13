using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace Noidea.Services
{
	// Token: 0x0200000B RID: 11
	public class RSA
	{
		// Token: 0x0600001B RID: 27 RVA: 0x000039F0 File Offset: 0x00001BF0
		private RSA(string pem)
		{
			this.key = (new PemReader(new StringReader(pem)).ReadObject() as AsymmetricKeyParameter);
			this.engine = new RsaEngine();
			this.engine.Init(true, this.key);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003A40 File Offset: 0x00001C40
		public string Encrypt(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			Pkcs1Encoding pkcs1Encoding = new Pkcs1Encoding(this.engine);
			pkcs1Encoding.Init(true, this.key);
			return Convert.ToBase64String(pkcs1Encoding.ProcessBlock(bytes, 0, bytes.Length));
		}

		// Token: 0x04000016 RID: 22
		public static readonly RSA Instance = new RSA("-----BEGIN PUBLIC KEY-----\r\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDCKFctVrhfF3m2Kes0FBL/JFeOcmNg9eJz8k/hQy1kadD+XFUpluRqa//Uxp2s9W2qE0EoUCu59ugcf/p7lGuL99UoSGmQEynkBvZct+/M40L0E0rZ4BVgzLOJmIbXMp0J4PnPcb6VLZvxazGcmSfjauC7F3yWYqUbZd/HCBtawwIDAQAB\r\n-----END PUBLIC KEY-----");

		// Token: 0x04000017 RID: 23
		private RsaEngine engine;

		// Token: 0x04000018 RID: 24
		private readonly AsymmetricKeyParameter key;
	}
}
