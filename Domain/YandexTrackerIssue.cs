using Newtonsoft.Json;

namespace YandexTrackerToNotion.Domain
{

    public class YandexTrackerIssue
    {
        [JsonProperty("packet.type")]
        public string PacketType { get; set; }

        public string Key { get; set; }

        public string CurrentUser { get; set; }

        [JsonProperty("currentDateTime.date")]
        public string CurrentDateTime { get; set; }

        [JsonProperty("issue.id")]
        public string Id { get; set; }

        [JsonProperty("issue.releaseNote")]
        public string ReleaseNote { get; set; }

        [JsonProperty("issue.summary")]
        public string Summary { get; set; }

        [JsonProperty("issue.type")]
        public string Type { get; set; }

        [JsonProperty("issue.priority")]
        public string Priority { get; set; }

        [JsonProperty("issue.description")]
        public string Description { get; set; }

        [JsonProperty("issue.status")]
        public string Status { get; set; }

        [JsonProperty("issue.resolution")]
        public string Resolution { get; set; }

        [JsonProperty("issue.statusStartTime")]
        public string StatusStartTime { get; set; }

        [JsonProperty("issue.created")]
        public string Created { get; set; }

        [JsonProperty("issue.updated")]
        public string Updated { get; set; }

        [JsonProperty("issue.resolved")]
        public string Resolved { get; set; }

        [JsonProperty("issue.lastCommentUpdatedAt")]
        public string LastCommentUpdatedAt { get; set; }

        [JsonProperty("issue.start")]
        public string Start { get; set; }

        [JsonProperty("issue.end")]
        public string End { get; set; }

        [JsonProperty("issue.dueDate")]
        public string DueDate { get; set; }

        [JsonProperty("issue.author")]
        public string Author { get; set; }

        [JsonProperty("issue.assignee")]
        public string Assignee { get; set; }

        [JsonProperty("issue.followers")]
        public string Followers { get; set; }

        [JsonProperty("issue.access")]
        public string Access { get; set; }

        [JsonProperty("issue.modifier")]
        public string Modifier { get; set; }

        [JsonProperty("issue.resolver")]
        public string Resolver { get; set; }

        [JsonProperty("issue.followingMaillists")]
        public string FollowingMaillists { get; set; }

        [JsonProperty("issue.project")]
        public string Project { get; set; }

        [JsonProperty("issue.tags")]
        public string Tags { get; set; }

        [JsonProperty("issue.components")]
        public string Components { get; set; }

        [JsonProperty("issue.affectedVersions")]
        public string AffectedVersions { get; set; }

        [JsonProperty("issue.fixVersions")]
        public string FixVersions { get; set; }

        [JsonProperty("issue.parent")]
        public string Parent { get; set; }

        [JsonProperty("issue.previousQueue")]
        public string PreviousQueue { get; set; }

        [JsonProperty("issue.votedBy")]
        public string VotedBy { get; set; }

        [JsonProperty("issue.votes")]
        public int Votes { get; set; }

        [JsonProperty("issue.commentWithoutExternalMessageCount")]
        public int CommentWithoutExternalMessageCount { get; set; }

        [JsonProperty("issue.commentWithExternalMessageCount")]
        public int CommentWithExternalMessageCount { get; set; }

        [JsonProperty("issue.boards")]
        public string Boards { get; set; }

        [JsonProperty("issue.pendingReplyFrom")]
        public string PendingReplyFrom { get; set; }

        [JsonProperty("issue.lastQueue")]
        public string LastQueue { get; set; }

        [JsonProperty("issue.participantPercentsTotal")]
        public string ParticipantPercentsTotal { get; set; }

        [JsonProperty("issue.possibleSpam")]
        public string PossibleSpam { get; set; }

        [JsonProperty("issue.statusType")]
        public string StatusType { get; set; }

        [JsonProperty("issue.qaEngineer")]
        public string QaEngineer { get; set; }

        [JsonProperty("issue.originalEstimation")]
        public string OriginalEstimation { get; set; }

        [JsonProperty("issue.estimation")]
        public string Estimation { get; set; }

        [JsonProperty("issue.spent")]
        public string Spent { get; set; }

        [JsonProperty("issue.epic")]
        public string Epic { get; set; }

        [JsonProperty("issue.storyPoints")]
        public string StoryPoints { get; set; }

        [JsonProperty("issue.sprint")]
        public string Sprint { get; set; }
    }
}