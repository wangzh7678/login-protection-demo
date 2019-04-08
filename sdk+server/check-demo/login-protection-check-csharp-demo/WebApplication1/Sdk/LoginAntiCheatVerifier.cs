﻿﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace LoginAntiCheatAPIDemo.Sdk
{

    public enum VerifyResultType
    {
        Success = 0,
        Suspicion = 10,
        ConfirmedCheat = 20,
        Error = -1,
    };

    /// <summary>
    /// 易盾验证码二次校验接口简单封装demo
    /// </summary>
    public class LoginAntiCheatVerifier
    {
        public static string VERIFY_API = "https://ac.dun.163yun.com/v2/login/check"; // verify接口地址

        private NESecretPair secretPair; // 密钥对
        private readonly string VERSION = "200";
        private readonly HttpClient client = Utils.makeHttpClient();
        private string BUSINESS_ID;

        public LoginAntiCheatVerifier( NESecretPair secretPair, string businessID)
        {
            this.secretPair = secretPair;
            BUSINESS_ID = businessID;
        }
        
        /// <summary>
        /// check函数封装
        /// </summary>
        /// <param name="token">sdk传递的token</param>
        /// <returns></returns>
        public VerifyResultType verify(string token)
        {
            //can add your own user validation logic 


            Dictionary<String, String> parameters = new Dictionary<String, String>();
            long curr = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds/1000;
            String time = curr.ToString();


            // 1.设置公共参数
            parameters.Add("businessId", BUSINESS_ID);
            parameters.Add("secretId", secretPair.secretId);
            parameters.Add("version", VERSION);
            parameters.Add("timestamp", time);
            parameters.Add("nonce", new Random().Next().ToString());
            parameters.Add("token", token);

            //可选参数 尽量详细添加
            //parameters.Add("account", "100001");
            //parameters.Add("email", "zhangsanzuiniu@163.com");
            //parameters.Add("phone", "18888888888");
            //parameters.Add("loginIp", "123.123.123.120");
            //parameters.Add("registerTime", "1479178545");
            //parameters.Add("registerIp", "123.123.123.123");

            // 2.生成签名信息
            String signature = Utils.genSignature(secretPair.secretKey, parameters);
            parameters.Add("signature", signature);

            // 3.发送HTTP请求
            String response = Utils.doPost(client, VERIFY_API, parameters, 5000);
            return verifyRet(response);
        }

        /// <summary>
        /// 解析check返回的结果
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private VerifyResultType verifyRet(string response)
        {
            VerifyResultType returnresult = VerifyResultType.Success;
            if (String.IsNullOrEmpty(response))
            {
                returnresult = VerifyResultType.Error;
                return returnresult;
            }
            try
            {
                JObject ret = JObject.Parse(response);
                int code = ret.GetValue("code").ToObject<Int32>();
                String msg = ret.GetValue("msg").ToObject<String>();
                if (code == 200)
                {
                    JObject array = (JObject)ret.SelectToken("result");
                    int actioncode = array.GetValue("action").ToObject<Int32>();
                    int hittype = array.GetValue("hitType").ToObject<Int32>();

                    returnresult = (VerifyResultType)actioncode;
                }
                else
                {
                    Console.WriteLine("error: {0}", msg);
                    returnresult = VerifyResultType.Error;
                }
            }
            catch (Exception e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
                returnresult = VerifyResultType.Error;
            }
            return returnresult;
        }
    }

    /// <summary>
    /// 业务密钥对
    /// </summary>
    public class NESecretPair
    {
        public string secretId; // 密钥对id
        public string secretKey; // 密钥对key

        public NESecretPair(string secretId, string secretKey)
        {
            this.secretId = secretId;
            this.secretKey = secretKey;
        }
    }
}