#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Orion.Config;
using Orion.Models.OneDrive;
using Orion.Views;

namespace Orion.Servicios {
    public class OneDriveService {


        // ====================================================================================================
        #region CAMPOS PRIVADOS
        // ====================================================================================================

        private const string REDIRECT_URI = "https://login.microsoftonline.com/consumers/oauth2/nativeclient";
        private const string AUTH_URI = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
        private const string GET_TOKEN_URI = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";

        private static readonly HttpClient httpClient;

        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region CONSTRUCTOR
        // ====================================================================================================

        static OneDriveService() {
            httpClient = new HttpClient();
        }

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region MÉTODOS PÚBLICOS
        // ====================================================================================================


        /// <summary>
        /// Inicia el proceso de autorización.
        /// Si hay un token de refresco que no ha caducado, se usa para generar un token de acceso.
        /// Si no hay token de refresco o no hay token de acceso, se inicia el proceso de autorización.
        /// </summary>
        public async Task AutorizarAsync() {

            if (string.IsNullOrEmpty(Autorizacion.AccessToken)) {
                await GetTokenConCodigoAsync().ConfigureAwait(false);
                return;
            }

            if (ExpiracionTokenAcceso < DateTime.Now) {
                if (ExpiracionTokenRefresco < DateTime.Now) {
                    await GetTokenConCodigoAsync().ConfigureAwait(false);
                } else {
                    await RefreshTokenAsync().ConfigureAwait(false);
                }
            }

        }


        /// <summary>
        /// Devuelve el Json de esta clase codificado.
        /// </summary>
        public string GetConfiguracionCodificada() {
            string textoJson = JsonConvert.SerializeObject(this, Formatting.None);
            return Utils.CodificaTexto(textoJson);
        }


        /// <summary>
        /// Establece los parámetros de la clase de un texto Json codificado.
        /// </summary>
        public void SetConfiguraciónCodificada(string textoJson) {
            JsonConvert.PopulateObject(Utils.DescodificaTexto(textoJson), this);
        }



        #endregion
        // ====================================================================================================


        // ====================================================================================================
        #region MÉTODOS PRIVADOS
        // ====================================================================================================

        /// <summary>
        /// Inicia el proceso de autenticación en OneDrive usando el Code Flow.
        /// </summary>
        [STAThread]
        private async Task GetTokenConCodigoAsync() {

            // Se construye la URL de autorización.
            string url = AUTH_URI +
                $"? client_id ={ApiKeys.OneDriveClientId}&" +
                $"scope={ApiKeys.OneDriveScope}&" +
                $"response_type=code&" +
                $"redirect_uri ={REDIRECT_URI}";

            // Creamos y mostramos la ventana donde se hará el login.
            var ventanaWeb = new VentanaWeb(new Uri(url), new Uri(REDIRECT_URI));
            var resultado = ventanaWeb.ShowDialog();

            if (resultado == true) {
                // Si la ventana se cerró sola...
                if (ventanaWeb.PaginaInicial.ToString().StartsWith(REDIRECT_URI + "?code=")) {
                    // Si ha devuelto un código, lo usamos para traer el token.
                    var codigo = ventanaWeb.PaginaInicial.ToString().Replace("https://login.microsoftonline.com/consumers/oauth2/nativeclient?code=", "");
                    var parametros = new Dictionary<string, string> {
                        { "client_id", ApiKeys.OneDriveClientId },
                        { "redirect_uri", REDIRECT_URI },
                        { "code", codigo },
                        { "grant_type", "authorization_code" },
                    };
                    var texto = await JsonFromPostAsync(GET_TOKEN_URI, parametros).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(texto)) {
                        JsonConvert.PopulateObject(texto, Autorizacion);
                        ExpiracionTokenAcceso = DateTime.Now.AddSeconds(Autorizacion.ExpiresIn);
                        ExpiracionTokenRefresco = DateTime.Now.AddMonths(6);
                    } else {
                        Autorizacion = new TokenResponse();
                    }
                } else {
                    // Si no ha traido un código, anulamos la autorización
                    Autorizacion = new TokenResponse();
                }
            } else {
                // Si se cerró la ventana, anulamos la autorización.
                Autorizacion = new TokenResponse();
            }
        }


        /// <summary>
        /// Obtiene un token de acceso a partir del token de refresco.
        /// </summary>
        [STAThread]
        private async Task RefreshTokenAsync() {
            var parametros = new Dictionary<string, string> {
                        { "client_id", ApiKeys.OneDriveClientId },
                        { "redirect_uri", REDIRECT_URI },
                        { "refresh_token", Autorizacion.RefreshToken },
                        { "grant_type", "refresh_token" },
                    };
            var texto = await JsonFromPostAsync(GET_TOKEN_URI, parametros);
            if (!string.IsNullOrEmpty(texto)) {
                JsonConvert.PopulateObject(texto, Autorizacion);
                ExpiracionTokenAcceso = DateTime.Now.AddSeconds(Autorizacion.ExpiresIn);
                ExpiracionTokenRefresco = DateTime.Now.AddMonths(6);
            } else {
                Autorizacion = new TokenResponse();
                await GetTokenConCodigoAsync();
            }
        }


        /// <summary>
        /// Hace una llamada POST con parámetros a una url devolviendo el texto de la respuesta o una cadena vacía si hay error.
        /// </summary>
        private async Task<string> JsonFromPostAsync(string url, Dictionary<string, string> parametros) {
            var resultado = "";
            var contenido = new FormUrlEncodedContent(parametros);
            var respuesta = await httpClient.PostAsync(url, contenido).ConfigureAwait(false);
            if (respuesta.StatusCode == System.Net.HttpStatusCode.OK) {
                resultado = await respuesta.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            return resultado;
        }

        /*
        Para hacer un get con token.
                
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Autorizacion.AccessToken);
        var json = await httpClient.GetStringAsync(uri);

        */

        #endregion
        // ====================================================================================================



        // ====================================================================================================
        #region PROPIEDADES
        // ====================================================================================================

        public TokenResponse Autorizacion { get; set; } = new TokenResponse();

        public DateTime ExpiracionTokenAcceso { get; set; }

        public DateTime ExpiracionTokenRefresco { get; set; }



        #endregion
        // ====================================================================================================


    }
}
