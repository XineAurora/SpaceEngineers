//#if DEBUG
//using System;
//using System.Linq;
//using System.Text;
//using System.Collections;
//using System.Collections.Generic;

//using VRageMath;
//using VRage.Game;
//using VRage.Collections;
//using Sandbox.ModAPI.Ingame;
//using VRage.Game.Components;
//using VRage.Game.ModAPI.Ingame;
//using Sandbox.ModAPI.Interfaces;
//using Sandbox.Game.EntityComponents;
//using SpaceEngineers.Game.ModAPI.Ingame;
//using VRage.Game.ObjectBuilders.Definitions;

//namespace SpaceEngineers
//{
//    public sealed class Program : MyGridProgram
//    {
//#endif
//        //=======================================================================
//        //////////////////////////BEGIN//////////////////////////////////////////
//        //=======================================================================
//        string gyroGroupName = "Gyros";
//        string connectorName = "Connector";
//        string cockpitName = "ControlSeat";
//        string thrustersGroupName = "Thrusters";

//        bool isRunning = false;

//        IMyTextPanel textPanel;

//        IMyCockpit cockpit;
//        ThrusterControl thrusterControl;

//        public Program()
//        {
//            Runtime.UpdateFrequency = UpdateFrequency.Update1;
//            thrusterControl = new ThrusterControl(this, cockpitName, thrustersGroupName);
//            cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;
//        }


//        public void Save()
//        {
//            // Используйте этот метод, чтобы сохранить состояние программы в поле Storage,
//        }

//        public void Main(string argument, UpdateType updateSource)
//        {
//            switch (argument)
//            {
//                case "run":
//                    isRunning = true;
//                    break;
//                case "stop":
//                    isRunning = false;
//                    break;
//            }
//            if (isRunning)
//            {
//                thrusterControl.SetThrust(-cockpit.GetNaturalGravity());
//            }
//            else
//            {
//                thrusterControl.ResetThrust();
//            }
//        }

//        public class ThrusterControl
//        {
//            private List<IMyThrust> _forwardThrusters;
//            private List<IMyThrust> _backwardThrusters;
//            private List<IMyThrust> _leftThrusters;
//            private List<IMyThrust> _rightThrusters;
//            private List<IMyThrust> _upThrusters;
//            private List<IMyThrust> _downThrusters;

//            public double ForwardMaxThrust { get; protected set; } = 0;
//            public double BackwardMaxThrust { get; protected set; } = 0;
//            public double LeftMaxThrust { get; protected set; } = 0;
//            public double RightMaxThrust { get; protected set; } = 0;
//            public double UpMaxThrust { get; protected set; } = 0;
//            public double DownMaxThrust { get; protected set; } = 0;

//            private IMyCockpit _cockpit;

//            public ThrusterControl(Program program, string mainCockpitName, string thrustersGroupName)
//            {
//                _cockpit = program.GridTerminalSystem.GetBlockWithName(mainCockpitName) as IMyCockpit;
//                Matrix cockpitMatrix;
//                _cockpit.Orientation.GetMatrix(out cockpitMatrix);

//                _forwardThrusters = new List<IMyThrust>();
//                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_forwardThrusters, (thruster) =>
//                {
//                    Matrix thrusterMatrix;
//                    thruster.Orientation.GetMatrix(out thrusterMatrix);
//                    if (thrusterMatrix.Forward == cockpitMatrix.Backward) return true;
//                    return false;
//                });
//                _backwardThrusters = new List<IMyThrust>();
//                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_backwardThrusters, (thruster) =>
//                {
//                    Matrix thrusterMatrix;
//                    thruster.Orientation.GetMatrix(out thrusterMatrix);
//                    if (thrusterMatrix.Forward == cockpitMatrix.Forward) return true;
//                    return false;
//                });
//                _leftThrusters = new List<IMyThrust>();
//                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_leftThrusters, (thruster) =>
//                {
//                    Matrix thrusterMatrix;
//                    thruster.Orientation.GetMatrix(out thrusterMatrix);
//                    if (thrusterMatrix.Forward == cockpitMatrix.Right) return true;
//                    return false;
//                });
//                _rightThrusters = new List<IMyThrust>();
//                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_rightThrusters, (thruster) =>
//                {
//                    Matrix thrusterMatrix;
//                    thruster.Orientation.GetMatrix(out thrusterMatrix);
//                    if (thrusterMatrix.Forward == cockpitMatrix.Left) return true;
//                    return false;
//                });
//                _upThrusters = new List<IMyThrust>();
//                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_upThrusters, (thruster) =>
//                {
//                    Matrix thrusterMatrix;
//                    thruster.Orientation.GetMatrix(out thrusterMatrix);
//                    if (thrusterMatrix.Forward == cockpitMatrix.Down) return true;
//                    return false;
//                });
//                _downThrusters = new List<IMyThrust>();
//                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_downThrusters, (thruster) =>
//                {
//                    Matrix thrusterMatrix;
//                    thruster.Orientation.GetMatrix(out thrusterMatrix);
//                    if (thrusterMatrix.Forward == cockpitMatrix.Up) return true;
//                    return false;
//                });
//            }

