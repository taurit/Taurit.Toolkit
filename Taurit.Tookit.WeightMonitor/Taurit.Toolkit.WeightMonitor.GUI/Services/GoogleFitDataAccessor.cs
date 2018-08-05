using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Fitness.v1;
using Google.Apis.Fitness.v1.Data;
using Google.Apis.Services;
using NodaTime;
using Taurit.Toolkit.WeightMonitor.Common.Models;

namespace Taurit.Toolkit.WeightMonitor.GUI.Services
{
    internal class GoogleFitDataAccessor
    {
        public async Task<WeightInTime[]> GetWeightDataPoints(Int32 numDays)
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
            Int64 from = now.Minus(Duration.FromDays(numDays)).ToUnixTimeMilliseconds() * 1_000_000;
            Int64 to = now.ToUnixTimeMilliseconds() * 1_000_000;

            Dataset weightPoints = service
                .Users
                .DataSources
                .Datasets
                .Get("me", "derived:com.google.weight:com.google.android.gms:merge_weight", $"{from}-{to}")
                .Execute();

            return weightPoints.Point.SelectMany(x =>
                x.Value.Select(y =>
                {
                    Debug.Assert(x.EndTimeNanos != null, "x.EndTimeNanos != null");
                    Debug.Assert(y.FpVal != null, "y.FpVal != null");

                    return new WeightInTime(x.EndTimeNanos.Value, y.FpVal.Value);
                })).ToArray();
        }
    }
}