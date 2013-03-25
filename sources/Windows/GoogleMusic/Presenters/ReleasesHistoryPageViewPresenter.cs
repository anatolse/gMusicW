﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.GoogleMusic.Presenters
{
    using System.Threading.Tasks;

    using OutcoldSolutions.GoogleMusic.Views;
    using OutcoldSolutions.Presenters;

    public class ReleasesHistoryPageViewPresenter : PagePresenterBase<IReleasesHistoryPageView>
    {
        private readonly INavigationService navigationService;

        public ReleasesHistoryPageViewPresenter(
            INavigationService navigationService)
        {
            this.navigationService = navigationService;

            this.LeavePageCommand = new DelegateCommand(this.LeavePage);
        }

        public DelegateCommand LeavePageCommand { get; private set; }

        protected override Task LoadDataAsync(NavigatedToEventArgs navigatedToEventArgs)
        {
            return Task.FromResult(0);
        }

        private void LeavePage()
        {
            this.navigationService.NavigateTo<IStartPageView>();
        }
    }
}