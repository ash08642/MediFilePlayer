using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediFilePlayer
{
    internal interface IMediPlayer
    {
        void startPlaying();
        void play();
        void pause();
        void stopPlaying();
    }
}
