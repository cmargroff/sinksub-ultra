using System;
using Godot;
using SinkSub.Managers.Interfaces;
using SinkSub.Resources;

namespace SinkSub.Managers;

public class RunManager : IRunManager
{
    private BiomeResource _currentBiome;
    public BiomeResource CurrentBiome => _currentBiome;
    public event Action<BiomeResource> BiomeChanged;
    public ulong Seed { get; private set; }

    private RandomNumberGenerator _rng;
    private RandomNumberGenerator _wfcRng;
    public RunManager()
    {
        _rng = new RandomNumberGenerator();
        _wfcRng = new RandomNumberGenerator();
        NewRun();
    }
    public void NewRun()
    {
        GenerateSeed();
        // other steps like holding run options can go here
    }
    private void GenerateSeed()
    {
        // generate new seed and reset rng states
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(sizeof(ulong));
        Seed = BitConverter.ToUInt64(bytes, 0);
        _wfcRng.Seed = Seed;
        _wfcRng.State = 0;
        _rng.Seed = Seed;
        _rng.State = 0;
    }

    // not reimplementing the entire rng interface for now
    public uint Randi()
    {
        return _rng.Randi();
    }
    public float Randf()
    {
        return _rng.Randf();
    }
    public uint RandiWfc()
    {
        return _wfcRng.Randi();
    }
    public float RandfWfc()
    {
        return _wfcRng.Randf();
    }

    public void ChangeBiome(BiomeResource newBiome)
    {
        _currentBiome = newBiome;
        BiomeChanged?.Invoke(newBiome);
    }
}