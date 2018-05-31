using System;
using RestSharp;

namespace Taurit.Toolkit.TodoistInboxHelper
{
    public abstract class TodoistApiService
    {
        protected const String ApiUrl = "https://todoist.com/api/v7/";
        protected readonly String AuthToken;
        protected RestClient RestClient;

        protected TodoistApiService(String authToken)
        {
            AuthToken = authToken;
            RestClient = new RestClient(ApiUrl);
        }
    }
}