using LambdaNotificacion.Helpers;
using LambdaNotificacion.Models;
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
        private readonly string _baseUrl = parameterStore.ObtenerParametro(variableEntorno.Obtener("ARN_PARAMETER_TANATOS_API_URL")).Result;
        private readonly string _cognitoBaseUrl = JsonSerializer.Deserialize<Dictionary<string, string>>(secretManagerHelper.ObtenerSecreto(variableEntorno.Obtener("ARN_SECRET_TANATOS_API")).Result)!["CognitoBaseUrl"];
        private readonly string _notificacionesClientId = JsonSerializer.Deserialize<Dictionary<string, string>>(secretManagerHelper.ObtenerSecreto(variableEntorno.Obtener("ARN_SECRET_TANATOS_API")).Result)!["NotificacionesUserPoolClientId"];
        private readonly string _notificacionesClientSecret = JsonSerializer.Deserialize<Dictionary<string, string>>(secretManagerHelper.ObtenerSecreto(variableEntorno.Obtener("ARN_SECRET_TANATOS_API")).Result)!["NotificacionesUserPoolClientSecret"];

        private readonly string[] _notificacionesScopes = [
			"api/negocios.read.all",
			"api/obligaciones.read.all",
			"api/vencimientos.read.all",
			"api/vencimientos.write.all",
			"api/sistema.read.public",
			"api/templates.read.public"
		];

		public async Task ProcesarNotificacion(EntradaLambda entrada) {
            using HttpClient client = new(new RetryHandler(new HttpClientHandler()));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await clientCredentialsHelper.ObtenerAccessToken(_cognitoBaseUrl, _notificacionesClientId, _notificacionesClientSecret, _notificacionesScopes));

            HttpResponseMessage response = await client.PostAsync(_baseUrl + "/NormaSuscrita/ProcesarNotificacion", new StringContent(JsonSerializer.Serialize(entrada), Encoding.UTF8, "application/json"));
			if (!response.IsSuccessStatusCode) {
				throw new Exception($"Ocurrió un error al procesar la notificación. StatusCode: {response.StatusCode} - Content: {await response.Content.ReadAsStringAsync()}");
			}
		}
    }
}
