
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using KSP.Sim.Maneuver;
using KSP.Messages;
using BepInEx.Logging;


namespace AutoExecuteNode
{
    public class GenericPilot
    {
        public bool finished = false;
        public string status_line = "";

        public virtual void Start()
        {
            finished = false;
        }

        public virtual void onUpdate()
        {
            throw new System.NotImplementedException();
        }

        public virtual void onGui()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AutoExecuteManeuver
    {
        public ManualLogSource logger;

        public static AutoExecuteManeuver Instance { get; set; }

        public ManeuverNodeData current_maneuvre_node = null;

        // Sub Pilots
        TurnToManeuvre turn;
        WarpToManeuvre warp;
        BurnManeuvre burn;

        GenericPilot current_pilot = null;

        public AutoExecuteManeuver(ManualLogSource logger)
        {
            this.logger = logger;
            logger.LogMessage("AutoExecuteManeuver !");
            Instance = this;

            Tools.Game().Messages.Subscribe<VesselChangedMessage>(OnActiveVesselChanged);

            turn = new TurnToManeuvre(this);
            warp = new WarpToManeuvre(this);
            burn = new BurnManeuvre(this);
        }

        public void OnActiveVesselChanged(MessageCenterMessage msg)
        {
            Stop();
        }


        public enum Mode
        {
            Off,
            Turn,
            Warp,
            Burn
        }

        public bool debug_infos = false;

        public Mode mode = Mode.Off;

        public void setMode(Mode mode)
        {
            if (mode == this.mode)
                return;

            logger.LogInfo("setMode " + mode);

            this.mode = mode;

            if (mode == Mode.Off)
            {
                TimeWarpTools.time_warp()?.SetRateIndex(0, true);
                current_pilot = null;
                return;
            }

            switch (mode)
            {
                case Mode.Off: return;
                case Mode.Turn:
                    current_pilot = turn;
                    break;
                case Mode.Warp:
                    current_pilot = warp;
                    break;
                case Mode.Burn:
                    current_pilot = burn;
                    break;
            }

            logger.LogInfo("current_pilot " + current_pilot);

            current_pilot.Start();
        }

        public void nextMode()
        {
            if (mode == Mode.Off)
            {
                Run();
                return;
            }
            if (mode == Mode.Burn)
            {
                Stop();
                return;
            }

            var next = this.mode + 1;
            setMode(next);
        }

        public void onGUI()
        {
            if (current_maneuvre_node == null)
            {
                GUILayout.Label("no Maneuvre node");
                return;
            }

            if (mode == Mode.Off)
            {
                if (GUILayout.Button("Run"))
                    Run();
            }
            else
            {
                if (GUILayout.Button("Stop !!!"))
                    Stop();
            }

            debug_infos = GUILayout.Toggle(debug_infos, "debug mode");

            node_infos();

            if (current_pilot != null)
            {
                current_pilot.onGui();

                if (debug_infos)
                {
                    if (GUILayout.Button("Next"))
                        nextMode();
                }
            }
            else
                GUILayout.Label("No current pilot");
        }

        public void Update()
        {
            current_maneuvre_node = Tools.getNextManeuveurNode();
            if (current_maneuvre_node == null)
            {
                Stop();
            }

            if (current_pilot != null)
            {
                current_pilot.onUpdate();
                if (current_pilot.finished && !debug_infos)
                {
                    // auto next
                    nextMode();
                }
            }
        }

        void node_infos()
        {

            if (debug_infos)
            {
                var dt = Tools.remainingStartTime(current_maneuvre_node);
                GUILayout.Label($"tic tac {Tools.printDuration(dt)} s ");
                if (dt < 0)
                {
                    GUILayout.Label("In The Past");
                    return;
                }

                GUILayout.Label($"BurnDuration {current_maneuvre_node.BurnDuration}");
                GUILayout.Label($"BurnRequiredDV {current_maneuvre_node.BurnRequiredDV}");
                GUILayout.Label($"BurnVector {Tools.printVector(current_maneuvre_node.BurnVector)}");
            }

            //GUILayout.Label($"Time {nextNode.Time}");
        }

        public void Run()
        {
            setMode(Mode.Turn);
        }

        public void Stop()
        {
            setMode(Mode.Off);
        }
    }
}