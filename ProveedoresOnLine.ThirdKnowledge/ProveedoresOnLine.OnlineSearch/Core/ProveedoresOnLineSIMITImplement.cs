using HtmlAgilityPack;
using ProveedoresOnLine.OnlineSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Core
{
    public class ProveedoresOnLineSIMITImplement
    {
        public async Task<List<Tuple<string, List<string> , List<string>>>> Search(int IdentificationType, string Name, string IdentificationNumber)
        {
            try
            {
                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();

                string Url = "http://procesos.ramajudicial.gov.co/consultaprocesos/ConsultaJusticias21.aspx?EntryId=YZGGxvuiZlXxbk0HoFK%2fFNYUHGc%3d";
                Uri uri = new Uri(Url);

                using (var client = new HttpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    HttpWebRequest request = HttpWebRequest.CreateHttp(Url);

                    client.BaseAddress = new Uri(Url);
                    client.DefaultRequestHeaders.Add("Authorization", "1M72112Z");

                    HttpResponseMessage response = await client.PostAsync(Url, null);
                    HtmlDocument HtmlDoc = new HtmlDocument();
                    HtmlDoc.LoadHtml(response.Content.ReadAsStringAsync().Result);
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
