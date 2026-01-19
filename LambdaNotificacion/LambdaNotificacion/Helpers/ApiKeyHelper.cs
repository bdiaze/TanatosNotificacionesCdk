using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaNotificacion.Helpers {
	internal class ApiKeyHelper(IAmazonAPIGateway client) {
		private readonly Dictionary<string, string> apiKeys = [];

		public async Task<string> ObtenerApiKey(string apiKeyId) {
			if (!apiKeys.TryGetValue(apiKeyId, out string? value)) {
				GetApiKeyResponse response = await client.GetApiKeyAsync(new GetApiKeyRequest {
					ApiKey = apiKeyId,
					IncludeValue = true
				});

				if (response == null || response.Value == null) {
					throw new Exception("No se pudo rescatar correctamente el api key");
				}

				value = response.Value;
				apiKeys[apiKeyId] = value;
			}

			return value;
		}
	}
}
