﻿using System.Reflection;
using UnityEngine;

namespace AlcoholV.Detouring
{
    internal class ITab_Bills : RimWorld.ITab_Bills
    {
        private static readonly Vector2 WinSize = new Vector2(400f, 480f);

        public ITab_Bills()
        {
            var fieldInfo = typeof(RimWorld.ITab_Bills).GetField("WinSize", BindingFlags.NonPublic | BindingFlags.Static);
            fieldInfo?.SetValue(this, WinSize);
            size = WinSize;
        }
    }
}