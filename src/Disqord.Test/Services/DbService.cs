using LiteDB;

namespace NewsParser.Services
{
    public sealed class ReportEntity
    {
        /// <summary>
        ///     Auto-managed LiteDB id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Community on which the report has been sent.
        /// </summary>
        public string Community { get; set; }

        /// <summary>
        ///     Server on which the report has been sent.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        ///     Channel on which the report has been sent.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        ///     Player's nickname that is being reported.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        ///     Details given by the author of the request.
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        ///     Author Nickname#Discriminator that created the request.
        /// </summary>
        public string ReportAuthor { get; set; }

        /// <summary>
        ///     Indicates whether the author of the request already gave more details.
        /// </summary>
        public bool DetailsGiven { get; set; }
    }

    public sealed class RSSEntity
    {
        /// <summary>
        ///     Auto-managed LiteDB id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Channel on which the RSS must be sent.
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        ///     URL used to retrieve RSS file.
        /// </summary>
        public string RSSUrl { get; set; }
    }

    public sealed class TopicEntity
    {
        /// <summary>
        ///     Auto-managed LiteDB id. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     ID of the RSSEntity associated with that topic.
        /// </summary>
        public int RSSEntityId { get; set; }

        /// <summary>
        ///     Title or name of the post.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Author of the post.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        ///     Complete URL of the post.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Complete date when the post has been created.
        /// </summary>
        public string CreatedAt { get; set; }
    }
}
