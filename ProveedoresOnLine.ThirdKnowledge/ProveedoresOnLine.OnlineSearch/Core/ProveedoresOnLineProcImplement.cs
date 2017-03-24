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
                using (var client = new HttpClient())
                {
                    string Url = ProveedoresOnLine.OnlineSearch.Models.InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_Url].Value;
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
                            Answer = oQuestionResult.TreeItem.FirstOrDefault().ChildItem.ItemName;

                            if (string.IsNullOrEmpty(Answer))
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
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_EVENTTARGET].Value, ""),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_EVENTARGUMENT].Value, ""),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_VIEWSTATE].Value, __VIEWSTATE),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_VIEWSTATEGENERATOR].Value, "D8335CE7"),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_EVENTVALIDATION].Value, __EVENTVALIDATION),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_TipoID].Value, IdentificationType.ToString()),
                        new KeyValuePair<string, string>(InternalSettings.Instance[Constants.Proc_NumID].Value, IdentificationNumber),
                        new KeyValuePair<string, string>(InternalSettings.Instance[ProveedoresOnLine.OnlineSearch.Models.Constants.Proc_QAnswer].Value, Answer),
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
                                if (HtmlDocResponse.DocumentNode.SelectNodes("//div[contains(@class, 'datosConsultado')]")[0].SelectNodes("span")  != null)
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
            catch (System.Threading.ThreadAbortException ex)
            {

                throw ex.InnerException;
            }
        }
    }
}
