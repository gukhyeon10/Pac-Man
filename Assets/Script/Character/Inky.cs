using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGame
{
    public class Inky : Ghost
    {
        protected override float RespawnCoolTime => 8f;

        protected override float TrackingTime => 4f;
    }
}
