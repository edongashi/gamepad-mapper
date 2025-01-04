using System;
using System.Collections.Generic;
using GamepadMapper.Actuators;
using GamepadMapper.Configuration;
using GamepadMapper.Input;

namespace GamepadMapper.Handlers
{
    public class RadialHandler : IAnalogHandler
    {
        private struct AngleData
        {
            public AngleData(double angle)
            {
                Sin = Math.Sin(angle);
                Cos = Math.Cos(angle);
            }

            public double Sin { get; }

            public double Cos { get; }
        }

        private readonly Queue<AngleData> memory;

        public RadialHandler(IRadialActuator radialActuator, IRadialConfiguration configuration)
        {
            RadialActuator = radialActuator;
            Configuration = configuration;
            memory = new Queue<AngleData>();
        }

        public IRadialConfiguration Configuration { get; }

        public IRadialActuator RadialActuator { get; }

        public void ClearState()
        {
            memory.Clear();
        }

        public void Update(AnalogState state, FrameDetails frame)
        {
            var x = Configuration.InvertX ? -state.X : state.X;
            var y = Configuration.InvertY ? state.Y : -state.Y;
            var distance = Math.Sqrt(x * x + y * y);
            if (distance <= Configuration.MinRadius)
            {
                memory.Clear();
                distance = 0d;
            }
            else
            {
                distance = (distance - Configuration.MinRadius) / (1d - Configuration.MinRadius);
            }

            var angle = Math.Atan2(y, x);
            memory.Enqueue(new AngleData(angle));
            var avgCos = 0d;
            var avgSin = 0d;
            foreach (var item in memory)
            {
                avgCos += item.Cos;
                avgSin += item.Sin;
            }

            avgCos /= memory.Count;
            avgSin /= memory.Count;
            var avgAngle = Math.Atan2(avgSin, avgCos);
            while (memory.Count > Configuration.Smoothing / frame.FrameTime)
            {
                memory.Dequeue();
            }

            RadialActuator.Update((avgAngle * 180d / Math.PI + 90d) % 360d, distance);
        }
    }
}
