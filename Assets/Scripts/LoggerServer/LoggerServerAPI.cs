using System;
using System.Collections.Generic;
using System.Net;
using LoggerServer.Models;
using RestSharp;
using Valve.Newtonsoft.Json;

namespace LoggerServer
{
    internal static class LoggerServerAPI
    {
        private const string Url = "http://team12-19.studenti.fiit.stuba.sk/api/logger/";

        private static readonly RestClient _server = new RestClient(Url);

        public static UserResponseModel PostUser(
            UserRequestModel userRequest,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
        {
            return Post<UserResponseModel>("user", userRequest, statusCodeHandlers, exceptionHandler);
        }

        public static List<MovementModel> GetUserMovements(
            string userId,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
        {
            return Get<List<MovementModel>>($"user/{userId}/movements", statusCodeHandlers, exceptionHandler);
        }

        public static UserMovementsResponseModel PostUserMovements(
            string userId,
            IList<MovementModel> movements,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
        {
            return Post<UserMovementsResponseModel>($"user/{userId}/movements", movements, statusCodeHandlers, exceptionHandler);
        }

        public static UserMovementsResponseModel DeleteUserMovements(
            string userId,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
        {
            return Delete<UserMovementsResponseModel>($"user/{userId}/movements", statusCodeHandlers, exceptionHandler);
        }

        public static LookupModel Lookup(
            LoggerModel model,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
        {
            return Post<LookupModel>("lookup", model, statusCodeHandlers, exceptionHandler);
        }

        private static TResponseData Get<TResponseData>(
            string requestUrl,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
            where TResponseData : class, new()
        {
            var request = new RestRequest(requestUrl, Method.GET, DataFormat.Json);
            return Execute<TResponseData>(request, statusCodeHandlers, exceptionHandler);
        }

        private static TResponseData Post<TResponseData>(
            string requestUrl,
            object requestBody,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
            where TResponseData : class, new()
        {
            var request = new RestRequest(requestUrl, Method.POST, DataFormat.Json);
            var json = JsonConvert.SerializeObject(requestBody);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            return Execute<TResponseData>(request, statusCodeHandlers, exceptionHandler);
        }

        private static TResponseData Delete<TResponseData>(
            string requestUrl,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
            where TResponseData : class, new()
        {
            var request = new RestRequest(requestUrl, Method.DELETE, DataFormat.Json);
            return Execute<TResponseData>(request, statusCodeHandlers, exceptionHandler);
        }

        private static TResponseData Execute<TResponseData>(
            IRestRequest request,
            IReadOnlyDictionary<HttpStatusCode, Func<bool>> statusCodeHandlers = null,
            Action<Exception> exceptionHandler = null)
            where TResponseData : class, new()
        {
            try
            {
                var response = _server.Execute<string>(request);

                if (statusCodeHandlers == null)
                {
                    return JsonConvert.DeserializeObject<TResponseData>(response.Data);
                }

                if (!statusCodeHandlers.ContainsKey(response.StatusCode))
                {
                    throw new Exception($"Unhandled response status code {response.StatusCode}");
                }

                var success = statusCodeHandlers[response.StatusCode]();
                return success == false ? null : JsonConvert.DeserializeObject<TResponseData>(response.Data);
            }
            catch (Exception e)
            {
                if (exceptionHandler == null)
                {
                    throw;
                }

                exceptionHandler(e);
                return null;
            }
        }
    }
}