using GromaxMobileApis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GromaxMobileApis.Utilities
{
    public static class WhatsappMessageSend
    {
        public class DealerAppointmentRequest
        {
            public string MobileNumber { get; set; }
            public string WhatsappUserName { get; set; }
            public string ProspectName { get; set; }
            public string ProspectMobile { get; set; }
            public string InterestedDistrict { get; set; }
            public string InterestedLocation { get; set; }
            public string CurrentBusiness { get; set; }
        }

        public class WhatsappSendResponse
        {
            public bool Success { get; set; }
            public string MessageId { get; set; }
            public string MessageStatus { get; set; }
            public string WaId { get; set; }
            public string RawResponse { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class smartpingresponseRound
        {
            public string messaging_product { get; set; }
            public List<contactss> contacts { get; set; }
            public List<messageRe> messages { get; set; }

        }

        public class contactss
        {
            public string input { get; set; }
            public string wa_id { get; set; }
        }

        public class messageRe
        {
            public string id { get; set; }
        }

        public static async Task<string> SendDealerAppointmentAsync(DealerAppointmentRequest model)
        {
            try
            {
                string[] tvariables =
                {
            model.WhatsappUserName,
            model.ProspectName,
            model.ProspectMobile,
            model.InterestedDistrict,
            model.InterestedLocation,
            model.CurrentBusiness
                };

                string parametersJson = "";

                if (tvariables != null && tvariables.Length > 0)
                {
                    for (int i = 0; i < tvariables.Length; i++)
                    {
                        parametersJson += @"                    {" + "\n" +
                                          @"                        ""type"": ""text""," + "\n" +
                                          @"                        ""text"": """ + (tvariables[i] ?? "") + @"""" + "\n" +
                                          @"                    }";

                        if (i < tvariables.Length - 1)
                            parametersJson += "," + "\n";
                    }
                }

                string body =
        @"{
    ""messaging_product"": ""whatsapp"",
    ""recipient_type"": ""individual"",
    ""to"": """ + model.MobileNumber + @""",
    ""type"": ""template"",
    ""template"": {
        ""name"": ""dealer_appointment"",
        ""language"": {
            ""code"": ""en""
        },
        ""components"": [
            {
                ""type"": ""body"",
                ""parameters"": [
" + parametersJson + @"
                ]
            }
        ]
    }
}";

                var client = new RestClient("https://partnersv1.pinbot.ai/v3/1135769352960189/messages");
                var request = new RestRequest();
                request.Method = Method.Post;

                request.AddHeader("apikey", "d2413074-71e6-11f1-894a-02c8a5e042bd");
                request.AddHeader("Content-Type", "application/json");

                request.AddParameter("application/json", body, ParameterType.RequestBody);


                var response = client.Execute(request);
                if (response != null && response.Content != null)
                    return response.Content.ToString();
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> getMessageString(DealerAppointmentRequest model)
        {
            try
            {
                string[] tvariables =
                {
            model.WhatsappUserName,
            model.ProspectName,
            model.ProspectMobile,
            model.InterestedDistrict,
            model.InterestedLocation,
            model.CurrentBusiness
                };


                string messageTemplate = "Dear {0},\n\nA new Gromax Dealer Appointment (NDA) enquiry has been registered in your state.\n\nEnquiry Details:\n* Prospect Name: {1}\n* Mobile Number: {2}\n* Interested District: {3}\n* Interested Location: {4}\n* Current Business: {5}\n\nKindly review the enquiry and coordinate with the concerned Area Manager/Territory Manager for further evaluation and follow-up.\n\nRegards,\nGromax Agri Equipment Ltd.";
                string message = string.Format(messageTemplate, tvariables);
                return message;
            }
            catch
            {
                throw;
            }
        }


        public static async Task<string> SendAdvances90DaysMoreAsync(AdvanceMoreThan90Whatsapp model)
        {
            try
            {
                string[] tvariables =
                {
                    model.stateHeadName,
                    model.stateName,
                    model.dealerCode,
                    model.location,
                    model.customerName,
                    model.deliveryDate,
                    model.financeStatus
                };

                string parametersJson = "";

                if (tvariables != null && tvariables.Length > 0)
                {
                    for (int i = 0; i < tvariables.Length; i++)
                    {
                        parametersJson += @"                    {" + "\n" +
                                          @"                        ""type"": ""text""," + "\n" +
                                          @"                        ""text"": """ + (tvariables[i] ?? "") + @"""" + "\n" +
                                          @"                    }";

                        if (i < tvariables.Length - 1)
                            parametersJson += "," + "\n";
                    }
                }

                string body =
        @"{
    ""messaging_product"": ""whatsapp"",
    ""recipient_type"": ""individual"",
    ""to"": """ + model.stateHeadMobile + @""",
    ""type"": ""template"",
    ""template"": {
        ""name"": ""90_day_not_retail"",
        ""language"": {
            ""code"": ""en""
        },
        ""components"": [
            {
                ""type"": ""body"",
                ""parameters"": [
" + parametersJson + @"
                ]
            }
        ]
    }
}";

                var client = new RestClient("https://partnersv1.pinbot.ai/v3/1135769352960189/messages");
                var request = new RestRequest();
                request.Method = Method.Post;

                request.AddHeader("apikey", "d2413074-71e6-11f1-894a-02c8a5e042bd");
                request.AddHeader("Content-Type", "application/json");

                request.AddParameter("application/json", body, ParameterType.RequestBody);


                var response = client.Execute(request);
                if (response != null && response.Content != null)
                    return response.Content.ToString();
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> Get90DaysAgeingMessage(AdvanceMoreThan90Whatsapp model)
        {
            try
            {
                string[] tvariables =
                {
            model.stateHeadName,
            model.stateName,
            model.dealerCode,
            model.location,
            model.customerName,
            model.deliveryDate,
            model.financeStatus
        };

                string messageTemplate = "Dear {0},\n\nThe following bank advance has crossed the 90-day ageing period.\n\nDetails:\n* State Name: {1}\n* SAP Code: {2}\n* Location: {3}\n* Customer Name: {4}\n* Date of Delivery: {5}\n* Status: {6}\n\nKindly treat this as urgent and update the CRM at the earliest.\n\nThank you";

                string message = string.Format(messageTemplate, tvariables);
                return message;
            }
            catch
            {
                throw;
            }
        }


    }
}
