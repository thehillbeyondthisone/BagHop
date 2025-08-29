// NavMeshWizard.cs
// AOSharp Plugin: Field recorder for player position navmesh sampling

using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace NavMeshWizard
{
    public class NavMeshWizard : AOPluginEntry
    {
        private bool _scanning = false;
        private bool _verbose = true;
        private readonly List<Vector3> _points = new List<Vector3>();

        public override void Run(string args)
        {
            Chat.RegisterCommand("/navscan", HandleNavScan);
            Chat.RegisterCommand("/navstop", HandleNavStop);
            Chat.RegisterCommand("/navsave", HandleNavSave);
            Chat.RegisterCommand("/navverbose", ToggleVerbose);

            Game.OnUpdate += OnUpdate;

            Chat.WriteLine("[NavMeshWizard] Plugin loaded! Use /navscan to begin sampling.");
        }

        private void HandleNavScan(string cmd, string[] args, ChatWindow window)
        {
            _scanning = true;
            Chat.WriteLine("[NavMeshWizard] SCANNING ENABLED");
        }

        private void HandleNavStop(string cmd, string[] args, ChatWindow window)
        {
            _scanning = false;
            Chat.WriteLine("[NavMeshWizard] Scanning halted.");
        }

        private void ToggleVerbose(string cmd, string[] args, ChatWindow window)
        {
            _verbose = !_verbose;
            Chat.WriteLine($"[NavMeshWizard] Verbose mode: {(_verbose ? "ON" : "OFF")}");
        }

        private void HandleNavSave(string cmd, string[] args, ChatWindow window)
        {
            if (_points.Count == 0)
            {
                Chat.WriteLine("[NavMeshWizard] No samples to save.");
                return;
            }

            int pf = DynelManager.LocalPlayer?.Identity.Instance ?? 0;
            Directory.CreateDirectory("NavMeshes");
            string path = Path.Combine("NavMeshes", $"NavMesh_{pf}.txt");

            File.WriteAllLines(path, _points.Select(p => $"{p.X},{p.Y},{p.Z}"));
            Chat.WriteLine($"[NavMeshWizard] Exported {_points.Count} points to {path}");
        }

        private void OnUpdate(object sender, float dt)
        {
            if (!_scanning || DynelManager.LocalPlayer == null)
                return;

            Vector3 pos = DynelManager.LocalPlayer.Position;
            if (_points.Count == 0 || Vector3.Distance(_points.Last(), pos) > 2.5f)
            {
                _points.Add(pos);
                if (_verbose)
                    Chat.WriteLine($"[NavMeshWizard] Sampled: {pos.X:F1}, {pos.Y:F1}, {pos.Z:F1}");
            }
        }
    }
}
