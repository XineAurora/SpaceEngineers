using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class ThrusterControl
    {
        private List<IMyThrust>[] _thrusters;

        private double[] _maxThrusts;

        private IMyCockpit _cockpit;
        private Vector3D[] _cockpitDirections;

        public ThrusterControl(Program program, string mainCockpitName, string thrustersGroupName)
        {
            _maxThrusts = new double[6];
            _thrusters = new List<IMyThrust>[6];
            _cockpitDirections = new Vector3D[6];
            for (int i = 0; i < 6; ++i)
            {
                _thrusters[i] = new List<IMyThrust>();
                _cockpitDirections[i] = new Vector3D();
                _maxThrusts[i] = 0;
            }
            
            _cockpit = program.GridTerminalSystem.GetBlockWithName(mainCockpitName) as IMyCockpit;
            Matrix cockpitMatrix;
            _cockpit.Orientation.GetMatrix(out cockpitMatrix);

            _cockpitDirections[(int)Base6Directions.Direction.Forward] = cockpitMatrix.Forward;
            _cockpitDirections[(int)Base6Directions.Direction.Backward] = cockpitMatrix.Backward;
            _cockpitDirections[(int)Base6Directions.Direction.Left] = cockpitMatrix.Left;
            _cockpitDirections[(int)Base6Directions.Direction.Right] = cockpitMatrix.Right;
            _cockpitDirections[(int)Base6Directions.Direction.Up] = cockpitMatrix.Up;
            _cockpitDirections[(int)Base6Directions.Direction.Down] = cockpitMatrix.Down;

            foreach (Base6Directions.Direction direction in Enum.GetValues(typeof(Base6Directions.Direction)))
            {
                program.GridTerminalSystem.GetBlockGroupWithName(thrustersGroupName).GetBlocksOfType<IMyThrust>(_thrusters[(int)direction], (thruster) =>
                {
                    Matrix thrusterMatrix;
                    thruster.Orientation.GetMatrix(out thrusterMatrix);
                    return (thrusterMatrix.Backward == _cockpitDirections[(int)direction]);
                });
            }
        }

        private void CalculateMaxThrust()
        {
            foreach (Base6Directions.Direction direction in Enum.GetValues(typeof(Base6Directions.Direction)))
            {
                _maxThrusts[(int)direction] = 0;
                foreach(IMyThrust thruster in _thrusters[(int)direction])
                {
                    _maxThrusts[(int)direction] += thruster.MaxEffectiveThrust;
                }
            }
        }

        public void SetThrust(Vector3D acceleration)
        {
            CalculateMaxThrust();
            double shipMass = _cockpit.CalculateShipMass().PhysicalMass;

            Matrix cockpitWorldMatrix = _cockpit.WorldMatrix;
            Vector3D[] cockpitWorldDirections = new Vector3D[6];
            cockpitWorldDirections[(int)Base6Directions.Direction.Forward] = cockpitWorldMatrix.Forward;
            cockpitWorldDirections[(int)Base6Directions.Direction.Backward] = cockpitWorldMatrix.Backward;
            cockpitWorldDirections[(int)Base6Directions.Direction.Left] = cockpitWorldMatrix.Left;
            cockpitWorldDirections[(int)Base6Directions.Direction.Right] = cockpitWorldMatrix.Right;
            cockpitWorldDirections[(int)Base6Directions.Direction.Up] = cockpitWorldMatrix.Up;
            cockpitWorldDirections[(int)Base6Directions.Direction.Down] = cockpitWorldMatrix.Down;

            foreach (Base6Directions.Direction direction in Enum.GetValues(typeof(Base6Directions.Direction)))
            {
                float force = (float)(acceleration.Dot(cockpitWorldDirections[(int)direction]) * shipMass / _maxThrusts[(int)direction]);
                foreach (IMyThrust thruster in _thrusters[(int)direction])
                {
                    thruster.ThrustOverridePercentage = force;
                }
            }
        }

        public void ResetThrust()
        {
            SetThrust(Vector3D.Zero);
        }
    }
}
