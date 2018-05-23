using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using System.Web;
using Newtonsoft.Json;
using Pivotal.Model;

namespace Pivotal {
    public class Tracker {
        private static string trackerApiToken;

        public Tracker() {
            trackerApiToken = Environment.GetEnvironmentVariable("PIVOTAL_TRACKER_API_TOKEN");
            if (String.IsNullOrEmpty(trackerApiToken)) {
                throw new Exception("You must set the environment variable PIVOTAL_TRACKER_API_TOKEN");
            }
        }

        private const string BASE_URL = "https://www.pivotaltracker.com/services/v5";

        public dynamic Me() {
            return Get("/me");
        }

        private dynamic Get(string path) {
            using (var wc = new WebClient()) {
                wc.Headers.Add("X-TrackerToken", trackerApiToken);
                return wc.DownloadString(BASE_URL + path);
            }
        }

        public IList<Project> Projects() {
            var json = Get("/projects");
            return JsonConvert.DeserializeObject<List<Project>>(json);
        }

        public IList<Story> Stories(int projectId) {
            var threshold = DateTime.Now.AddDays(-60).Date.ToString("O");
            var total = 0;
            var offset = 0;
            var stories = new List<Story>();

            using (var wc = new WebClient()) {
                wc.Headers.Add("X-TrackerToken", trackerApiToken);
                var returned = 0;
                var limit = 0;
                do {
                    var url =
                        $"{BASE_URL}/projects/{projectId}/stories?updated_after={HttpUtility.UrlEncode(threshold)}&offset={offset}";
                    Console.WriteLine(url);
                    var json = wc.DownloadString(url);
                    foreach (var key in wc.ResponseHeaders.AllKeys) {
                        Console.WriteLine(key + ": " + wc.ResponseHeaders[key]);
                    }

                    stories.AddRange(JsonConvert.DeserializeObject<List<Story>>(json));
                    int.TryParse(wc.ResponseHeaders["X-Tracker-Pagination-Limit"], out limit);
                    int.TryParse(wc.ResponseHeaders["X-Tracker-Pagination-Offset"], out offset);
                    int.TryParse(wc.ResponseHeaders["X-Tracker-Pagination-Returned"], out returned);
                    int.TryParse(wc.ResponseHeaders["X-Tracker-Pagination-Total"], out total);
                    offset += limit;
                } while (returned == limit);

                returned = 0;
                limit = 0;
                offset = 0;
                var transitions = new List<StoryTransition>();
                do {
                    var url =
                        $"{BASE_URL}/projects/{projectId}/story_transitions?occurred_after={HttpUtility.UrlEncode(threshold)}&offset={offset}";
                    Console.WriteLine(url);
                    var json = wc.DownloadString(url);
                    foreach (var key in wc.ResponseHeaders.AllKeys) {
                        Console.WriteLine(key + ": " + wc.ResponseHeaders[key]);
                    }

                    transitions.AddRange(JsonConvert.DeserializeObject<List<StoryTransition>>(json));
                    int.TryParse(wc.ResponseHeaders["X-Tracker-Pagination-Limit"], out limit);
                    int.TryParse(wc.ResponseHeaders["X-Tracker-Pagination-Offset"], out offset);
                    int.TryParse(wc.ResponseHeaders["X-Tracker-Pagination-Returned"], out returned);
                    int.TryParse(wc.ResponseHeaders["X-Tracker-Pagination-Total"], out total);
                    offset += limit;
                } while (returned == limit);

                foreach (var group in transitions.GroupBy(t => t.StoryId)) {
                    var story = stories.FirstOrDefault(s => s.Id == group.Key);
                    if (story == default(Story)) continue;
                    story.Transitions = group.OrderBy(t => t.OccurredAt).ToList();
                }
            }

            return stories;
        }
    }
}