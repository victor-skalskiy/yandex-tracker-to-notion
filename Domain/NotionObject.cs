﻿using System;
using Newtonsoft.Json;

namespace YandexTrackerToNotion.Domain
{
    public class NotionObject
    {
        public NotionObject() { Description = ""; }
        public string Object { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string YTID { get; set; }
        public TimeSpan OriginalEstimation { get; set; }
        public TimeSpan Estimation { get; set; }
        public TimeSpan Spent { get; set; }
        public string? Emoji { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Deserialization wrapper class for search response
    /// </summary>
    public class NotionSearchResponse
    {
        public List<NotionObject>? Results { get; set; }
    }
}