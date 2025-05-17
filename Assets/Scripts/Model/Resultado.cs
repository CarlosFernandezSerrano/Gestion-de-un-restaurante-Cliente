
using Newtonsoft.Json;

namespace Assets.Scripts.Model
{
    public class Resultado
    {
        public int Result { get; set; }

        [JsonConstructor]
        public Resultado(int result)
        {
            this.Result = result;
        }
    }
}
