using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LambdaNotificacion.Helpers {
    internal class ClientCredentialsHelper {

        private readonly Dictionary<(string tokenUrl, string client_id, string[] scopes), (string access_token, DateTimeOffset expires_at)> accessTokens = [];

        public async Task<string> ObtenerAccessToken(string tokenUrl, string client_id, string client_secret, string[] scopes, int safetyWindow = 30) {
            scopes = [.. scopes.OrderBy(x => x)];

            if (!accessTokens.TryGetValue((tokenUrl, client_id, scopes), out (string access_token, DateTimeOffset expires_at) value) || DateTimeOffset.UtcNow >= value.expires_at - TimeSpan.FromSeconds(safetyWindow)) {

                FormUrlEncodedContent content = new(new Dictionary<string, string> {
                    ["grant_type"] = "client_credentials",
                    ["client_id"] = client_id,
                    ["client_secret"] = client_secret,
                    ["scope"] = string.Join(" ", scopes)
                });

                using HttpClient client = new(new RetryHandler(new HttpClientHandler()));
                HttpResponseMessage response = await client.PostAsync(tokenUrl, content);
                if (!response.IsSuccessStatusCode) {
                    throw new Exception($"Ocurrió un error al obtener el access token - Status Code: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }

                Dictionary<string, object> dictResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(await response.Content.ReadAsStringAsync())!;

                value = (dictResponse["access_token"].ToString()!, DateTimeOffset.UtcNow.AddSeconds((int)dictResponse["expires_in"]));
                accessTokens[(tokenUrl, client_id, scopes)] = value!;
            }

            return value.access_token;
        }
    }
}
