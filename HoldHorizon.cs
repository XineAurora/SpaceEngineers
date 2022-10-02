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
//        string seatName = "ControlSeat";
//        double gyroKoef = 1.0;


//        IMyShipController sc;
//        List<IMyGyro> gyros;
//        bool hold_horizon = false;
//        bool hold_position = false;
//        Vector3D startPoint;


//        public Program()
//        {
//            gyros = new List<IMyGyro>();
//            Runtime.UpdateFrequency = UpdateFrequency.Update1;
//            sc = GridTerminalSystem.GetBlockWithName(seatName) as IMyShipController;
//            GridTerminalSystem.GetBlockGroupWithName(gyroGroupName).GetBlocksOfType<IMyGyro>(gyros);
//        }

//        public void Save()
//        {
//            // Используйте этот метод, чтобы сохранить состояние программы в поле Storage,
//        }

//        public void Main(string argument, UpdateType updateSource)
//        {
//            switch (argument)
//            {
//                case "hold_horizon":
//                    hold_horizon = true;
//                    break;
//                case "hold_position":
//                    startPoint = sc.GetPosition();
//                    hold_position = true;
//                    break;
//                case "stop":
//                    hold_horizon = false;
//                    hold_position = false;
//                    Reset();
//                    break;
//            }

//            if (hold_horizon)
//            {
//                HoldHorizon();
//            }

//        }

//        private void HoldHorizon()
//        {
//            Vector3D currentRotation = sc.WorldMatrix.Down;
//            Vector3D grav = sc.GetNaturalGravity();
//            grav.Normalize();
//            Vector3D rotationAxis = grav.Cross(currentRotation);
//            if (grav.Dot(currentRotation) < 0)
//            {
//                rotationAxis.Normalize();
//            }

//            foreach (IMyGyro gyro in gyros)
//            {
//                gyro.GyroOverride = true;
//                gyro.Pitch = (float)gyro.WorldMatrix.Right.Dot(rotationAxis);
//                gyro.Yaw = (float)gyro.WorldMatrix.Up.Dot(rotationAxis);
//                gyro.Roll = (float)gyro.WorldMatrix.Forward.Dot(rotationAxis);

//            }
//        }

//        private void Reset()
//        {
//            foreach (IMyGyro gyro in gyros)
//            {
//                gyro.GyroOverride = false;
//            }
//        }

//        //=======================================================================
//        //////////////////////////END////////////////////////////////////////////
//        //=======================================================================
//#if DEBUG
//    }
//}
//#endif