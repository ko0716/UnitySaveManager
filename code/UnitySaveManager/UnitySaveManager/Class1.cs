using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

/*
 * CREATED BY Hugu0141 with AI
 * 
 * If you have questions,Please check README.
 */

namespace UnitySaveManager
{
    public class UnitySaveAssist : MonoBehaviour
    {
        // AESで文字列を暗号化（Base64で返す）
        private static string KeyIssuence(string plainText, string key)
        {
            // 文字列→バイト配列（UTF8）
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            // 鍵とIVを準備
            using (Aes aes = Aes.Create())
            {
                aes.Key = CreateKeyBytes(key, 32);  // AES-256用に32バイト鍵を生成
                aes.GenerateIV();                   // 毎回ランダムなIVを使用
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // 暗号化実行
                using (var ms = new MemoryStream())
                {
                    // 先頭にIVを埋め込んでおく（復号時に必要）
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plainBytes, 0, plainBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    // Base64で返す
                    return Convert.ToBase64String(ms.ToArray());


                }
            }
        }


        // AESで文字列を復号化
        private static string Decode(string base64Cipher, string key)
        {
            byte[] cipherBytes = Convert.FromBase64String(base64Cipher);

            using (Aes aes = Aes.Create())
            {
                aes.Key = CreateKeyBytes(key, 32);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // 先頭のIV(16byte)を取り出す
                byte[] iv = new byte[16];
                Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                // 残りが暗号本文
                byte[] actualCipher = new byte[cipherBytes.Length - iv.Length];
                Array.Copy(cipherBytes, iv.Length, actualCipher, 0, actualCipher.Length);

                // 復号
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(actualCipher, 0, actualCipher.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        // 鍵文字列から指定バイト長の鍵を生成（SHA256ハッシュ利用）
        private static byte[] CreateKeyBytes(string key, int length)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
                if (hash.Length == length) return hash;

                byte[] keyBytes = new byte[length];
                Array.Copy(hash, keyBytes, Math.Min(hash.Length, length));
                return keyBytes;
            }
        }

        public static string Encrypt(string security_key, string originalText)
        {
            string encrypted = UnitySaveAssist.KeyIssuence(originalText, security_key);
            return encrypted;
        }



        //暗号化したテキストの取得
        public static string Create_AES_text(string security_key, dynamic originalText)
        {
            //暗号化識別子
            int num;

            //暗号化
            string encrypted = UnitySaveAssist.KeyIssuence(originalText.ToString(), security_key);


            //型に応じて分岐
            if (originalText is int)
            {
                do
                {
                    num = UnityEngine.Random.Range(100, 1000); // 100〜999のランダムな数
                }
                while (num % 3 != 0);

                //二重暗号化
                string twoAEStxt = UnitySaveAssist.KeyIssuence(encrypted,num.ToString());

                //識別子を追加
                encrypted = num.ToString() + twoAEStxt;

            }
            else if (originalText is string)
            {
                do
                {
                    num = UnityEngine.Random.Range(100, 1000); // 100〜999のランダムな数
                }
                while (num % 4 != 0);

                //二重暗号化
                string twoAEStxt = UnitySaveAssist.KeyIssuence(encrypted, num.ToString());

                //識別子を追加
                encrypted = num.ToString() + twoAEStxt;

            }
            else if (originalText is float)
            {
                do
                {
                    num = UnityEngine.Random.Range(100, 1000); // 100〜999のランダムな数
                }
                while (num % 7 != 0);

                //二重暗号化
                string twoAEStxt = UnitySaveAssist.KeyIssuence(encrypted, num.ToString());

                //識別子を追加
                encrypted = num.ToString() + twoAEStxt;
            }

             //値を返す
             return encrypted;
        }

        public static dynamic Decode_AES_text(string security_key, string Text)
        {
            int prefix = int.Parse(Text.Substring(0, 3));  // 先頭3文字を取得
            string decodetxt = Text.Substring(3); //残りの文字を抽出

            //暗号化された暗号化した文を復元
            string createdecrypted = UnitySaveAssist.Decode(decodetxt, prefix.ToString());

            //データの復元
            string decrypted = UnitySaveAssist.Decode(createdecrypted, security_key);


            //適応する型に変換
            if (prefix % 3 == 0)
            {
                return int.Parse(decrypted);
            }
            else if (prefix % 4 == 0)
            {
                return decrypted;
            }
            else if (prefix % 7 == 0)
            { 
                return float.Parse(decrypted);
            }


            // その他は default を返す（null / 0相当）
            return default;
        }



        public static void Save(string key_name, string security_key, dynamic originalText)
        {
            //originalTextがnullでないことを検証
            if (originalText != null)
            {
                //型がこれらのいずれかであることを検証
                if (originalText is int || originalText is float || originalText is string)
                {
                    //暗号化
                    string encrypted = UnitySaveAssist.KeyIssuence(originalText.ToString(), security_key);

                    try
                    {
                        PlayerPrefs.SetString(key_name, encrypted);
                    }
                    catch
                    {
                        UnityEngine.Debug.LogError("セーブ時にエラーが発生しました。");
                    }
                }



            }

        }

        public static T Load<T>(string key_name, string security_key)
        {
            try
            {
                //型に応じて分岐
                if (typeof(T) == typeof(int))
                {
                    string decodeText = PlayerPrefs.GetString(key_name);
                    string decrypted = UnitySaveAssist.Decode(decodeText, security_key);
                    return (T)(object)int.Parse(decrypted);
                }
                else if (typeof(T) == typeof(string))
                {
                    string decodeText = PlayerPrefs.GetString(key_name);
                    string decrypted = UnitySaveAssist.Decode(decodeText, security_key);
                    return (T)(object)decrypted;
                }
                else if (typeof(T) == typeof(float))
                {
                    string decodeText = PlayerPrefs.GetString(key_name);
                    string decrypted = UnitySaveAssist.Decode(decodeText, security_key);
                    return (T)(object)float.Parse(decrypted);
                }
            }

            catch
            {
                UnityEngine.Debug.LogError("読み込みに失敗しました");
            }

            // その他は default を返す（null / 0相当）
            return default;


        }

    }
}