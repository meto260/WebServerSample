public class Http {
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

    public void Resolve(dynamic body, CookieCollection cookies= null, Dictionary<string, string> headers = null) {
        headers ??= new Dictionary<string, string>();
        HttpContext.Response.Cookies = cookies;
        foreach(var hd in Config.MyResponseHeaders()) {
            HttpContext.Response.AddHeader(hd.Key, hd.Value);
        }
        HttpContext.Response.AddHeader("license", "metinyakar.net");
        foreach (var hd in headers) {
            HttpContext.Response.AddHeader(hd.Key, hd.Value);
        }

        string requestMsg = JsonConvert.SerializeObject(body, new JsonSerializerSettings {
            Error = (s, e) => e.ErrorContext.Handled = true
        });

        byte[] responseBytes = Encoding.UTF8.GetBytes(requestMsg);
        HttpContext.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
        HttpContext.Response.OutputStream.Close();
    }


    public event EventHandler OnHttpContext;
    public event EventHandler OnHttpStarted;
    public event EventHandler OnHttpListening;
    public event EventHandler OnHttpRequest;
}