// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using TowardAgarioStepTwo;

Person drae = new("Draeden", (float)3.8);
string message = JsonSerializer.Serialize(drae);

Console.WriteLine(message);

Person? temp = JsonSerializer.Deserialize<Person>(message);

Person jim = new("Jim", (float)3.0);
Person dav = new("Dav", (float)3.2);
Person erin = new("Erin", (float)3.4);
Person mary = new("Mary", (float)3.6);
Person pat = new("Pat", (float)3.8);

List<Person> people = new();

people.Add(jim);
people.Add(dav);
people.Add(erin);
people.Add(mary);
people.Add(pat);

string peopleList = JsonSerializer.Serialize(people, new JsonSerializerOptions{WriteIndented = true});
Console.WriteLine(peopleList);

List<Person>? tempList = JsonSerializer.Deserialize<List<Person>>(peopleList);

Console.WriteLine("finished");