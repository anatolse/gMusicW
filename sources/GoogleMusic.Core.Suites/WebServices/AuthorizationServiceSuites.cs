﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.GoogleMusic.Suites.WebServices
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Moq;

    
    using OutcoldSolutions.GoogleMusic.WebServices;
    using OutcoldSolutions.GoogleMusic.WebServices.Models;

    using Xunit;

    public class AuthorizationServiceSuites : SuitesBase
    {
        

        [Fact(Skip = "Web Service")]
        public async Task Authorize_SetCredentials_CookiesLoaded()
        {
            //var authorizationDataService = new Mock<IUserAuthorizationDataService>();
            //    authorizationDataService.Setup(s => s.GetUserSecurityDataAsync()).Returns(
            //        () => Task.Factory.StartNew(() => new UserInfo() { Email = "outcoldman.test@gmail.com", Password = "Qw12er34" }));

            //var cookieManager = new Mock<ICookieManager>();
            //cookieManager.Setup(m => m.GetCookies()).Returns(() => null);

            //using (var registration = this.Container.Registration())
            //{
            //    registration.Register<IUserAuthorizationDataService>()
            //        .AsSingleton(authorizationDataService.Object);

            //    registration.Register<IClientLoginService>().As<ClientLoginService>();
            //    registration.Register<IGoogleWebService>().As<GoogleWebService>();

            //    registration.Register<ICookieManager>().AsSingleton(cookieManager.Object);
            //}

            //var service = this.Container.Resolve<AuthorizationService>();
            //await service.AuthorizeAsync();

            //cookieManager.Verify(x => x.SaveCookies(It.IsAny<Uri>(), It.IsAny<CookieCollection>()), Times.Once());
        }
    }
}