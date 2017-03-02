//using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public static class Extension : object
{
	public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> instance, TValue value, out TKey key)
	{
		foreach (var entry in instance)
		{
			if (!entry.Value.Equals(value))
			{
				continue;
			}
			key = entry.Key;
			return true;
		}
		key = default(TKey);
		return false;
	}

	//string to int
	public static int ToInt (this string vValue)
	{
		try
		{
			return System.Convert.ToInt32(vValue);
		}
		catch
		{
			//Debug.LogError("ToInt's string is : " + vValue);
			return 0;
		}
	}

    //string to uint
    public static uint ToUint (this string vValue)
    {
        try
        {
            return System.Convert.ToUInt32(vValue);
        }
        catch
        {
            //Debug.LogError("ToUint's string is : " + vValue);
            return 0;
        }
    }

    //string to float
    public static float ToFloat (this string vValue)
    {
        try
        {
            return System.Convert.ToSingle(vValue);
        }
        catch
        {
            //Debug.LogError("ToFloat's string is : " + vValue);
            return 0f;
        }
    }
	
	//string to double
	public static double ToDouble (this string vValue)
	{
		try
		{
			return System.Convert.ToDouble(vValue);
		}
		catch
		{
			//Debug.LogError("ToDouble's string is : " + vValue);
			return 0f;
		}
	}
	
	//double to Datetime
	public static DateTime ToDateTime (this double vValue)
	{
		try
		{
			const double DAYS_BETWEEN_00010101_AND_18991230 = 693593;
			
			const double TIME_UNIT = 864000000000;
			
			return new DateTime((long)((vValue + DAYS_BETWEEN_00010101_AND_18991230) * TIME_UNIT));
		}
		catch
		{
			//Debug.LogError("ToDateTime's double is : " + vValue);
			return new DateTime();
		}
	}

    //string to Byte[]
    public static byte[] ToByteArray (this string vStr)
    {
        try
        {
            return System.Text.UTF8Encoding.UTF8.GetBytes(vStr);
        }
        catch
        {
            //Debug.LogError("ToByteArray's string is : " + vStr);
            return null;
        }
    }

	//string to MD5
	public static string ToMD5 (this string vStr)
	{
		try {
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			
			byte[] t = md5.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes(vStr));
			
			StringBuilder sb = new StringBuilder(32);
			
			for (int i = 0; i < t.Length; i++)
			{
				sb.Append(t[i].ToString("x").PadLeft(2, '0'));
			}
			
			return sb.ToString();
		}
		catch
		{
			//Debug.LogError("ToMD5's string is : " + vStr);
			return string.Empty;
		}
	}
}
