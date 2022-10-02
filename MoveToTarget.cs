#if DEBUG
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;

namespace SpaceEngineers
{
    public sealed class Program : MyGridProgram
    {
#endif
        //=======================================================================
        //////////////////////////BEGIN//////////////////////////////////////////
        //=======================================================================
        string gyroGroupName = "Gyros";
        string connectorName = "Connector";
        string cockpitName = "ControlSeat";
        string thrustersGroupName = "Thrusters";
        double kp = 4;
        double ki = 0;
        double kd = 20;
        
        //double gyroKoef = 1.0;

        bool isRunning = false;

        Vector3D targetConnectorRotation = new Vector3D(-0.0832507379034131, -0.528236026602448, 0.845006517630364);
        Vector3D targetConnectorPosition = new Vector3D(-38689.58, -38379.88, -27824.68);
        //Vector3D(-38644.6608811507, -38340.222034952, -27817.6415905685);

        IMyTextPanel textPanel;

        List<IMyGyro> gyros;
        IMyShipConnector connector;
        IMyCockpit cockpit;

        PID pidX, pidY, pidZ;
        ThrusterControl thrusterControl;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            gyros = new List<IMyGyro>();
            GridTerminalSystem.GetBlockGroupWithName(gyroGroupName).GetBlocksOfType<IMyGyro>(gyros);

            connector = GridTerminalSystem.GetBlockWithName(connectorName) as IMyShipConnector;
            cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;

            pidX = new PID(kp, ki, kd, 1.0 / 60.0);
            pidY = new PID(kp, ki, kd, 1.0 / 60.0);
            pidZ = new PID(kp, ki, kd, 1.0 / 60.0);

            thrusterControl = new ThrusterControl(this, cockpitName, thrustersGroupName);

            textPanel = GridTerminalSystem.GetBlockWithName("Text Panel") as IMyTextPanel;
        }

        public void Save()
        {
            // Используйте этот метод, чтобы сохранить состояние программы в поле Storage,
        }

        public void Main(string argument, UpdateType updateSource)
        {
            switch (argument)
            {
                case "run":
                    isRunning = true;
                    break;
                case "stop":
                    isRunning = false;
                    break;
            }

            if (isRunning)
            {
                Vector3D dir = targetConnectorPosition - connector.GetPosition();
                thrusterControl.SetThrust(CalculateAccelerationToTarget(dir) - cockpit.GetNaturalGravity());
            }
            else
            {
                ResetThrust();
            }
        }

        private Vector3D CalculateAccelerationToTarget(Vector3D direction)
        {
            //Vector3D expectedVelocity = direction * kVelocity;
            //Vector3D currentVelocity = cockpit.GetShipVelocities().LinearVelocity;
            //Vector3D errorVelocity = expectedVelocity - currentVelocity;
            //Vector3D controlVelocity = new Vector3D(pidX.Control(errorVelocity.X), pidY.Control(errorVelocity.Y), pidZ.Control(errorVelocity.Z));
            //textPanel.WriteText($"'{expectedVelocity}'\n'{errorVelocity}'\n'{controlVelocity}'");


            Vector3D controlVelocity = new Vector3D(pidX.Control(direction.X), pidY.Control(direction.Y), pidZ.Control(direction.Z));
            return controlVelocity;
        }

        private void Hover()
        {
            
        }

        private void ResetThrust()
        {
            thrusterControl.ResetThrust();
            pidX.Reset();
            pidY.Reset();
            pidZ.Reset();
        }

        public class PID
        {
            public double Kp { get; set; } = 0;
            public double Ki { get; set; } = 0;
            public double Kd { get; set; } = 0;
            public double ControlValue { get; set; } = 0;

            bool _firstStart = true;
            double _timeStep = 0;
            double _prevError = 0;
            double _integralError = 0;

            public PID(double kp, double ki, double kd, double timeStep)
            {
                Kp = kp;
                Ki = ki;
                Kd = kd;

                _timeStep = timeStep;
            }

            protected virtual double GetIntegral(double currentError)
            {
                return _integralError + currentError * _timeStep;
            }

            public double Control(double currentError)
            {
                double derivativeError = (currentError - _prevError) / _timeStep;
                if (_firstStart)
                {
                    _firstStart = false;
                    derivativeError = 0;
                }

                _integralError = GetIntegral(currentError);

                _prevError = currentError;

                ControlValue = Kp * currentError + Ki * _integralError + Kd * derivativeError;
                return ControlValue;
            }

            public virtual void Reset()
            {
                _firstStart = true;
                //_timeStep = 0;
                _prevError = 0;
                _integralError = 0;
            }

        }

        public class ThrusterControl
        {
            private List<IMyThrust> _forwardThrusters;
            private List<IMyThrust> _backwardThrusters;
            private List<IMyThrust> _leftThrusters;
            private List<IMyThrust> _rightThrusters;
            private List<IMyThrust> _upThrusters;
            private List<IMyThrust> _downThrusters;

            public double ForwardMaxThrust { get; protected set; } = 0;
            public double BackwardMaxThrust { get; protected set; } = 0;
            public double LeftMaxThrust { get; protected set; } = 0;
            public double RightMaxThrust { get; protected set; } = 0;
            public double UpMaxThrust { get; protected set; } = 0;
            public double DownMaxThrust { get; protected set; } = 0;

            private IMyCockpit _cockpit;

