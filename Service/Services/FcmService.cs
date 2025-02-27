using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Repository.ViewModels;
using FirebaseAdmin;
using Firebase.Database;
using Firebase.Database.Query;


namespace Service.Services
{
    public class FcmService : IFcmService
    {
        private readonly HttpClient _httpClient;
        private readonly string firebaseProjectId;
        //private readonly string serviceAccountPath;
        private readonly string firebaseurl;
        public FcmService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            firebaseProjectId = configuration["Firebase:ProjectId"];
            //serviceAccountPath = configuration["Firebase:ServiceAccountPath"];
            firebaseurl = configuration["Firebase:FBDataBase"];
        }
        
        public async Task<NotificationViewModel> SendPushNotificationAsync(string deviceToken, string title, string body)
        {
            NotificationViewModel result = new NotificationViewModel();
            try
            {
                string accessToken = await GetAccessTokenAsync();
                string firebaseUrl = $"https://fcm.googleapis.com/v1/projects/{firebaseProjectId}/messages:send";
                var message = new
                {
                    message = new
                    {
                        token = deviceToken,
                        notification = new
                        {
                            title = title,
                            body = body,
                        }
                    }
                };
                var jsonMessage = JsonConvert.SerializeObject(message);
                var request = new HttpRequestMessage(HttpMethod.Post, firebaseUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    result.Success = true;
                    result.Message = $"Notification sent successfully. Response: ${response}";
                }
                else
                {
                    result.Success = false;
                    result.Message = $"Failed to send notification. Response: ${response}";
                }
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = $"Error sending notification: {ex.Message}";
            }
            return result;
        }
        private async Task<string> GetAccessTokenAsync()
        {
            string serviceAccountJson = await GetServiceAccountJsonAsync();
            GoogleCredential credential = GoogleCredential.FromJson(serviceAccountJson).
                CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
        private async Task<string> GetServiceAccountJsonAsync()
        {
            string firebaseDatabaseUrl = firebaseurl.TrimEnd('/');
            var firebaseClient = new FirebaseClient(firebaseDatabaseUrl);

            var serviceAccount = await firebaseClient
                .Child("service_account")
                .OnceSingleAsync<object>(); 

            if (serviceAccount == null)
                throw new Exception("service_account not found in real time db");

            return Newtonsoft.Json.JsonConvert.SerializeObject(serviceAccount);
        }
    };
}
