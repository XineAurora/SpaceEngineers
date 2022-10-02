using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceEngineers
{
    public class PID
    {
        public double Kp { get; set; } = 0;
        public double Ki { get; set; } = 0;
        public double Kd { get; set; } = 0;
        public double ControlValue { get; set; } = 0;

        private bool _firstStart = true;
        private double _timeStep = 0;
        private double _prevError = 0;
        private double _integralError = 0;

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
}
