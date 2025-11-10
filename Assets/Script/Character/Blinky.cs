using System.Collections;
using System.Collections.Generic;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class Blinky : Ghost
    {
        protected override float RespawnCoolTime => 0f;
        protected override float TrackingTime => 8f;
    }
}
