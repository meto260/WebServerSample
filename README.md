# WebServerSample
An http server with basic tools for multipurpose use

![metinyakar.WebServerSample](https://raw.githubusercontent.com/meto260/WebServerSample/master/webserver_ss.png)

  Example:
```csharp
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
    Config.MyResponseHeaders().Select(x => {
        http.Response.AddHeader(x.Key, x.Value);
        return x;
    });
    byte[] responseBytes = Encoding.UTF8.GetBytes(requestMsg);
    http.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
    http.Response.OutputStream.Close();
};

http.OnHttpRequest += (s, e) => {
    Console.WriteLine($"Received new request");
};

http.Start();
```
</code>
