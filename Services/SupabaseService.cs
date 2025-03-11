﻿using Supabase;
using Supabase.Storage;
using Blazored.LocalStorage;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase.Gotrue;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlogApplication.Services
{
    public class SupabaseService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthStateProvider _authStateProvider;

        public SupabaseService(ILocalStorageService localStorage, CustomAuthStateProvider authStateProvider)
        {
            var url = "https://vmndunlatzedvqczeyap.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZtbmR1bmxhdHplZHZxY3pleWFwIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mzg4MTcyMTYsImV4cCI6MjA1NDM5MzIxNn0.5qtfu6TCpLLyUnIatb0W6hDzCukYZDjpw2vs38k4SPk";
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            _supabaseClient = new Supabase.Client(url, key, options);
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<Dictionary<string, object>?> GetUserMetadataAsync()
        {
            var metadataJson = await _localStorage.GetItemAsync<string>("user_metadata");
            return metadataJson != null ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(metadataJson) : null;
        }

        public async Task<string?> GetUsernameAsync()
        {
            var userMetadata = await GetUserMetadataAsync();
            return userMetadata != null && userMetadata.ContainsKey("username") ? userMetadata["username"].ToString() : "Unknown User";
        }

        public async Task<bool> SignUpAsync(string email, string password, string username)
        {
            try
            {
                var options = new SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "username", username },
                        { "role", "User"}
                    }
                };

                var response = await _supabaseClient.Auth.SignUp(email, password, options);
                if (response?.User != null)
                {
                    await _localStorage.SetItemAsync("user_metadata", System.Text.Json.JsonSerializer.Serialize(response.User.UserMetadata));
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> SignInAsync(string email, string password)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(email, password);
                if (session != null)
                {
                    await _localStorage.SetItemAsync("auth_token", session.AccessToken);
                    await _localStorage.SetItemAsync("user_metadata", System.Text.Json.JsonSerializer.Serialize(session.User.UserMetadata));
                    await _localStorage.SetItemAsync("userid", session.User.Id);
                    await _localStorage.SetItemAsync("username", session.User.UserMetadata["username"].ToString());

                    _authStateProvider.NotifyUserAuthentication(session.AccessToken);

                    return session.AccessToken;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task SignOutAsync()
        {
            await _supabaseClient.Auth.SignOut();
            await _localStorage.RemoveItemAsync("auth_token");
            await _localStorage.RemoveItemAsync("user_metadata");
            await _localStorage.RemoveItemAsync("userid");

            _authStateProvider.NotifyUserLogout();
        }
    }
}