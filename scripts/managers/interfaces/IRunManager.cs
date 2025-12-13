using System;
using SinkSub.Resources;

namespace SinkSub.Managers.Interfaces;

public interface IRunManager
{
    BiomeResource CurrentBiome { get; }
    ulong Seed { get; }

    event Action<BiomeResource> BiomeChanged;

    void ChangeBiome(BiomeResource newBiome);
    void NewRun();
    float Randf();
    float RandfWfc();
    uint Randi();
    uint RandiWfc();
}