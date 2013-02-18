﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.GoogleMusic.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OutcoldSolutions.GoogleMusic.Models;

    public interface IMusicPlaylistRepository
    {
        Task InitializeAsync();

        Task<IEnumerable<MusicPlaylist>> GetAllAsync();

        Task<MusicPlaylist> CreateAsync(string name);

        Task<bool> DeleteAsync(Guid playlistId);

        Task<bool> ChangeName(Guid playlistId, string name);

        Task<bool> RemoveEntry(Guid playlistId, Guid entryId);

        Task<bool> AddEntryAsync(Guid playlistId, Song song);
    }
}