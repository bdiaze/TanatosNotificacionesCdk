using LambdaNotificacion.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaNotificacion.Helpers {
	internal class HermesHelper(VariableEntornoHelper variableEntorno, ParameterStoreHelper parameterStore, ApiKeyHelper apiKey) {
		private readonly string _baseUrl = parameterStore.ObtenerParametro(variableEntorno.Obtener("ARN_PARAMETER_HERMES_API_URL")).Result;
		private readonly string _xApiKey = apiKey.ObtenerApiKey(parameterStore.ObtenerParametro(variableEntorno.Obtener("ARN_PARAMETER_HERMES_API_KEY_ID")).Result).Result;
		private readonly DireccionCorreo remitenteDefecto = JsonConvert.DeserializeObject<DireccionCorreo>(parameterStore.ObtenerParametro(variableEntorno.Obtener("ARN_PARAMETER_DIRECCION_DE_DEFECTO")).Result)!;

		public async Task<HermesRetorno> EnviarCorreo(HermesCorreo correo) {
			correo.De ??= remitenteDefecto;

			using HttpClient client = new(new RetryHandler(new HttpClientHandler()));
			client.DefaultRequestHeaders.Add("x-api-key", _xApiKey);
			HttpResponseMessage response = await client.PostAsync(_baseUrl + "Correo/Enviar", new StringContent(JsonConvert.SerializeObject(correo), Encoding.UTF8, "application/json"));
			return JsonConvert.DeserializeObject<HermesRetorno>(await response.Content.ReadAsStringAsync())!;
		}
	}
}
