using HtmlAgilityPack;
using ProveedoresOnLine.OnlineSearch.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

                string Url = Models.InternalSettings.Instance[Models.Constants.JudicialProcess_Url].Value;
                string urlResult = Models.InternalSettings.Instance[Models.Constants.JudicialProcessResult_Url].Value;
                HttpClientHandler handler = new HttpClientHandler();
                handler.UseCookies = true;
                handler.Proxy = new WebProxy(Models.InternalSettings.Instance[Models.Constants.JudicialP_Proxy].Value, true);
                handler.UseProxy = true;

                using (var client = new HttpClient(handler as HttpMessageHandler))
                {
                    var Content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("cbadju", IdentificationType.ToString()),
                        new KeyValuePair<string, string>("norad", IdentificationNumber),
                        new KeyValuePair<string, string>("Buscar", "Buscar"),
                    });

                    Uri myUri = new Uri(Url, UriKind.Absolute);
                    var result = await client.PostAsync(myUri, Content);
                    string resultContent = result.Content.ReadAsStringAsync().Result;
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
                //List<Tuple<string, List<string>, List<string>>> oCatchReturn = new List<Tuple<string, List<string>, List<string>>>();
                //List<string> oDetailError = new List<string>();
                //oDetailError.Add("RAMA JUDICIAL NO DISPONIBLE");
                //oDetailError.Add(ex.InnerException.ToString());
                //oCatchReturn.Add(new Tuple<string, List<string>, List<string>>(null, null, oDetailError));
                return null;                
            }            
        }
    }
}
