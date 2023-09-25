using System.Net;
using System.Text.Json;

Config.Prepare(); 
var http = new Http();

http.OnHttpStarted += (s, e) => {
    Console.WriteLine("Http started");
};

http.OnHttpListening += (s, e) => {
    Console.WriteLine("Http listening");
};

http.OnHttpContext += (s, e) => {
    var message = http.Receive(http.Request);
    http.Send(message);
};

http.OnHttpRequest += (s, e) => {
    Console.WriteLine($"Received new request");
};

http.Start();