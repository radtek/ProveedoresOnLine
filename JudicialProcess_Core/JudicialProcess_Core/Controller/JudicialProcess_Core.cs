using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JudicialProcess_Core.Models;
using HtmlAgilityPack;

namespace JudicialProcess_Core.Controller
{
    public  class JudicialProcess_Core
    {
        public static async Task<string> Search(int IdentificationType, string Name, string IdentificationNumber, string Token)
        {
            try
            {
                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();

                string Url = Models.InternalSettings.Instance[Constans.JudicialProcess_Url].Value;
                string urlResult = Models.InternalSettings.Instance[Constans.JudicialProcessResult_Url].Value;
                HttpClientHandler handler = new HttpClientHandler();
                handler.UseCookies = true;
                handler.Proxy = new WebProxy(Models.InternalSettings.Instance[Constans.JudicialP_Proxy].Value, true);
                handler.UseProxy = true;
                string innerResultContent = "<label> No se encontró ninguna coincidencia por Identificación" + IdentificationNumber + "</label>";
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

                        link = urlResult.Replace("{link}", HtmlDocResponse.DocumentNode.SelectNodes("//tr[contains(@class, 'e_tablas')]//tr//td//a")[0].InnerText).Replace("{datetime}", DateTime.Now.ToString("dd/MM/yyyy"));

                        var innerResult = await client.GetAsync(link);
                        innerResultContent = innerResult.Content.ReadAsStringAsync().Result;
                    }
                }

                Thread.Sleep(1000);

                //Todo
                return innerResultContent;
            }
            catch (Exception ex)
            {
                //List<Tuple<string, List<string>, List<string>>> oCatchReturn = new List<Tuple<string, List<string>, List<string>>>();
                //List<string> oDetailError = new List<string>();
                //oDetailError.Add("RAMA JUDICIAL NO DISPONIBLE");
                //oDetailError.Add(ex.InnerException.ToString());
                //oCatchReturn.Add(new Tuple<string, List<string>, List<string>>(null, null, oDetailError));
                return "";
            }
        }
    }
}
