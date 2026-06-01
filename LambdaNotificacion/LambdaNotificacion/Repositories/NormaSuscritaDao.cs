using LambdaNotificacion.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LambdaNotificacion.Repositories {
    internal class NormaSuscritaDao(VariableEntornoHelper variableEntorno, ParameterStoreHelper parameterStore, SecretManagerHelper secretManagerHelper, ClientCredentialsHelper clientCredentialsHelper) {
        private readonly string[] _notificacionesScopes = [
			"api/negocios.read.all",
			"api/obligaciones.read.all",
			"api/vencimientos.read.all",
			"api/vencimientos.write.all",
			"api/sistema.read.public",
			"api/templates.read.public"
		];

		private readonly Lazy<Task<ApiConfig>> _config = new(() => InicializarApiConfig(variableEntorno, parameterStore, secretManagerHelper));

		private static async Task<ApiConfig> InicializarApiConfig(VariableEntornoHelper variableEntorno, ParameterStoreHelper parameterStore, SecretManagerHelper secretManagerHelper) {
			Task<string> taskParametro = parameterStore.ObtenerParametro(variableEntorno.Obtener("ARN_PARAMETER_TANATOS_API_URL"));
			Task<string> taskSecreto = secretManagerHelper.ObtenerSecreto(variableEntorno.Obtener("ARN_SECRET_TANATOS_API"));

			await Task.WhenAll(taskParametro, taskSecreto);
			string baseUrl = taskParametro.Result;
			Dictionary<string, string> secretos = JsonSerializer.Deserialize<Dictionary<string, string>>(taskSecreto.Result)!;

			return new ApiConfig {
				BaseUrl = baseUrl,
				CognitoBaseUrl = secretos["CognitoBaseUrl"],
				NotificacionesClientId = secretos["NotificacionesUserPoolClientId"],
				NotificacionesClientSecret = secretos["NotificacionesUserPoolClientSecret"],
			};
		}

		public async Task ProcesarNotificacion(string rawJson) {
			ApiConfig config = await _config.Value;

			using HttpClient client = new(new RetryHandler(new HttpClientHandler()));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await clientCredentialsHelper.ObtenerAccessToken(
				config.CognitoBaseUrl,
				config.NotificacionesClientId,
				config.NotificacionesClientSecret, 
				_notificacionesScopes
			));

            HttpResponseMessage response = await client.PostAsync(config.BaseUrl + "/NormaSuscrita/ProcesarNotificacion", new StringContent(rawJson, Encoding.UTF8, "application/json"));
			if (!response.IsSuccessStatusCode) {
				throw new HttpRequestException(
					$"Ocurrió un error al procesar la notificación. StatusCode: {response.StatusCode} - Content: {await response.Content.ReadAsStringAsync()}",
					inner: null,
					statusCode: response.StatusCode
				);
			}
		}

		internal class ApiConfig {
			public required string BaseUrl { get; init; }
			public required string CognitoBaseUrl { get; init; }
			public required string NotificacionesClientId { get; init; }
			public required string NotificacionesClientSecret { get; init; }
		}
	}
}
