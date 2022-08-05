using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WWS.PowerAutomate.StringHelper
{
    public class Script : ScriptBase
    {
        public override async Task<HttpResponseMessage> ExecuteAsync()
        {
            // Check if the operation ID matches what is specified in the OpenAPI definition of the connector
            switch (Context.OperationId)
            {
                case "RegexMatch":
                    return await HandleRegexMatchOperation().ConfigureAwait(false);
                case "RegexIsMatch":
                    return await HandleRegexIsMatchOperation().ConfigureAwait(false);
                case "RegexReplace":
                    return await HandleRegexReplaceOperation().ConfigureAwait(false);
                case "CreateMD5Hash":
                    return await HandleCreateMd5HashOperation().ConfigureAwait(false);
            }

            // Handle an invalid operation ID
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            response.Content = CreateJsonContent($"Unknown operation ID '{Context.OperationId}'");
            return response;
        }


        private async Task<HttpResponseMessage> HandleCreateMd5HashOperation()
        {
            HttpResponseMessage response;
            var contentAsString = await Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Parse as JSON object
            var contentAsJson = JObject.Parse(contentAsString);

            // Get the value of text to check
            if(contentAsJson["Value"] != null){
                var textInput = (string)contentAsJson["Value"];
                string textOutput;

                // Use input string to calculate MD5 hash
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(textInput);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    textOutput = Convert.ToBase64String(hashBytes);
                }
            
                JObject output = new JObject
                {
                    ["Value"] = textInput,
                    ["MD5Hash"] = textOutput,
                };

                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = CreateJsonContent(output.ToString());
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return response;
        }

        private async Task<HttpResponseMessage> HandleRegexIsMatchOperation()
        {
            HttpResponseMessage response;
            var contentAsString = await Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Parse as JSON object
            var contentAsJson = JObject.Parse(contentAsString);

            // Get the value of text to check
            var textInput = (string)contentAsJson["Value"];

            // Create a regex based on the request content
            var regexString = (string)contentAsJson["regex"];
            if (regexString != null && textInput != null)
            {
                var rx = new Regex(regexString);

                JObject output = new JObject
                {
                    ["Value"] = textInput,
                    ["isMatch"] = rx.IsMatch(textInput),
                };

                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = CreateJsonContent(output.ToString());
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return response;
        }

        private async Task<HttpResponseMessage> HandleRegexMatchOperation()
        {
            HttpResponseMessage response;
            var contentAsString = await Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Parse as JSON object
            var contentAsJson = JObject.Parse(contentAsString);

            // Get the value of text to check
            var textInput = (string)contentAsJson["Value"];

            // Create a regex based on the request content
            var regexString = (string)contentAsJson["regex"];
            if (regexString != null && textInput != null)
            {
                var regEx = new Regex(regexString);
                
                    Match regexMatchOutput = regEx.Match(textInput);
                    
                    JObject output = new JObject
                    {
                        ["Value"] = textInput,
                        ["match"] = regexMatchOutput.Value
            };
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = CreateJsonContent(output.ToString());
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return response;
        }
        private async Task<HttpResponseMessage> HandleRegexReplaceOperation()
        {
            HttpResponseMessage response;
            
            var contentAsString = await Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Parse as JSON object
            var contentAsJson = JObject.Parse(contentAsString);

            // Get the value of text to check
            var textInput = (string)contentAsJson["Value"];

            // Create a regex based on the request content
            var regexString = (string)contentAsJson["regex"];
            var regexValue = (string)contentAsJson["replaceValue"];
            if (regexValue == null)
            {
                regexValue = "";
            }
            if (regexString != null && textInput != null)
            {
                var regEx = new Regex(regexString);
                JObject output = new JObject
                {
                    ["Value"] = textInput,
                    ["replacedString"] = regEx.Replace(textInput, regexValue)
            };

                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = CreateJsonContent(output.ToString());
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return response;
        }
    }

}
