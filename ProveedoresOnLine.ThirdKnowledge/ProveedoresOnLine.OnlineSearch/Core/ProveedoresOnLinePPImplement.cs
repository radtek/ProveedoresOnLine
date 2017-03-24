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
    public class ProveedoresOnLinePPImplement : IOnLineSearch
    {
        public async Task<List<Tuple<string, List<string>, List<string>>>> Search(int IdentificationType, string Name, string IdentificationNumber)
        {
            try
            {
                string oDivResult = Models.InternalSettings.Instance[Models.Constants.PanamaPaapers_Url].Value + "::ErrorPage:: 503";
                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();
                bool isReported = false;
                using (var client = new HttpClient())
                {
                    string Url = Models.InternalSettings.Instance[Models.Constants.PanamaPaapers_Url].Value + IdentificationType.ToString() + "&q=" + Name;
                    client.BaseAddress = new Uri(Url);
                    HttpResponseMessage response = await client.GetAsync(Url);
                    HtmlDocument HtmlDoc = new HtmlDocument();
                    HtmlDoc.LoadHtml(response.Content.ReadAsStringAsync().Result);

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.BadGateway ||
                        response.StatusCode == System.Net.HttpStatusCode.GatewayTimeout || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return null;
                    }
                    if (HtmlDoc != null && HtmlDoc.DocumentNode.SelectNodes("//td[contains(@class, 'description')]") != null)
                    {
                        isReported = HtmlDoc.DocumentNode.SelectNodes("//td[contains(@class, 'description')]").Where(x => x.InnerText.Contains(Name)).Select(x => x).FirstOrDefault() != null ? true : false;
                    }
                }
                Thread.Sleep(1000);
                //Todo
                return oDetailinfo;
            }
            catch (ThreadAbortException ex)
            {
                throw ex.InnerException;
            };
        }
    }
}
