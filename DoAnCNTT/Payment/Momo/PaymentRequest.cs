using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;
using System;
using System.Security.Cryptography;

namespace DoAnCNTT.Payment.Momo
{
    public class PaymentRequest
    {
        public PaymentRequest()
        {

        }
        public static async Task<string> sendPaymentRequestAsync(string endpoint, string postJsonString)
        {

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(postJsonString, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(endpoint, content);

                    response.EnsureSuccessStatusCode(); // Ensure success status code

                    return await response.Content.ReadAsStringAsync();
                }

            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

    }
}
