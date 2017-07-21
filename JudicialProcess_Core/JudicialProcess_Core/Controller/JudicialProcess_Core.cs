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
using System.Web;
using JudicialProcess_Core.DAL;

namespace JudicialProcess_Core.Controller
{
    public class JudicialProcess_Core
    {
        public static async Task<string> Search(int IdentificationType, string Name, string IdentificationNumber, string Token)
        {
            try
            {
                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();

                string Url = InternalSettings.Instance[Constans.JudicialProcess_Url].Value;
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

                System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;
                string s_unicode = innerResultContent;
                byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(s_unicode);
                string s_unicode2 = System.Text.Encoding.UTF8.GetString(utf8Bytes);

                //Save Transaction
                TransactionModel oTransaction = new TransactionModel()
                {
                    Token = Token,
                    Message = s_unicode2,
                    Query = IdentificationNumber,
                    ServiceType = Models.Enumerations.enumServiceType.JudicialProcessService,
                    IsSuccess = true,
                };
                DAL.JudicialProcessDataController.Instance.SaveTransaction(oTransaction.Token, oTransaction.Message, oTransaction.Query, (int)oTransaction.ServiceType, oTransaction.IsSuccess);
                return s_unicode2;
            }
            catch (Exception ex)
            {
                TransactionModel oTransaction = new TransactionModel()
                {
                    Token = Token,
                    Message = ex.Message + " - " + ex.InnerException,
                    Query = IdentificationNumber,
                    ServiceType = Models.Enumerations.enumServiceType.JudicialProcessService,
                    IsSuccess = false,
                };
                DAL.JudicialProcessDataController.Instance.SaveTransaction(oTransaction.Token, oTransaction.Message, oTransaction.Query, (int)oTransaction.ServiceType, oTransaction.IsSuccess);
                return "Service temporally out of service";
            }
        }

        public static bool IsAuthorized(string Token)
        {
            return JudicialProcessDataController.Instance.IsAuthorized(Token);
        }
    }
}
