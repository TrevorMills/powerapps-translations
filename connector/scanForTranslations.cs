public class Script : ScriptBase
{
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
    if (this.Context.OperationId == "scanForTranslations")
        {
            return await this.HandleScanForTranslations().ConfigureAwait(false);
        }

        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        response.Content = CreateJsonContent($"Unknown operation ID '{this.Context.OperationId}'");
        return response;
    }

    private async Task<HttpResponseMessage> HandleScanForTranslations()
    {
        var contentAsString = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);
        var contentAsJson = JObject.Parse(contentAsString);

        string code = (string)contentAsJson["code"];
        string component = (string)contentAsJson["component"];

        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(component))
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = CreateJsonContent("Both 'code' and 'component' must be provided.")
            };
        }

        string escapedComponent = Regex.Escape(component);

        // Updated regex to handle escaped quotes
        string pattern = @$"\s*{escapedComponent}\.__\(\s*""((?:\\.|[^""])*)""(?:\s*,\s*""((?:\\.|[^""])*)"")?\s*\),?\s*\r?\n";

        var matches = Regex.Matches(code, pattern);

        JArray results = new JArray();
        foreach (Match match in matches)
        {
            if (match.Groups.Count >= 2)
            {
                string text = Regex.Unescape(match.Groups[1].Value);
                string context = (match.Groups.Count > 2 && match.Groups[2].Success)
                    ? Regex.Unescape(match.Groups[2].Value)
                    : null;

                JObject item = new JObject
                {
                    ["string"] = text,
                    ["context"] = context
                };

                results.Add(item);
            }
        }

        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = CreateJsonContent(results.ToString());
        return response;
    }

}
