using System;
using RestSharp;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public abstract class TodoistApiService
    {
        public const String ApiUrl = "https://todoist.com/api/v8/";
        protected readonly String AuthToken;
        protected RestClient RestClient;

        protected TodoistApiService(String authToken)
        {
            AuthToken = authToken;
            RestClient = new RestClient(ApiUrl);
        }

    }
}