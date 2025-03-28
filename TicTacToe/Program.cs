using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;


namespace TicTacToe;

class Program
{
    private static readonly HttpClient cliente = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
    
    private static Dictionary<string, string> jugadores = new();
    private static Dictionary<string, int> victorias = new();
    //private static List<string> listaJugadores = new List<string>();

    static async Task Main(string[] args)
    {
        await ListaJugadores();
        await Partidas();
        MostrarGanador();
    }

    private static async Task ListaJugadores()
    {
        string participantes = await cliente.GetStringAsync("/jugadors");
        var listaJugadores = JsonSerializer.Deserialize<List<string>>(participantes);

        Regex regex = new Regex(@"participant ([A-Z]+\w+ [A-Z-'-a-z]+\w+).*representa(nt)? (a |de )([A-Z-a-z]+\w+)");
        
        foreach (var respuesta in listaJugadores)
        {
            Match match = regex.Match(respuesta);
            if (match.Success)
            {
                string nombre = match.Groups[1].Value;
                string pais = match.Groups[4].Value;
                if (pais != "Espanya")
                {
                    jugadores[nombre] = pais;
                    if (victorias.ContainsKey(nombre))
                    {
                        victorias[nombre] = victorias[nombre] + 1;
                    }
                    else
                    {
                        victorias[nombre] = 1;
                    }
                }
            }
        }
    }

    private static async Task Partidas()
    {
        for (int i = 1; i <= 10000; i++)
        {
            try
            {
                string partidaJson = await cliente.GetStringAsync($"/partida/{i}");
                var partida = JsonSerializer.Deserialize<Partida>(partidaJson);

                if (jugadores.ContainsKey(partida.Jugador1) && jugadores.ContainsKey(partida.Jugador2))
                {
                    string ganador = VerGanador(partida.Tauler);
                    if (ganador == "O" && victorias.ContainsKey(partida.Jugador1))
                    {
                        victorias[partida.Jugador1]++;
                    }
                    else if (ganador == "X" && victorias.ContainsKey(partida.Jugador2))
                    {
                        victorias[partida.Jugador2]++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar partida {i}: {ex.Message}");
            }
        }
    }

    private static string VerGanador(List<string> tablero)
    {
        string[] lineas = new string[8]
        {
            tablero[0], tablero[1], tablero[2],  // Filas
            "" + tablero[0][0] + tablero[1][0] + tablero[2][0], // Columnas
            "" + tablero[0][1] + tablero[1][1] + tablero[2][1],
            "" + tablero[0][2] + tablero[1][2] + tablero[2][2],
            "" + tablero[0][0] + tablero[1][1] + tablero[2][2], // Diagonales
            "" + tablero[0][2] + tablero[1][1] + tablero[2][0]
        };

        if (Array.Exists(lineas, a => a == "XXX"))
        {
            return "X";
        }
        if (Array.Exists(lineas, a => a == "OOO"))
        {
            return "O";
        }
        return "";
    }

    private static void MostrarGanador()
    {
        int maxVictorias = 0;
        List<string> ganadores = new();

        foreach (var jugador in victorias)
        {
            if (jugador.Value > maxVictorias)
            {
                maxVictorias = jugador.Value;
                ganadores.Clear();
                ganadores.Add(jugador.Key);
            }
            else if (jugador.Value == maxVictorias)
            {
                ganadores.Add(jugador.Key);
            }
        }
        
        Console.WriteLine("Ganador del torneo:");
        foreach (var ganador in ganadores)
        {
            Console.WriteLine($"{ganador} de {jugadores[ganador]}");
        }
    }
}