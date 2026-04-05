
using System;
using Microsoft.OpenApi;
class Prop { static void Main() { foreach(var p in typeof(OpenApiSecurityScheme).GetProperties()) Console.WriteLine(p.Name); Console.WriteLine("---"); foreach(var p in typeof(OpenApiInfo).GetProperties()) Console.WriteLine(p.Name); } }

