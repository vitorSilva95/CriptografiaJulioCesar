using Newtonsoft.Json;
using System;

namespace CriptografiaJulioCesar.Models
{
    public partial class PayloadCriptografiaJulioCesar
    {
        [JsonProperty("numero_casas")]
        public int NumeroCasas { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("cifrado")]
        public string Cifrado { get; set; }

        [JsonProperty("decifrado")]
        public string Decifrado { get; set; }

        [JsonProperty("resumo_criptografico")]
        public string ResumoCriptografico { get; set; }

        public void Decifrar()
        {
            var alfabeto = new char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};
            var alfabetoCriptografado = new char[26];
            for (int i = 0; i < alfabeto.Length; i++)
            {
                var indiceAlfabetoNovo = i + this.NumeroCasas;
                if (indiceAlfabetoNovo > alfabeto.Length-1)
                {
                    indiceAlfabetoNovo -= alfabeto.Length;
                }
  
                alfabetoCriptografado[i] = alfabeto[indiceAlfabetoNovo];
            }

            var cifradoArray = this.Cifrado.ToCharArray();
            for (int i = 0; i < cifradoArray.Length; i++)
            {
                var indice = Array.IndexOf(alfabetoCriptografado, cifradoArray[i]);
                if (indice < 0)
                {
                    Decifrado += cifradoArray[i];
                }
                else
                    Decifrado += alfabeto[indice];
            }
        }
    }
}
