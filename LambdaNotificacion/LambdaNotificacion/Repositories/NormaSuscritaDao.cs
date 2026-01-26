using LambdaNotificacion.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LambdaNotificacion.Repositories {
    internal class NormaSuscritaDao(VariableEntornoHelper variableEntorno, ParameterStoreHelper parameterStore, SecretManagerHelper secretManagerHelper, ClientCredentialsHelper clientCredentialsHelper) {
        private readonly string _baseUrl = parameterStore.ObtenerParametro(variableEntorno.Obtener("ARN_PARAMETER_TANATOS_API_URL")).Result;
        private readonly string _client_id = JsonSerializer.Deserialize<Dictionary<string, string>>(secretManagerHelper.ObtenerSecreto(variableEntorno.Obtener("ARN_SECRET_TANATOS_API")).Result)!["client_id"];
        private readonly string _client_secret = JsonSerializer.Deserialize<Dictionary<string, string>>(secretManagerHelper.ObtenerSecreto(variableEntorno.Obtener("ARN_SECRET_TANATOS_API")).Result)!["client_secret"];

    }
}
