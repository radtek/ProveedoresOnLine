using HtmlAgilityPack;
using ProveedoresOnLine.OnlineSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Core
{
    public class ProveedoresOnLineJudicialProcess : IOnLineSearch
    {
        public async Task<List<Tuple<string, List<string>, List<string>>>> Search(int IdentificationType, string Name, string IdentificationNumber)
        {
            List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();
            using (var client = new HttpClient())
            {
                string Url = "http://procesos.ramajudicial.gov.co/jepms/bogotajepms/lista.asp";
                var Content = new FormUrlEncodedContent(new[]
                   {
                        new KeyValuePair<string, string>("cbadju", "3"),
                        new KeyValuePair<string, string>("norad", IdentificationNumber),
                        new KeyValuePair<string, string>("Buscar", "Buscar"),
                    });
                var result = await client.PostAsync(Url, Content);
                string resultContent = await result.Content.ReadAsStringAsync();
                HtmlDocument HtmlDocResponse = new HtmlDocument();
                HtmlDocResponse.LoadHtml(resultContent);
                string searchResult = "";
                //searchResult = HtmlDocResponse.GetElementbyId().InnerText;
            }
            Thread.Sleep(1000);
            //Todo
            return oDetailinfo;
        }
    }
}
