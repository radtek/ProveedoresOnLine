using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading;
using ProveedoresOnLine.OnlineSearch.Models;
using Autofac;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Http.Headers;
using ProveedoresOnLine.OnlineSearch.Interfaces;

namespace ProveedoresOnLine.OnlineSearch.Core
{
    public class ProveedoresOnLineRegImplement : IOnLineSearch   
    {

        public async Task<List<Tuple<string, List<string>, List<string>>>> Search(int IdentificationType, string Name, string IdentificationNumber)
        {
            try
            {   
                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();
                
                string Url = ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.RegisterServiceURL].Value;
                Url = Url.Replace("{TID}", IdentificationType.ToString());
                Url = Url.Replace("{NID}", IdentificationNumber.ToString());
                Uri uri = new Uri(Url);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Url);
                    client.DefaultRequestHeaders.Add("Authorization", InternalSettings.Instance[Constants.RegisterServiceToken].Value);

                    HttpResponseMessage response = await client.GetAsync(Url);
                    HtmlDocument HtmlDoc = new HtmlDocument();
                    HtmlDoc.LoadHtml(response.Content.ReadAsStringAsync().Result);
                    if (HtmlDoc.DocumentNode.InnerText != "\"Error\"")
                    {
                        Tuple<string, List<string>, List<string>> oDetail = new Tuple<string, List<string>, List<string>>(HtmlDoc.DocumentNode.InnerText, null, null);
                        oDetailinfo.Add(oDetail);
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
