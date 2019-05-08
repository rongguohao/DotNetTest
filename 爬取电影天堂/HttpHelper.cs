﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace 爬取电影天堂
{
    public class HttpHelper:IHttpHelper
    {
        public IHttpClientFactory Http;

        public HttpHelper(IHttpClientFactory http)
        {
            Http = http;
        }

        public async Task<string> GetHTMLByURL(string url)
        {
            try
            {
                var client = Http.CreateClient("dy");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var t = response.Content.ReadAsByteArrayAsync().Result;
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var ret = System.Text.Encoding.GetEncoding("GB2312").GetString(t);
                    return ret;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("http异常:"+ ex.ToString());
                return string.Empty;
            }
        }
    }
}
