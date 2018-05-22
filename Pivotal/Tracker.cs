using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;

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
            using (var wc = new WebClient()) {
                wc.Headers.Add("X-TrackerToken", trackerApiToken);
                var json = wc.DownloadString($"{BASE_URL}/projects/{projectId}/stories/");
                // TODO: pagination stuff here?
                return JsonConvert.DeserializeObject<List<Story>>(json);
            }
        }
    }

    public class Project {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Story {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}