//            private void CalculateMaxThrust()
//            {
//                ForwardMaxThrust = 0;
//                foreach (IMyThrust thruster in _forwardThrusters)
//                {
//                    ForwardMaxThrust += thruster.MaxEffectiveThrust;
//                }
//                BackwardMaxThrust = 0;
//                foreach (IMyThrust thruster in _backwardThrusters)
//                {
//                    BackwardMaxThrust += thruster.MaxEffectiveThrust;
//                }
//                LeftMaxThrust = 0;
//                foreach (IMyThrust thruster in _leftThrusters)
//                {
//                    LeftMaxThrust += thruster.MaxEffectiveThrust;
//                }
//                RightMaxThrust = 0;
//                foreach (IMyThrust thruster in _rightThrusters)
//                {
//                    RightMaxThrust += thruster.MaxEffectiveThrust;
//                }
//                UpMaxThrust = 0;
//                foreach (IMyThrust thruster in _upThrusters)
//                {
//                    UpMaxThrust += thruster.MaxEffectiveThrust;
//                }
//                DownMaxThrust = 0;
//                foreach (IMyThrust thruster in _downThrusters)
//                {
//                    DownMaxThrust += thruster.MaxEffectiveThrust;
//                }
//            }

//            public void SetThrust(Vector3D acceleration)
//            {
//                CalculateMaxThrust();
//                Matrix cockpitMatrix = _cockpit.WorldMatrix;
//                double shipMass = _cockpit.CalculateShipMass().PhysicalMass;
//                float force = (float)(acceleration.Dot(cockpitMatrix.Forward) * shipMass / ForwardMaxThrust);
//                foreach (IMyThrust thruster in _forwardThrusters)
//                {
//                    thruster.ThrustOverridePercentage = force;
//                }
//                force = (float)(acceleration.Dot(cockpitMatrix.Backward) * shipMass / BackwardMaxThrust);
//                foreach (IMyThrust thruster in _backwardThrusters)
//                {
//                    thruster.ThrustOverridePercentage = force;
//                }
//                force = (float)(acceleration.Dot(cockpitMatrix.Left) * shipMass / LeftMaxThrust);
//                foreach (IMyThrust thruster in _leftThrusters)
//                {
//                    thruster.ThrustOverridePercentage = force;
//                }
//                force = (float)(acceleration.Dot(cockpitMatrix.Right) * shipMass / RightMaxThrust);
//                foreach (IMyThrust thruster in _rightThrusters)
//                {
//                    thruster.ThrustOverridePercentage = force;
//                }
//                force = (float)(acceleration.Dot(cockpitMatrix.Up) * shipMass / UpMaxThrust);
//                foreach (IMyThrust thruster in _upThrusters)
//                {
//                    thruster.ThrustOverridePercentage = force;
//                }
//                force = (float)(acceleration.Dot(cockpitMatrix.Down) * shipMass / DownMaxThrust);
//                foreach (IMyThrust thruster in _downThrusters)
//                {
//                    thruster.ThrustOverridePercentage = force;
//                }
//            }

//            public void ResetThrust()
//            {
//                SetThrust(Vector3D.Zero);
//            }
//        }

//        //=======================================================================
//        //////////////////////////END////////////////////////////////////////////
//        //=======================================================================
//#if DEBUG
//    }
//}
//#endif