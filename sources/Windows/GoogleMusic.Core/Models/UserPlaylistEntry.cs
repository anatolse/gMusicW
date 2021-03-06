﻿// --------------------------------------------------------------------------------------------------------------------
// OutcoldSolutions (http://outcoldsolutions.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.GoogleMusic.Models
{
    using SQLite;

    [Table("UserPlaylistEntry")]
    public class UserPlaylistEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int PlaylistId { get; set; }

        public int SongId { get; set; }

        public int PlaylistOrder { get; set; }

        [Unique]
        public string ProviderEntryId { get; set; }

        [Reference]
        public Song Song { get; set; }
    }
}
