using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ResendEmailClient
{
    public class ResendService
    {
        private readonly string apiKey;
        private readonly string apiUrl = "https://api.resend.com/emails";

        public ResendService(string apiKey)
        {
            this.apiKey = apiKey;
        }
        [HttpPost]
        public async Task SendEmailAsync(string from, string to, string subject, string htmlBody)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var payload = new
                {
                    from = from,
                    to = to,
                    subject = subject,
                    html = htmlBody
                };

                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Correo enviado correctamente:");
                    Console.WriteLine(responseBody);
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error al enviar el correo:");
                    Console.WriteLine(errorBody);
                }
            }
        }
    }
}
