﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.GoogleMusic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using OutcoldSolutions.Diagnostics;
    using OutcoldSolutions.GoogleMusic.Models;
    using OutcoldSolutions.GoogleMusic.Web;
    using OutcoldSolutions.GoogleMusic.Web.Models;

    using Windows.ApplicationModel.Resources;

    public class AuthentificationService : IAuthentificationService
    {
        private readonly ILogger logger;
        private readonly IGoogleAccountService googleAccountService;
        private readonly IGoogleMusicSessionService sessionService;
        private readonly IGoogleAccountWebService googleAccountWebService;
        private readonly IGoogleMusicWebService googleMusicWebService;

        private readonly ResourceLoader resourceLoader = new ResourceLoader("CoreResources");

        public AuthentificationService(
            ILogManager logManager,
            IGoogleAccountService googleAccountService,
            IGoogleMusicSessionService sessionService,
            IGoogleAccountWebService googleAccountWebService,
            IGoogleMusicWebService googleMusicWebService)
        {
            this.logger = logManager.CreateLogger("AuthentificationService");
            this.googleAccountService = googleAccountService;
            this.sessionService = sessionService;
            this.googleAccountWebService = googleAccountWebService;
            this.googleMusicWebService = googleMusicWebService;
        }

        public async Task<AuthentificationResult> CheckAuthentificationAsync(UserInfo userInfo = null)
        {
            if (userInfo == null)
            {
                this.logger.Debug("CheckAuthentificationAsync: trying to restore previos session.");
                var userSession = this.sessionService.GetSession();
                if (userSession != null)
                {
                    this.logger.Debug("CheckAuthentificationAsync: GetSession is not null.");

                    var cookieCollection = await this.sessionService.GetSavedCookiesAsync();
                    if (cookieCollection != null)
                    {
                        this.logger.Debug("CheckAuthentificationAsync: cookie collection is not null. Initializing web services.");
                        this.googleMusicWebService.Initialize(cookieCollection.Cast<Cookie>());
                        userSession.IsAuthenticated = true; 
                        return AuthentificationResult.SucceedResult();
                    }
                }
            }

            if (userInfo == null)
            {
                this.logger.Debug("CheckAuthentificationAsync: Trying to get user info with pasword.");
                userInfo = this.googleAccountService.GetUserInfo(retrievePassword: true);
            }

            if (userInfo == null)
            {
                this.logger.Debug("CheckAuthentificationAsync: Cannot get user info.");
                return AuthentificationResult.FailedResult(null);
            }

            GoogleAuthResponse authResponse = await this.googleAccountWebService.AuthenticateAsync(
                new Uri(this.googleMusicWebService.GetServiceUrl()), userInfo.Email, userInfo.Password);

            if (authResponse.Success)
            {
                if (authResponse.CookieCollection != null && authResponse.CookieCollection.Count > 0)
                {
                    this.googleMusicWebService.Initialize(authResponse.CookieCollection.Cast<Cookie>());
                    this.sessionService.GetSession().IsAuthenticated = true;
                    return AuthentificationResult.SucceedResult();
                }
            }
            else if (authResponse.Error.HasValue)
            {
                string errorMessage = this.GetErrorMessage(authResponse.Error.Value);
                this.logger.Warning("CheckAuthentificationAsync: ErrorMessage: {0}, error code: {1}", errorMessage, authResponse.Error.Value);
                return AuthentificationResult.FailedResult(errorMessage);
            }

            this.logger.Error("CheckAuthentificationAsync: showing 'Login_Unknown'.");
            return AuthentificationResult.FailedResult(this.resourceLoader.GetString("Login_Unknown"));
        }

        private string GetErrorMessage(GoogleAuthResponse.ErrorResponseCode errorResponseCode)
        {
            switch (errorResponseCode)
            {
                case GoogleAuthResponse.ErrorResponseCode.BadAuthentication:
                    return this.resourceLoader.GetString("Login_BadAuthentication");
                case GoogleAuthResponse.ErrorResponseCode.NotVerified:
                    return this.resourceLoader.GetString("Login_NotVerified");
                case GoogleAuthResponse.ErrorResponseCode.TermsNotAgreed:
                    return this.resourceLoader.GetString("Login_TermsNotAgreed");
                case GoogleAuthResponse.ErrorResponseCode.CaptchaRequired:
                    return this.resourceLoader.GetString("Login_CaptchaRequired");
                case GoogleAuthResponse.ErrorResponseCode.Unknown:
                    return this.resourceLoader.GetString("Login_Unknown");
                case GoogleAuthResponse.ErrorResponseCode.AccountDeleted:
                    return this.resourceLoader.GetString("Login_AccountDeleted");
                case GoogleAuthResponse.ErrorResponseCode.AccountDisabled:
                    return this.resourceLoader.GetString("Login_AccountDisabled");
                case GoogleAuthResponse.ErrorResponseCode.ServiceDisabled:
                    return this.resourceLoader.GetString("Login_ServiceDisabled");
                case GoogleAuthResponse.ErrorResponseCode.ServiceUnavailable:
                    return this.resourceLoader.GetString("Login_ServiceUnavailable");
                default:
                    throw new NotSupportedException("Value is not supported: " + errorResponseCode.ToString());
            }
        }

        public class Captcha
        {
            public Captcha(string captchaToken, string captchaUrl)
            {
                this.CaptchaToken = captchaToken;
                this.CaptchaUrl = captchaUrl;
            }

            public string CaptchaToken { get; private set; }

            public string CaptchaUrl { get; private set; }
        }

        public class AuthentificationResult
        {
            private AuthentificationResult(bool succeed, string errorMessage = null)
            {
                this.Succeed = succeed;
                this.ErrorMessage = errorMessage;
            }

            public bool Succeed { get; private set; }

            public string ErrorMessage { get; set; }

            public static AuthentificationResult FailedResult(string errorMessage)
            {
                return new AuthentificationResult(succeed: false, errorMessage: errorMessage);
            }

            public static AuthentificationResult SucceedResult()
            {
                return new AuthentificationResult(succeed: true);
            }
        }
    }
}
