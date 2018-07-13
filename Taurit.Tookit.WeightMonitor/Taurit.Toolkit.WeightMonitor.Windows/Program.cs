using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Fitness.v1;
using Google.Apis.Fitness.v1.Data;
using Google.Apis.Services;
using NodaTime;

namespace Taurit.Toolkit.WeightMonitor.Windows
{
    internal class Program
    {
        private static async Task Main(String[] args)
        {
            UserCredential credential;
            using (var stream = new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] {FitnessService.Scope.FitnessBodyWrite},
                    "user", CancellationToken.None);
            }

            var service = new FitnessService(new BaseClientService.Initializer
            {
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name,
                HttpClientInitializer = credential
            });

            Instant now = SystemClock.Instance.GetCurrentInstant();
            Int64 from = now.Minus(Duration.FromDays(30)).ToUnixTimeMilliseconds() * 1_000_000;
            Int64 to = now.ToUnixTimeMilliseconds() * 1_000_000;

            Dataset weightPoints = service
                .Users
                .DataSources
                .Datasets
                .Get("me", "derived:com.google.weight:com.google.android.gms:merge_weight", $"{from}-{to}")
                .Execute();
        }
    }
}