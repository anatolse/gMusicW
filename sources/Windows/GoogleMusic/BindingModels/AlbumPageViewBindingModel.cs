﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.GoogleMusic.BindingModels
{
    using OutcoldSolutions.GoogleMusic.Models;

    public class AlbumPageViewBindingModel : BindingModelBase
    {
        private Album album;

        public Album Album
        {
            get
            {
                return this.album;
            }

            set
            {
                this.album = value;
                this.RaiseCurrentPropertyChanged();
            }
        }
    }
}