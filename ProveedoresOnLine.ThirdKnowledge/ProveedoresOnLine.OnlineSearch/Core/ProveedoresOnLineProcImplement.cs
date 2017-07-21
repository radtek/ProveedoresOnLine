using ProveedoresOnLine.OnlineSearch.Interfaces;
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

namespace ProveedoresOnLine.OnlineSearch.Core
{
    public class ProveedoresOnLineProcImplement : IOnLineSearch
    {
        public async Task<List<Tuple<string, List<string>, List<string>>>> Search(int IdentificationType, string Name, string IdentificationNumber)
        {
           try
            {             
                string __EVENTVALIDATION = "";
                string __VIEWSTATE = "";
                string __Question = "";
                string Answer = "";
                string oDivResult = ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_Url].Value + "::ErrorPage:: 503";
                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();

                HttpClientHandler handler = new HttpClientHandler()
                {
                    UseCookies = true,
                    //Proxy = new WebProxy(Models.InternalSettings.Instance[Models.Constants.JudicialP_Proxy].Value, true),
                    UseProxy = true,                   
                };

                string Url = ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_Url].Value;
                Uri uri = new Uri(Url);
                handler.CookieContainer = new CookieContainer();

                handler.CookieContainer.Add(uri, new Cookie("__utma", "203843921.750141917.1498165873.149875363")); // Adding a Cookie
                handler.CookieContainer.Add(uri, new Cookie("ASP.NET_SessionId", "dpvuetelwaq24pb5msjl5svb")); // Adding a Cookie
                handler.CookieContainer.Add(uri, new Cookie("ASP.NET_SessionId", "dpvuetelwaq24pb5msjl5svb")); // Adding a Cookie
                using (var client = new HttpClient(handler as HttpMessageHandler))
                {
                    System.Net.ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 |
                    SecurityProtocolType.Tls11 |
                    SecurityProtocolType.Tls;

                    ServicePointManager
    .ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) => true;

