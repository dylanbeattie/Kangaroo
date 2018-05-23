using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Pivotal.Model {
    public class Story {
        public int Id { get; set; }
        public string Name { get; set; }


        [JsonProperty("current_state")]
        public string CurrentState { get; set; }

        public List<StoryTransition> Transitions { get; set; } = new List<StoryTransition>();

        public string[] History {
            get {
                if (this.Transitions == null || this.Transitions.Count == 0) return new string[] { };

                var states = new List<string>();
                for (var i = 0; i < this.Transitions.Count - 1; i++) {
                    var t1 = this.Transitions[i];
                    var t2 = this.Transitions[i + 1];
                    var days = (int) Math.Ceiling((t2.OccurredAt - t1.OccurredAt).TotalDays);
                    for (var j = 0; j < days; j++) states.Add(t1.State);
                }

                var tf = this.Transitions.LastOrDefault();
                var now = DateTime.Now;
                var d2 = (int) Math.Ceiling((now - tf.OccurredAt).TotalDays);
                for (var j = 0; j < d2; j++) states.Add(tf.State);
                return states.ToArray();
            }
        }
    }
}