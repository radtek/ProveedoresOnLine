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
            try
            {
                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();
                using (var client = new HttpClient())
                {
                    string Url = Models.InternalSettings.Instance[Models.Constants.JudicialProcess_Url].Value;
                    string urlResult = Models.InternalSettings.Instance[Models.Constants.JudicialProcessResult_Url].Value;
                    var Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("cbadju", IdentificationType.ToString()),
                        new KeyValuePair<string, string>("norad", IdentificationNumber),
                        new KeyValuePair<string, string>("Buscar", "Buscar"),
                    });
                    var result = await client.PostAsync(Url, Content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    HtmlDocument HtmlDocResponse = new HtmlDocument();
                    HtmlDocResponse.LoadHtml(resultContent);

                    string link = string.Empty;
                    if (HtmlDocResponse.DocumentNode.SelectNodes("//tr[contains(@class, 'e_tablas')]//tr//td") != null)
                    {
                        List<string> detResult = new List<string>();

                        detResult.Add(HtmlDocResponse.DocumentNode.SelectNodes("//tr[contains(@class, 'e_tablas')]//tr//td")[1].InnerText);
                        detResult.Add(HtmlDocResponse.DocumentNode.SelectNodes("//tr[contains(@class, 'e_tablas')]//tr//td")[2].InnerText);
                        link = urlResult.Replace("{link}", HtmlDocResponse.DocumentNode.SelectNodes("//tr[contains(@class, 'e_tablas')]//tr//td//a")[0].InnerText).Replace("{datetime}", DateTime.Now.ToString("dd/MM/yyyy"));
                        oDetailinfo.Add(new Tuple<string, List<string>, List<string>>(link, detResult, null));
                    }                    
                }
                Thread.Sleep(1000);
                //Todo
                return oDetailinfo;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
           
        }
    }
}
