using System.Collections;
using System.Collections.Generic;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class Clyde : Ghost
    {
        protected override float RespawnCoolTime => 10f;

        protected override float TrackingTime => 3f;
    }
}