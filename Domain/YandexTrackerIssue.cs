using System;
using Newtonsoft.Json;

namespace YandexTrackerToNotion.Domain
{

    public class YandexTrackerIssue
    {
        public string Key { get; set; }
        public string CurrentUser { get; set; }

        [JsonProperty("currentDateTime.date")]
        public string CurrentDateTime { get; set; }

        [JsonProperty("issue.summary")]
        public string Summary { get; set; }

        [JsonProperty("issue.description")]
        public string Description { get; set; }

        public YandexTrackerIssueDetail Issue { get; set; }
    }

    public class YandexTrackerIssueDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("releaseNote")]
        public string ReleaseNote { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("resolution")]
        public string Resolution { get; set; }

        [JsonProperty("statusStartTime")]
        public string StatusStartTime { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("updated")]
        public string Updated { get; set; }

        [JsonProperty("resolved")]
        public string Resolved { get; set; }

        [JsonProperty("lastCommentUpdatedAt")]
        public string LastCommentUpdatedAt { get; set; }

        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }

        [JsonProperty("dueDate")]
        public string DueDate { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("assignee")]
        public string Assignee { get; set; }

        [JsonProperty("followers")]
        public string Followers { get; set; }

        [JsonProperty("access")]
        public string Access { get; set; }

        [JsonProperty("modifier")]
        public string Modifier { get; set; }

        [JsonProperty("resolver")]
        public string Resolver { get; set; }

        [JsonProperty("followingMaillists")]
        public string FollowingMaillists { get; set; }

        [JsonProperty("project")]
        public string Project { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("components")]
        public string Components { get; set; }

        [JsonProperty("affectedVersions")]
        public string AffectedVersions { get; set; }

        [JsonProperty("fixVersions")]
        public string FixVersions { get; set; }

        [JsonProperty("parent")]
        public string Parent { get; set; }

        [JsonProperty("previousQueue")]
        public string PreviousQueue { get; set; }

        [JsonProperty("votedBy")]
        public string VotedBy { get; set; }

        [JsonProperty("votes")]
        public int Votes { get; set; }

        [JsonProperty("commentWithoutExternalMessageCount")]
        public int CommentWithoutExternalMessageCount { get; set; }

        [JsonProperty("commentWithExternalMessageCount")]
        public int CommentWithExternalMessageCount { get; set; }

        [JsonProperty("boards")]
        public string Boards { get; set; }

        [JsonProperty("pendingReplyFrom")]
        public string PendingReplyFrom { get; set; }

        [JsonProperty("lastQueue")]
        public string LastQueue { get; set; }

        [JsonProperty("participantPercentsTotal")]
        public string ParticipantPercentsTotal { get; set; }

        [JsonProperty("possibleSpam")]
        public string PossibleSpam { get; set; }

        [JsonProperty("statusType")]
        public string StatusType { get; set; }

        [JsonProperty("qaEngineer")]
        public string QaEngineer { get; set; }

        [JsonProperty("originalEstimation")]
        public string OriginalEstimation { get; set; }

        [JsonProperty("estimation")]
        public string Estimation { get; set; }

        [JsonProperty("spent")]
        public string Spent { get; set; }

        [JsonProperty("epic")]
        public string Epic { get; set; }

        [JsonProperty("storyPoints")]
        public string StoryPoints { get; set; }

        [JsonProperty("sprint")]
        public string Sprint { get; set; }
    }
}