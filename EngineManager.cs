using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CBBL.src.AI;
using CBBL.src.Board;
using CBBL.src.Implementation;
using CBBL.src.Interfaces;
using CBBL.src.Util;
using CBBTop.AI;

namespace CBBTop;
public class EngineManager
{
    private Process? _engineProcess;
    private StreamWriter _input;
    private StreamReader _output;

    public IBoard? Board { get; set; }
    public IAIPlayer? AIPlayer { get; set; }

    public event Action<string>? OnEngineOutput;

    public EngineManager()
    {
        Context.Register(new CBBLServiceLoader());
        Context.Instance.Init();
    }

    public void Start(string path, string[]? args = null)
    {

        if (_engineProcess != null && !_engineProcess.HasExited)
        {
            OnEngineOutput?.Invoke("Engine is already running.");
            return;
        }

        if (path == null || !File.Exists(path))
        {
            OnEngineOutput?.Invoke("Engine path is invalid.");
            return;
        }

        _engineProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        _engineProcess.Start();

        _input = _engineProcess.StandardInput;
        _output = _engineProcess.StandardOutput;

        Task.Run(ReadOutputLoop);
        OnEngineOutput?.Invoke("Connecting to engine at " + path);
    }

    public IBoard Start(int depth = 3)
    {
        Board = new CBBLBoard();
        AIPlayer = new Player(Board.State, PlayerColor.Black, depth);
       
        OnEngineOutput?.Invoke("Console engine started. Type commands to interact.");
        return Board;
    }

    public IBoard Start(string fen, int depth = 3)
    {
        Board = new CBBLBoard(fen);
        var opposite = Board.State.ActivePlayer == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        AIPlayer = new Player(Board.State, opposite, depth);

        OnEngineOutput?.Invoke("Console engine started with FEN: " + fen);
        return Board;
    }

    public IBoard Start(PlayerColor color, int depth = 3)
    {
        Board = new CBBLBoard(color);
        var opposite = color == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;
        AIPlayer = new Player(Board.State, opposite, depth);

        OnEngineOutput?.Invoke("Console engine started with player color: " + color);
        return Board;
    }

    private async Task ReadOutputLoop()
    {
        while (!_output.EndOfStream)
        {
            var line = await _output.ReadLineAsync();
            if (line != null)
            {
                OnEngineOutput?.Invoke(line);
            }
        }
    }

    public void Send(string command)
    {
        if (_input == null || _engineProcess == null || _engineProcess.HasExited)
        {
            OnEngineOutput?.Invoke("Engine is not running.");
            return;
        }
        _input.WriteLine(command);
        _input.Flush();
    }

    public void Stop()
    {
        if (_engineProcess != null && !_engineProcess.HasExited)
        {
            Send("");
            _engineProcess.Kill();
            _engineProcess.Dispose();
            _engineProcess = null;
            OnEngineOutput?.Invoke("Engine stopped.");
        }
        else
        {
            OnEngineOutput?.Invoke("Engine is not running.");
        }
    }
}