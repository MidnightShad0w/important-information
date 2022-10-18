﻿using ImpInfCommon.ApiServices;
using ImpInfCommon.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ImpInfFrontCommon.Services
{
    public class TokenAuthStateProvider : AuthenticationStateProvider
    {
        private readonly CookieService cookieService;
        private readonly AuthService authService;
        private readonly ITokenProvider tokenProvider;

        public TokenAuthStateProvider(CookieService cookieService, AuthService authService, ITokenProvider tokenProvider)
        {
            this.cookieService = cookieService;
            this.authService = authService;
            this.tokenProvider = tokenProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await cookieService.GetCookies("token");
            if (string.IsNullOrWhiteSpace(token) || !await authService.CheckToken(token)) return GetStateAnonymous();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Authentication, token),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await tokenProvider.SetToken();

            return new AuthenticationState(claimsPrincipal);
        }

        public async void SetLogoutState()
        {
            await cookieService.SetCookies("token", "");

            NotifyAuthenticationStateChanged(Task.FromResult(GetStateAnonymous()));
        }

        private static AuthenticationState GetStateAnonymous()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            var state = new AuthenticationState(anonymous);
            return state;
        }
    }
}
