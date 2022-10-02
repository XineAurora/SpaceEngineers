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
//        double gyroKoef = 1.0;

//        bool isRunning = false;

//        Vector3D targetConnectorRotation = new Vector3D(-0.0832507379034131, -0.528236026602448, 0.845006517630364);
//        Vector3D targetConnectorPosition = new Vector3D(-38644.6608811507, -38340.222034952, -27817.6415905685);

//        IMyTextPanel textPanel;

//        List<IMyGyro> gyros;
//        IMyShipConnector connector;
//        IMyCockpit cockpit;

//        public Program()
//        {
//            gyros = new List<IMyGyro>();
//            GridTerminalSystem.GetBlockGroupWithName(gyroGroupName).GetBlocksOfType<IMyGyro>(gyros);
//            connector = GridTerminalSystem.GetBlockWithName(connectorName) as IMyShipConnector;
//            cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;
//            Runtime.UpdateFrequency = UpdateFrequency.Update1;

//            textPanel = GridTerminalSystem.GetBlockWithName("Text Panel") as IMyTextPanel;
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
//                GyrosResetRotation();
//                Rotate(-targetConnectorRotation, connector.WorldMatrix.Forward); //RotateForConnection()
//                Rotate(cockpit.GetNaturalGravity(), cockpit.WorldMatrix.Down); //RotateAgainstGravity();
//            }
//            else
//            {
//                foreach (IMyGyro gyro in gyros)
//                {
//                    gyro.GyroOverride = false;
//                }
//            }
//        }

//        private void Rotate(Vector3D targetRotation, Vector3D currentRotation)
//        {
//            targetRotation.Normalize();
//            currentRotation.Normalize();
//            Vector3D rotationAxis = targetRotation.Cross(currentRotation);
//            if (targetRotation.Dot(currentRotation) < 0)
//            {
//                rotationAxis.Normalize();
//            }

//            foreach (IMyGyro gyro in gyros)
//            {
//                double pitch = gyro.WorldMatrix.Right.Dot(rotationAxis);
//                double yaw = gyro.WorldMatrix.Up.Dot(rotationAxis);
//                double roll = gyro.WorldMatrix.Backward.Dot(rotationAxis);
//                GyroAddRotation(gyro, pitch, yaw, roll);
//            }
//        }

//        private void GyroAddRotation(IMyGyro gyro, double pitch, double yaw, double roll)
//        {
//            gyro.GyroOverride = true;
//            gyro.Pitch += (float)(pitch * gyroKoef);
//            gyro.Yaw += (float)(yaw * gyroKoef);
//            gyro.Roll += (float)(roll * gyroKoef);
//        }

//        private void GyrosResetRotation()
//        {
//            foreach(IMyGyro gyro in gyros)
//            {
//                gyro.Pitch = 0;
//                gyro.Yaw = 0;
//                gyro.Roll = 0;
//            }
//        }

//        //=======================================================================
//        //////////////////////////END////////////////////////////////////////////
//        //=======================================================================
//#if DEBUG
//    }
//}
//#endif