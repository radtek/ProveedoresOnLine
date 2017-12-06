using ADO.Models;
using HtmlAgilityPack;
using ProveedoresOnLine.OnlineSearch.Interfaces;
using ProveedoresOnLine.OnlineSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProveedoresOnLine.OnlineSearch.Core
{
    public class ProveedoresOnLineRUESImplement : IOnLineSearch
    {
        public async Task<List<Tuple<string, List<string>, List<string>>>> Search(int IdentificationType, string Name, string IdentificationNumber)
        {
            try
            {
                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();
                string Url = InternalSettings.Instance[Models.Constants.RUES_Url].Value;
                Uri uri = new Uri(Url);
               
                using (var client = new HttpClient())
                {
                    var Content = new FormUrlEncodedContent(new[]
                    {  
                        new KeyValuePair<string, string>(InternalSettings.Instance[Models.Constants.RUES_txtParam].Value, IdentificationNumber),  
                    });
                    var result = await client.PostAsync(Url, Content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    HtmlDocument HtmlDocResponse = new HtmlDocument();
                    HtmlDocResponse.LoadHtml(resultContent);
                    List<string> stringResult = new List<string>();
                    stringResult.Add(HtmlDocResponse.DocumentNode.InnerText);
                    oDetailinfo.Add(new Tuple<string, List<string>, List<string>>(null, stringResult, null));
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
