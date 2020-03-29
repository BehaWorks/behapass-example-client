using System;
using System.Collections.Generic;
using System.Net;
using LoggerServer.Models;
using RestSharp;
using Valve.Newtonsoft.Json;

namespace LoggerServer
{
    public static class LoggerServerAPI
    {
        private const string Url = "http://team12-19.studenti.fiit.stuba.sk/api/logger/";

        private static readonly RestClient _server = new RestClient(Url);

        public static UserResponseModel PostUser(
            UserRequestModel userRequest,
            Func<HttpStatusCode, bool> statusHandler = null,
            Action<Exception> exceptionHandler = null)
        {
            return Post<UserResponseModel>("user", userRequest, statusHandler, exceptionHandler);
        }

        public static UserMovementsResponseModel PostUserMovements(
            string userId,
            IList<MovementModel> movements,
            Func<HttpStatusCode, bool> statusHandler = null,
            Action<Exception> exceptionHandler = null)
        {
            return Post<UserMovementsResponseModel>($"{userId}/movements", movements);
        }

        private static TResponseData Get<TResponseData>(
            string requestUrl,
            Func<HttpStatusCode, bool> statusHandler = null,
            Action<Exception> exceptionHandler = null)
            where TResponseData : class, new()
        {
            var request = new RestRequest(requestUrl, Method.GET, DataFormat.Json);
            return Execute<TResponseData>(request, statusHandler, exceptionHandler);
        }

        private static TResponseData Post<TResponseData>(
            string requestUrl,
            object requestBody,
            Func<HttpStatusCode, bool> statusHandler = null,
            Action<Exception> exceptionHandler = null)
            where TResponseData : class, new()
        {
            var request = new RestRequest(requestUrl, Method.POST, DataFormat.Json);
            var json = JsonConvert.SerializeObject(requestBody);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            return Execute<TResponseData>(request, statusHandler, exceptionHandler);
        }

        private static TResponseData Execute<TResponseData>(
            RestRequest request,
            Func<HttpStatusCode, bool> statusHandler = null,
            Action<Exception> exceptionHandler = null)
            where TResponseData : class, new()
        {
            try
            {
                var response = _server.Execute<string>(request);

                var success = statusHandler?.Invoke(response.StatusCode);

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