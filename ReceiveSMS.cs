using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Twilio.TwiML;
using System.Net.Http;
using System.Linq;

namespace TwilioTextDriver
{
    public static class ReceiveSMS
    {
        [FunctionName("ReceiveSMS")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var data = await req.Content.ReadAsStringAsync();
            var formValues = data.Split('&')
                .Select(value => value.Split('='))
                .ToDictionary(pair => Uri.UnescapeDataString(pair[0]).Replace("+", " "),
                              pair => Uri.UnescapeDataString(pair[1]).Replace("+", " "));

            // Perform calculations, API lookups, etc. here

            var response = new MessagingResponse();

            if (formValues["Body"].ToLower().Trim() == "hello")
            {
                response.Message($"Hello! Thank you for using the Driver Helpline.\n\n" +
                    $"Type 'SUMMARY' for a summary of your vehicle's health\n" +
                    $"'TIRE' for important tire metrics\n" +
                    $"'GAS' to check the gas level\n" +
                    $"'REPAIRS' to see recent repairs\n\n" +
                    $"Type 'HELLO' at any time to access this menu.");
            }
            // in each of these else if statements, we will hopefully be able to call the most recent data
            // to be updated later
            else if (formValues["Body"].ToLower().Trim() == "summary")
            {
                response.Message("You requested a summary of your vehicle's health.");
            }
            else if (formValues["Body"].ToLower().Trim() == "tire")
            {
                response.Message("You requested tire data.");
            }
            else if (formValues["Body"].ToLower().Trim() == "gas")
            {
                response.Message("You requested gas data.");
            }
            else if (formValues["Body"].ToLower().Trim() == "repairs")
            {
                response.Message("You requested repair data.");
            }
            else
            {
                response.Message("Your request was invalid. Please try again.\n\n" +
                    "Type 'HELLO' to access the main menu.");
            }


            var twiml = response.ToString();
            twiml = twiml.Replace("utf-16", "utf-8");

            return new HttpResponseMessage
            {
                Content = new StringContent(twiml, Encoding.UTF8, "application/xml")
            };

            // commented below is the body of the original function.
            /*
            var response = new MessagingResponse()
                .Message($"You said: {formValues["Body"]}");
            var twiml = response.ToString();
            twiml = twiml.Replace("utf-16", "utf-8");

            return new HttpResponseMessage
            {
                Content = new StringContent(twiml, Encoding.UTF8, "application/xml")
            };
            */

        }
    }
}
