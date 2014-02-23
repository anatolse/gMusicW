﻿namespace OutcoldSolutions.BindingModels
{
    using System;

    using Windows.UI.Xaml;

    public class AppSizes : BindingModelBase
    {
        private bool isLarge;
        private bool isMedium;
        private bool isSmall;

        public AppSizes()
        {
            Window.Current.SizeChanged += (sender, args) => this.UpdateApplicationSizes(args.Size.Width);
            this.UpdateApplicationSizes(Window.Current.Bounds.Width);
        }

        public bool IsLarge
        {
            get
            {
                return this.isLarge;
            }
            set
            {
                this.isLarge = value;
                this.RaiseCurrentPropertyChanged();
            }
        }

        public bool IsMedium
        {
            get
            {
                return this.isMedium;
            }
            set
            {
                this.isMedium = value;
                this.RaiseCurrentPropertyChanged();
            }
        }

        public bool IsMediumOrLarge
        {
            get
            {
                return this.IsMedium || this.IsLarge;
            }
        }

        public bool IsMediumOrSmall
        {
            get
            {
                return this.IsMedium || this.IsSmall;
            }
        }

        public bool IsSmall
        {
            get
            {
                return this.isSmall;
            }
            set
            {
                this.isSmall = value;
                this.RaiseCurrentPropertyChanged();
            }
        }

        public event EventHandler SizeChanges;

        private void UpdateApplicationSizes(double width)
        {
            bool oldIsLarge = this.IsLarge, oldIsMedium = this.IsMedium, oldIsSmall = this.IsSmall;

            this.FreezeNotifications();
            this.IsLarge = 1024 <= width;
            this.IsMedium = 700 <= width && width < 1024;
            this.IsSmall = width < 700;
            this.RaisePropertyChanged(() => this.IsMediumOrLarge);
            this.RaisePropertyChanged(() => this.IsMediumOrSmall);
            this.UnfreezeNotifications();

            if (oldIsLarge != this.IsLarge || oldIsMedium != this.IsMedium || oldIsSmall != this.IsSmall)
            {
                this.OnOnSizeChanges();
            }
        }

        private void OnOnSizeChanges()
        {
            EventHandler handler = this.SizeChanges;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}