            public ThrusterControl(Program program, string mainCockpitName, string thrustersGroupName)
            {
                _cockpit = program.GridTerminalSystem.GetBlockWithName(mainCockpitName) as IMyCockpit;
                Matrix cockpitMatrix;
                _cockpit.Orientation.GetMatrix(out cockpitMatrix);

                _forwardThrusters = new List<IMyThrust>();
                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_forwardThrusters, (thruster) =>
                {
                    Matrix thrusterMatrix;
                    thruster.Orientation.GetMatrix(out thrusterMatrix);
                    if (thrusterMatrix.Forward == cockpitMatrix.Backward) return true;
                    return false;
                });
                _backwardThrusters = new List<IMyThrust>();
                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_backwardThrusters, (thruster) =>
                {
                    Matrix thrusterMatrix;
                    thruster.Orientation.GetMatrix(out thrusterMatrix);
                    if (thrusterMatrix.Forward == cockpitMatrix.Forward) return true;
                    return false;
                });
                _leftThrusters = new List<IMyThrust>();
                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_leftThrusters, (thruster) =>
                {
                    Matrix thrusterMatrix;
                    thruster.Orientation.GetMatrix(out thrusterMatrix);
                    if (thrusterMatrix.Forward == cockpitMatrix.Right) return true;
                    return false;
                });
                _rightThrusters = new List<IMyThrust>();
                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_rightThrusters, (thruster) =>
                {
                    Matrix thrusterMatrix;
                    thruster.Orientation.GetMatrix(out thrusterMatrix);
                    if (thrusterMatrix.Forward == cockpitMatrix.Left) return true;
                    return false;
                });
                _upThrusters = new List<IMyThrust>();
                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_upThrusters, (thruster) =>
                {
                    Matrix thrusterMatrix;
                    thruster.Orientation.GetMatrix(out thrusterMatrix);
                    if (thrusterMatrix.Forward == cockpitMatrix.Down) return true;
                    return false;
                });
                _downThrusters = new List<IMyThrust>();
                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_downThrusters, (thruster) =>
                {
                    Matrix thrusterMatrix;
                    thruster.Orientation.GetMatrix(out thrusterMatrix);
                    if (thrusterMatrix.Forward == cockpitMatrix.Up) return true;
                    return false;
                });
            }

            private void CalculateMaxThrust()
            {
                ForwardMaxThrust = 0;
                foreach (IMyThrust thruster in _forwardThrusters)
                {
                    ForwardMaxThrust += thruster.MaxEffectiveThrust;
                }
                BackwardMaxThrust = 0;
                foreach (IMyThrust thruster in _backwardThrusters)
                {
                    BackwardMaxThrust += thruster.MaxEffectiveThrust;
                }
                LeftMaxThrust = 0;
                foreach (IMyThrust thruster in _leftThrusters)
                {
                    LeftMaxThrust += thruster.MaxEffectiveThrust;
                }
                RightMaxThrust = 0;
                foreach (IMyThrust thruster in _rightThrusters)
                {
                    RightMaxThrust += thruster.MaxEffectiveThrust;
                }
                UpMaxThrust = 0;
                foreach (IMyThrust thruster in _upThrusters)
                {
                    UpMaxThrust += thruster.MaxEffectiveThrust;
                }
                DownMaxThrust = 0;
                foreach (IMyThrust thruster in _downThrusters)
                {
                    DownMaxThrust += thruster.MaxEffectiveThrust;
                }
            }

            public void SetThrust(Vector3D acceleration)
            {
                CalculateMaxThrust();
                Matrix cockpitMatrix = _cockpit.WorldMatrix;
                double shipMass = _cockpit.CalculateShipMass().PhysicalMass;
                float force = (float)(acceleration.Dot(cockpitMatrix.Forward) * shipMass / ForwardMaxThrust);
                foreach (IMyThrust thruster in _forwardThrusters)
                {
                    thruster.ThrustOverridePercentage = force;
                }
                force = (float)(acceleration.Dot(cockpitMatrix.Backward) * shipMass / BackwardMaxThrust);
                foreach (IMyThrust thruster in _backwardThrusters)
                {
                    thruster.ThrustOverridePercentage = force;
                }
                force = (float)(acceleration.Dot(cockpitMatrix.Left) * shipMass / LeftMaxThrust);
                foreach (IMyThrust thruster in _leftThrusters)
                {
                    thruster.ThrustOverridePercentage = force;
                }
                force = (float)(acceleration.Dot(cockpitMatrix.Right) * shipMass / RightMaxThrust);
                foreach (IMyThrust thruster in _rightThrusters)
                {
                    thruster.ThrustOverridePercentage = force;
                }
                force = (float)(acceleration.Dot(cockpitMatrix.Up) * shipMass / UpMaxThrust);
                foreach (IMyThrust thruster in _upThrusters)
                {
                    thruster.ThrustOverridePercentage = force;
                }
                force = (float)(acceleration.Dot(cockpitMatrix.Down) * shipMass / DownMaxThrust);
                foreach (IMyThrust thruster in _downThrusters)
                {
                    thruster.ThrustOverridePercentage = force;
                }
            }

            public void ResetThrust()
            {
                SetThrust(Vector3D.Zero);
            }
        }

        //=======================================================================
        //////////////////////////END////////////////////////////////////////////
        //=======================================================================
#if DEBUG
    }
}
#endif