using System.Text.Json;

public class Http : IDisposable {
    public HttpListener http;
    public HttpListenerContext HttpContext;
    public HttpListenerRequest Request;
    public HttpListenerResponse Response;

    public void Start() {
        http = new HttpListener();
        Config.HttpPrefixes().ForEach(p => http.Prefixes.Add(p));
        http.Start();
        OnHttpStarted?.Invoke(http, EventArgs.Empty);
        while (!http.IsListening) ;
        OnHttpListening?.Invoke(http, EventArgs.Empty);
        do {
            var result = http.BeginGetContext(new AsyncCallback(ListenerCallback), http);
            result.AsyncWaitHandle.WaitOne();
            OnHttpRequest?.Invoke(http, EventArgs.Empty);
        } while (true);
    }

    public void ListenerCallback(IAsyncResult result) {
        HttpListener listener = (HttpListener)result.AsyncState;
        HttpContext = listener.EndGetContext(result);
        Request = HttpContext.Request;
        Response = HttpContext.Response;
        OnHttpContext?.Invoke(listener, EventArgs.Empty);
    }

    public string Receive(HttpListenerRequest request, bool waitForResponse=true) {
        using var ms = new MemoryStream();
        request.InputStream.CopyTo(ms);

        var requestMessage = Encoding.UTF8.GetString(ms.ToArray());
        if (!waitForResponse) {
            HttpContext.Response.Close();
        }
        return requestMessage;
    }

    public void Send(string message, CookieCollection cookies = null, Dictionary<string, string> headers = null) {
        if (!HttpContext.Response.OutputStream.CanWrite)
            throw new Exception("Stream closed");

        headers ??= new Dictionary<string, string>();
        HttpContext.Response.Cookies = cookies;
        foreach (var hd in Config.MyResponseHeaders()) {
            HttpContext.Response.AddHeader(hd.Key, hd.Value);
        }
        HttpContext.Response.AddHeader("license", "metinyakar.net");
        foreach (var hd in headers) {
            HttpContext.Response.AddHeader(hd.Key, hd.Value);
        }
        var responseBytes = Encoding.UTF8.GetBytes(message);
        HttpContext.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
        HttpContext.Response.Close();
    }

    public event EventHandler OnHttpContext;
    public event EventHandler OnHttpStarted;
    public event EventHandler OnHttpListening;
    public event EventHandler OnHttpRequest;

    public void Dispose() {
        HttpContext.Response.Close();
    }
}