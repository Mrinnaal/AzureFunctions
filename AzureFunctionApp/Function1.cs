using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace AzureFunctionApp
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("ExternalAPI")]
        public static async Task<HttpResponseMessage> ExternalAPICall(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Calling my External API");

            try
            {

                var mycontent = req.Content;
                string jsonContent = mycontent.ReadAsStringAsync().Result;
                dynamic actualparm = JsonConvert.DeserializeObject<WaveProductModel>(jsonContent);


                //if (string.IsNullOrEmpty(actualparm.WaveProductID))
                //{
                //    return req.CreateResponse(HttpStatusCode.OK, "Please enter a valid Wave Product Id!");
                //}

                HttpClient client = new HttpClient();
                HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://catfact.ninja/fact"));


                HttpResponseMessage httpresponse = await client.SendAsync(myRequest);
                WaveProductModel isValidWaveProduct = await httpresponse.Content.ReadAsAsync<WaveProductModel>();



                return req.CreateResponse(HttpStatusCode.OK, isValidWaveProduct);
            }
            catch (Exception ex)
            {

                return req.CreateResponse(HttpStatusCode.OK, "Wave API product ID is not valid !!! Reason: {0}", string.Format(ex.Message));
            }
        }

        [FunctionName("ExternalAPIWithQuery")]
        public static async Task<HttpResponseMessage> ExternalAPICallWithQuery(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Calling my External API");

            try
            {

                //To get query parameters -- example try like http://localhost:7071/api/ExternalAPIWithQuery?q=val&s=ans
                var query = System.Web.HttpUtility.ParseQueryString(req.RequestUri.Query);
                string q1 = query["q"]; //returns "val" as query parameter q=val
                string q2 = query["s"]; //returns "ans" as query parameter s=ans

                //To get authentication code from header - header key should be authKey
                var head = req.Headers.GetValues("authKey");


                //if (string.IsNullOrEmpty(actualparm.WaveProductID))
                //{
                //    return req.CreateResponse(HttpStatusCode.OK, "Please enter a valid Wave Product Id!");
                //}

                HttpClient client = new HttpClient();
                HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://catfact.ninja/fact"));


                HttpResponseMessage httpresponse = await client.SendAsync(myRequest);
                WaveProductModel isValidWaveProduct = await httpresponse.Content.ReadAsAsync<WaveProductModel>();



                return req.CreateResponse(HttpStatusCode.OK, isValidWaveProduct);
            }
            catch (Exception ex)
            {

                return req.CreateResponse(HttpStatusCode.OK, "Wave API product ID is not valid !!! Reason: {0}", string.Format(ex.Message));
            }
        }

        [FunctionName("ExternalAPIWithQuery2")]
        public static async Task<HttpResponseMessage> ExternalAPICallWithQuery2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "trial/{query2}")] HttpRequestMessage req, ILogger log, string query2)
        {
            log.LogInformation("Calling my External API");

            try
            {
                //Call api like this - http://localhost:7071/api/trial/result

                //To get the query after route
                var ans = query2; //you will get the return value as "result" since in route we have mentioned after trial/result


                //if (string.IsNullOrEmpty(actualparm.WaveProductID))
                //{
                //    return req.CreateResponse(HttpStatusCode.OK, "Please enter a valid Wave Product Id!");
                //}

                HttpClient client = new HttpClient();
                HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("https://catfact.ninja/fact"));


                HttpResponseMessage httpresponse = await client.SendAsync(myRequest);
                WaveProductModel isValidWaveProduct = await httpresponse.Content.ReadAsAsync<WaveProductModel>();



                return req.CreateResponse(HttpStatusCode.OK, isValidWaveProduct);
            }
            catch (Exception ex)
            {

                return req.CreateResponse(HttpStatusCode.OK, "Wave API product ID is not valid !!! Reason: {0}", string.Format(ex.Message));
            }
        }

        [FunctionName("ActualAPI")]
        public static async Task<HttpResponseMessage> ActualAPI(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Calling my External API");

            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                HttpRequestMessage myRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("yourUrl")); //replace it with actual url
                myRequest.Headers.Add("Authorization-Token", "yourToken"); //replace it with actual token


                HttpResponseMessage httpresponse = await client.SendAsync(myRequest);
                APIResponse[] isValidWaveProduct = await httpresponse.Content.ReadAsAsync<APIResponse[]>();



                return req.CreateResponse(HttpStatusCode.OK, isValidWaveProduct);
            }
            catch (Exception ex)
            {

                return req.CreateResponse(HttpStatusCode.OK, "Wave API product ID is not valid !!! Reason: {0}", string.Format(ex.Message));
            }
        }
    }

    public class APIResponse
    {
        public string CustomerID { get; set; }
        public int ActionID { get; set; }
        public int ChannelID { get; set; }
        public string TemplateID { get; set; }
        public string ScheduleTime { get; set; }
        public string PromoCode { get; set; }
        public string[] CustomerAttributes { get; set; }
    }

    public class WaveProductModel
    {
        public string fact { get; set; }
        public int length { get; set; }
    }


    public class WaveProductResponseModel
    {
        public string isValidWaveProductID { get; set; }
    }
}
