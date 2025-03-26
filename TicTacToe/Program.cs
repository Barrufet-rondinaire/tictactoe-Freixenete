using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TicTacToe;

class Program
{
    private static readonly HttpClient cliente = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
    
    private static List<string> listaJugadores = new List<string>();

    static async Task Main(string[] args)
    {
        await ListaJugadores();
    }

    private static async Task ListaJugadores()
    {
        string participante = await cliente.GetStringAsync("/jugadors");
        listaJugadores = JsonSerializer.Deserialize<List<string>>(participante);

        //var lista = Regex.Match(participant ([A-Za-z]+ [A-Za-z'-]+));
        string patternNom = @"participant ([A-Za-z]+ [A-Za-z'-]+)";
        string[] listaNombre;

        string patternPais = @"representa(nt)? (a|de) ([A-Za-z]+)";
        string[] listaPais;
        
        string patternTodo = @"participant ([A-Za-z]+ [A-Za-z'-]+).*representa(nt)? (a|de) ([A-Za-z])+";
        string[] listaTodo;
        
        MatchCollection matchesNombre = Regex.Matches(participante, patternNom);
        MatchCollection matchesPais = Regex.Matches(participante, patternPais);
        MatchCollection matchesTodo = Regex.Matches(participante, patternTodo);

        string[] listaParticipantes = new string[matchesNombre.Count + matchesPais.Count];

        Match match1 = matchesPais[0];
        
        foreach (Match match in matchesNombre)
        {
            Console.WriteLine($"{match} {match1}");
        }
        
        /*for (int i = 0; i < matchesPais.Count; i++)
        {
            listaParticipantes[i] = matchesNombre[i].Groups[0].Value;
            Console.WriteLine($"{listaParticipantes[i]}");
        }*/
        
        //representa(nt)? (a|de) ([A-Za-z]+)
        //participant ([A-Za-z]+ [A-Za-z'-]+).*representa(nt)? (a|de) ([A-Za-z])+
        
        //Console.WriteLine($"Nombres: {nombres}")};
    }
}