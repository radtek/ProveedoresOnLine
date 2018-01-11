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
    public class ProveedoresOnLineRegistreImplement : IOnLineSearch
    {
        public async Task<List<Tuple<string, List<string>, List<string>>>> Search(int IdentificationType, string Name, string IdentificationNumber)
        {
            List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();
            try
            {
                string Url = ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.RegisterPageUrl].Value.Replace("CC", IdentificationNumber);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Url);                    

                    HttpResponseMessage response = await client.GetAsync(Url);
                    HtmlDocument HtmlDoc = new HtmlDocument();
                    HtmlDoc.LoadHtml(response.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrEmpty(HtmlDoc.GetElementbyId("info").InnerHtml))
                    {
                        List<string> oResultQuery = new List<string>();
                        oResultQuery.Add(HtmlDoc.GetElementbyId("info").InnerHtml);
                        oDetailinfo.Add(new Tuple<string, List<string>, List<string>>(Url, oResultQuery, null));
                    }
                    return oDetailinfo;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
