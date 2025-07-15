using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Application.Game.Interfaces
{
    public interface IGameTimer
    {
        int RemainingTime { get; }
        event Action TimeExpired;
        event Action<int> TimeTicked;

        Task StartAsync();
        void Stop();
        void Reset();
    }
}
