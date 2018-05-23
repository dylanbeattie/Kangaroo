using System;
using Newtonsoft.Json;

namespace Pivotal.Model {
    public class StoryTransition {
        public string State { get; set; }

        [JsonProperty("story_id")]
        public int StoryId { get; set; }

        [JsonProperty("project_id")]
        public int ProjectId { get; set; }

        [JsonProperty("occurred_at")]
        public DateTime OccurredAt { get; set; }

        [JsonProperty("performed_by_id")]
        public int PerformedById { get; set; }
    }
}