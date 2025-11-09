using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGame
{

    public class Pinky : Ghost
    {
        protected override float RespawnCoolTime => 5f;
        protected override float TrackingTime => 5f;
    }
}
