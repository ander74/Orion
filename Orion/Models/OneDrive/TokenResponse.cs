#region COPYRIGHT
// ===============================================
//     Copyright 2017 - Orion 1.0 - A. Herrero    
// -----------------------------------------------
//  Vea el archivo Licencia.txt para más detalles 
// ===============================================
#endregion
using Newtonsoft.Json;

namespace Orion.Models.OneDrive {

    public class TokenResponse {


        [JsonProperty("token_type")]
        public string TokenType { get; set; }


        [JsonProperty("scope")]
        public string Scope { get; set; }


        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }


        [JsonProperty("ext_expires_in")]
        public int ExtExpiresIn { get; set; }


        [JsonProperty("access_token")]
        public string AccessToken { get; set; }


        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }


    }
}
