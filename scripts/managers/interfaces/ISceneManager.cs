using System.Collections.Generic;

namespace SinkSub.Managers.Interfaces;

using Preloads = Dictionary<string, Dictionary<string, string>>;

public interface ISceneManager
{
    void _Ready();
    void _EnterTree();
    void ShowLoading();
    void HideLoading();
    void _Process(double delta);
    void ChangeScene(string name);
    void ChangeScene(string name, Preloads preloads);
}
