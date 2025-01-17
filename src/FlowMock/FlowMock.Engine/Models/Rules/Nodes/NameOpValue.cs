﻿using System.Text.Json.Serialization;

namespace FlowMock.Engine.Models.Rules.Nodes
{
    public class NameOpValue
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("op")]
        public string Op { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
