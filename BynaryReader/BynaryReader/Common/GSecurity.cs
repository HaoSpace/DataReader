using System.Collections;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

static public class GSecurity : object
{
    static private string _Key;
    static private string _IV;
	
	static GSecurity ()
	{
        _Key = "Party001";
        _IV = "Island01";
	}

    static public string Encrypt (string value)
    {
        return Encrypt(value, _Key, _IV);
    }


    static public string Decrypt (string value)
    {
        return Decrypt(value, _Key, _IV);
    }


    static private string Encrypt (string pToEncrypt, string sKey, string sIV)
    {
        StringBuilder ret = new StringBuilder();
        using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
        {
            byte[] inputByteArray = Encoding.Unicode.GetBytes(pToEncrypt);

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);

            des.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

            using (MemoryStream ms = new MemoryStream()                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          )
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                }

                foreach (byte b in ms.ToArray())
                    ret.AppendFormat("{0:X2}", b);
            }
        }

        return ret.ToString();
    }

    static private string Decrypt (string pToDecrypt, string sKey, string sIV)
    {
        using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
        {
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];

            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    try
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();

                        return System.Text.Encoding.Unicode.GetString(ms.ToArray());
                    }
                    catch (CryptographicException)
                    {

                        return "N/A";
                    }
                }
            }
        }
    }
}