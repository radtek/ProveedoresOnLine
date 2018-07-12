using HtmlAgilityPack;
using Newtonsoft.Json;
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
                    List<string> stringJsonResult = new List<string>();
                    stringJsonResult.Add(HtmlDocResponse.DocumentNode.InnerText);

                    List<string> stringResult = new List<string>();                   

                    RuesModel deserializedProduct = JsonConvert.DeserializeObject<RuesModel>(stringJsonResult.FirstOrDefault());
                    if (deserializedProduct.codigo_error == "0000")
                    {
                        deserializedProduct.rows = deserializedProduct.rows.Where(x => !string.IsNullOrEmpty(x.detalleESAL)
                                            || !string.IsNullOrEmpty(x.detalleRM)
                                            || !string.IsNullOrEmpty(x.detalleRUP)
                                            || !string.IsNullOrEmpty(x.detalleRNT)).Select(x => x).ToList();
                        HtmlNode link = null;
                        if (!string.IsNullOrEmpty(deserializedProduct.rows.FirstOrDefault().detalleRM))
                        {
                            HtmlDocResponse.LoadHtml(deserializedProduct.rows.FirstOrDefault().detalleRM);
                            link = HtmlDocResponse.DocumentNode
                                          .Descendants("a")
                                          .First(x => x.Attributes["class"] != null
                                                   && x.Attributes["class"].Value == "rm");
                        }
                        else if (!string.IsNullOrEmpty(deserializedProduct.rows.FirstOrDefault().detalleESAL))
                        {
                            HtmlDocResponse.LoadHtml(deserializedProduct.rows.FirstOrDefault().detalleESAL);
                            link = HtmlDocResponse.DocumentNode
                                          .Descendants("a")
                                          .First(x => x.Attributes["class"] != null
                                                   && x.Attributes["class"].Value == "esal");
                        }
                        else if (!string.IsNullOrEmpty(deserializedProduct.rows.FirstOrDefault().detalleRUP))
                        {
                            HtmlDocResponse.LoadHtml(deserializedProduct.rows.FirstOrDefault().detalleRUP);
                            link = HtmlDocResponse.DocumentNode
                                          .Descendants("a")
                                          .First(x => x.Attributes["class"] != null
                                                   && x.Attributes["class"].Value == "rupoff"
                                                   || x.Attributes["class"].Value == "rup");
                        }
                        else if (!string.IsNullOrEmpty(deserializedProduct.rows.FirstOrDefault().detalleRNT))
                        {
                            HtmlDocResponse.LoadHtml(deserializedProduct.rows.FirstOrDefault().detalleRNT);
                            link = HtmlDocResponse.DocumentNode
                                          .Descendants("a")
                                          .First(x => x.Attributes["class"] != null
                                                   && x.Attributes["class"].Value == "rnt");
                        }

                        string linkPath = InternalSettings.Instance[Models.Constants.RUES_Domine].Value + link.Attributes["href"].Value;

                        stringResult.Add(deserializedProduct.rows.FirstOrDefault().nit);
                        stringResult.Add(deserializedProduct.rows.FirstOrDefault().razSol);
                        stringResult.Add(deserializedProduct.rows.FirstOrDefault().desc_categoria_matricula);                        
                        stringResult.Add(linkPath);
                    }                                    

                    oDetailinfo.Add(new Tuple<string, List<string>, List<string>>(null, stringResult, null));
                }
                Thread.Sleep(1000);
                //Todo
                return oDetailinfo;
            }
            catch (ThreadAbortException ex)
            {
                return null;                
            };
        }       
    }
}
