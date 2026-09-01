using System.Globalization;
using System.Text.Json.Serialization;

namespace pruebadedesempeño.Models
{
    public class TrmRate
    {
        [JsonPropertyName("valor")]
        public string ValueString { get; set; }

        [JsonPropertyName("unidad")]
        public string Unit { get; set; }

        [JsonPropertyName("vigenciadesde")]
        public string ValidFrom { get; set; }

        [JsonPropertyName("vigenciahasta")]
        public string ValidTo { get; set; }

        public decimal Value
        {
            get
            {
                if (decimal.TryParse(ValueString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }
                return 0;
            }
        }
    }
}