                    HttpWebRequest request = HttpWebRequest.CreateHttp(Url);
                    request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    client.BaseAddress = new Uri(Url);
                    HttpResponseMessage response = await client.GetAsync(Url);
                    HtmlDocument HtmlDoc = new HtmlDocument();
                    HtmlDoc.LoadHtml(response.Content.ReadAsStringAsync().Result);

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.BadGateway ||
                        response.StatusCode == System.Net.HttpStatusCode.GatewayTimeout || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        return null;
                    }
                    __VIEWSTATE = HtmlDoc.GetElementbyId(ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_VIEWSTATE].Value).GetAttributeValue("value", "");
                    __EVENTVALIDATION = HtmlDoc.GetElementbyId(ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_EVENTVALIDATION].Value).GetAttributeValue("value", "");
                    __Question = HtmlDoc.GetElementbyId(ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_Questionlbl].Value).InnerText;

                    while (string.IsNullOrEmpty(Answer))
                    {
                        if (__Question != null)
                        {
                            TreeModel oQuestionResult = new TreeModel();
                            oQuestionResult = ProveedoresOnLine.OnlineSearch.Controller.SearchController.GetAnswerByTreeidAndQuestion(102001, __Question);

                            if (oQuestionResult != null)
                            {
                                Answer = oQuestionResult.TreeItem.FirstOrDefault().ChildItem.ItemName;
                            }
                            else
                            {
                                response = await client.GetAsync(Url);
                                HtmlDoc = new HtmlDocument();
                                HtmlDoc.LoadHtml(response.Content.ReadAsStringAsync().Result);

                                __VIEWSTATE = HtmlDoc.GetElementbyId(ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_VIEWSTATE].Value).GetAttributeValue("value", "");
                                __EVENTVALIDATION = HtmlDoc.GetElementbyId(ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_EVENTVALIDATION].Value).GetAttributeValue("value", "");
                                __Question = HtmlDoc.GetElementbyId(ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_Questionlbl].Value).InnerText;
                                TreeModel oInnerQuestionResult = new TreeModel();
                                oInnerQuestionResult = ProveedoresOnLine.OnlineSearch.Controller.SearchController.GetAnswerByTreeidAndQuestion(102001, __Question);
                                Answer = oInnerQuestionResult.TreeItem.FirstOrDefault().ChildItem.ItemName;
                            }
                        }
                    }

                    var Content = new FormUrlEncodedContent(new[]
                    {
                        //new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_EVENTTARGET].Value, ""),
                        //new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_EVENTARGUMENT].Value, ""),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_VIEWSTATE].Value, "/wEPDwULLTIxNDMxNDEwMzIPFgIeCklkUHJlZ3VudGEFAjE1FgICAw9kFgoCAQ8PFgIeBFRleHQFGENvbnN1bHRhIGRlIGFudGVjZWRlbnRlc2RkAg0PFgIeB1Zpc2libGVoFgQCAQ9kFgICAQ8QZGQWAWZkAgMPZBYCAgEPEGRkFgBkAg8PDxYCHwEFN8K/IEN1YWwgZXMgbGEgQ2FwaXRhbCBkZWwgVmFsbGUgZGVsIENhdWNhPyAoU2luIHRpbGRlKT9kZAIYDw8WAh8CaBYCHgdvbmNsaWNrBR4kKCcjdHh0Q2FwdGNoYScpLnJlYWxwZXJzb24oKTtkAiQPDxYCHwEFB1YuMC4wLjRkZBgBBR5fX0NvbnRyb2xzUmVxdWlyZVBvc3RCYWNrS2V5X18WAQUMSW1hZ2VCdXR0b24xK3w2+wIgQnERphHHqFPN3ChZNDQ="),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_VIEWSTATEGENERATOR].Value, "D8335CE7"),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_EVENTVALIDATION].Value, "/wEWCQK7rp7sDAL8kK+TAQLwkOOQAQLxkOOQAQL0kOOQAQK8zP8SAtLCmdMIAsimk6ECApWrsq8IBWyHcTn+DYdhKSllIj89FzDnfHk="),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_TipoID].Value, IdentificationType.ToString()),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_NumID].Value, IdentificationNumber),
                        new KeyValuePair<string, string>(InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_QAnswer].Value, "Cali"),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_btnSearch].Value, "Consultar"),
                    });
                    var result = await client.PostAsync(Url, Content);
                    string resultContent = await result.Content.ReadAsStringAsync();
                    HtmlDocument HtmlDocResponse = new HtmlDocument();
                    HtmlDocResponse.LoadHtml(resultContent);

                    oDivResult = HtmlDocResponse.GetElementbyId(ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_divSec].Value).InnerText;

                    string procName = "";
                    if (HtmlDocResponse.DocumentNode.SelectNodes("//table[contains(@class, 'tablas')]") != null)
                    {
                        HtmlDocResponse.DocumentNode.SelectNodes("//table[contains(@class, 'tablas')]").All(tbls =>
                        {
                            if (HtmlDocResponse.DocumentNode.SelectNodes("//div[contains(@class, 'datosConsultado')]")[0].SelectNodes("span") != null)
                            {
                                procName = "";
                                HtmlDocResponse.DocumentNode.SelectNodes("//div[contains(@class, 'datosConsultado')]")[0].SelectNodes("span").All(x =>
                                {
                                    procName += x.InnerText + " ";
                                    return true;
                                });

                            }
                            List<string> hRows = new List<string>();
                            HtmlNodeCollection hCells = tbls.SelectNodes("tr")[0].SelectNodes("th");
                            hCells.All(x =>
                            {
                                hRows.Add(x.InnerText);
                                return true;
                            });
                            if (tbls.SelectNodes("tbody/tr") != null)
                            {
                                tbls.SelectNodes("tbody/tr").All(row =>
                                {
                                    List<string> dRows = new List<string>();
                                    HtmlNodeCollection cells = row.SelectNodes("td");
                                    for (int i = 0; i < cells.Count; i++)
                                    {
                                        dRows.Add(cells[i].InnerHtml);
                                    }

                                    Tuple<string, List<string>, List<string>> oDetail = new Tuple<string, List<string>, List<string>>(procName, hRows, dRows);
                                    oDetailinfo.Add(oDetail);
                                    return true;
                                });
                                return true;
                            }
                            else if (tbls.SelectNodes("tr") != null)
                            {
                                for (int i = 0; i < tbls.SelectNodes("tr").Count; i++)
                                {
                                    if (i != 0)
                                    {
                                        List<string> dRows = new List<string>();
                                        HtmlNodeCollection cells = tbls.SelectNodes("tr")[i].SelectNodes("td");
                                        for (int c = 0; c < cells.Count; c++)
                                        {
                                            dRows.Add(cells[c].InnerHtml);
                                        }

                                        Tuple<string, List<string>, List<string>> oDetail = new Tuple<string, List<string>, List<string>>(procName, hRows, dRows);
                                        oDetailinfo.Add(oDetail);
                                    }
                                }
                                return true;
                            }
                            return true;
                        });
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
