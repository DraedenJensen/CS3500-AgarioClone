// See https://aka.ms/new-console-template for more information
using Communications;
using Microsoft.Extensions.Logging.Abstractions;
using System.Runtime.CompilerServices;
using System.Text.Json;
using TowardAgarioStepThree;

Networking channel = new(NullLogger.Instance, onConnect, onDisconnect, onMessage, '\n');
channel.Connect("localhost", 11000);

channel.AwaitMessagesAsync();

Console.ReadLine();

void onConnect(Networking channel) { }

void onDisconnect(Networking channel) { }

void onMessage(Networking channel, string message) 
{
    //Console.WriteLine(message);

    if (message.Contains("{Command Food}"))
    {
        //Console.WriteLine(message);

        Food[]? food = JsonSerializer.Deserialize<Food[]>(message.Substring(message.IndexOf('}') + 1));

        for(int i = 0; i < 10; i++)
        {
            string thisFood = JsonSerializer.Serialize(food[i]);
            Console.WriteLine(thisFood);
        }
    }
}