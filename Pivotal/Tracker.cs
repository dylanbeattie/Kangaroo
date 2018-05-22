using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;

namespace Pivotal {
    public class Tracker {
        private const string TRACKER_API_TOKEN = "b1be30c19645d929605c3ffdb23dfad8";
        private const string BASE_URL = "https://www.pivotaltracker.com/services/v5";

        public dynamic Me() {
            return Get("/me");
        }

        private dynamic Get(string path) {
            using (var wc = new WebClient()) {
                wc.Headers.Add("X-TrackerToken", TRACKER_API_TOKEN);
                return wc.DownloadString(BASE_URL + path);
            }
        }

        public IList<Project> Projects() {
            var json = Get("/projects");
            return JsonConvert.DeserializeObject<List<Project>>(json);
        }

        public IList<Story> Stories(int projectId) {
            using (var wc = new WebClient()) {
                wc.Headers.Add("X-TrackerToken", TRACKER_API_TOKEN);
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