using HtmlAgilityPack;
using Register_Core.DAL.Controller;
using Register_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Register_Core.Controller
{
    public class Register_Core
    {
        public static async Task<string> Search(int IdentificationType, string IdentificationNumber, string Token)
        {
            try
            {
                string Url = InternalSettings.Instance[Constants.Register_Url].Value;
                string NameResult = "";
                if (IdentificationType == 1)                
                    Url = Url.Replace("{1}", "cc").ToString() + IdentificationNumber;                
                else
                    Url = Url.Replace("{1}", "nit").ToString() + IdentificationNumber;

                List<Tuple<string, List<string>, List<string>>> oDetailinfo = new List<Tuple<string, List<string>, List<string>>>();
                
                using (var client = new HttpClient())
                {
                    Uri myUri = new Uri(Url, UriKind.Absolute);
                    var result = await client.GetAsync(myUri);
                    string resultContent = result.Content.ReadAsStringAsync().Result;
                    
                    HtmlDocument HtmlDocResponse = new HtmlDocument();
                    HtmlDocResponse.LoadHtml(resultContent);

                    if (HtmlDocResponse.DocumentNode.SelectNodes("//h3")[1] != null)
                    {
                        NameResult = HtmlDocResponse.DocumentNode.SelectNodes("//h3")[1].InnerHtml.Replace("Obtene un informe de: ", "").ToString();                        
                    }
                    else
                    {
                        NameResult = "No existe nombre o razón social";
                    }

                }
                Thread.Sleep(1000);

                //Save Transaction
                TransactionModel oTransaction = new TransactionModel()
                {
                    Token = Token,
                    Message = NameResult,
                    Query = IdentificationNumber,
                    ServiceType = Models.Enumerations.enumServiceType.RegisterService,
                    IsSuccess = true,
                };
                RegisterDataController.Instance.SaveTransaction(oTransaction.Token, oTransaction.Message, oTransaction.Query, (int)oTransaction.ServiceType, oTransaction.IsSuccess);
                return NameResult;
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
                RegisterDataController.Instance.SaveTransaction(oTransaction.Token, oTransaction.Message, oTransaction.Query, (int)oTransaction.ServiceType, oTransaction.IsSuccess);
                return "Service temporally out of service";
            }
        }
        public static bool IsAuthorized(string Token)
        {
            return RegisterDataController.Instance.IsAuthorized(Token);
        }
    }
}
