using Ks.GitHub.Nuget.Sample.Json;

namespace Ks.GitHub.Nuget.Sample.Model
{
    public class Sample
    {
        public Sample() { }

        public string ConvertToJson(SampleModel model)
            => model.ToJson();
    }
}
