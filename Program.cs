//metinyakar.net
Config.Prepare();
var http = new Http();

http.OnHttpStarted += (s, e) => {
    Console.WriteLine("Http started");
};

http.OnHttpListening += (s, e) => {
    Console.WriteLine("Http listening");
};

http.OnHttpContext += (s, e) => {
    var requestMsg = JsonConvert.SerializeObject(http.Request, new JsonSerializerSettings {
        Formatting = Formatting.Indented,
        Error = (s, e) => e.ErrorContext.Handled = true
    });
    Console.WriteLine(requestMsg);
    http.Resolve(http.Request);
};

http.OnHttpRequest += (s, e) => {
    Console.WriteLine($"Received new request");
};

http.Start();
