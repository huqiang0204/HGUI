﻿using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Data2D
{
    public unsafe struct PointEffectorData
    {
        public bool useColliderMask;
        public int colliderMask;
        public float forceMagnitude;
        public float forceVariation;
        public float distanceScale;
        public float drag;
        public float angularDrag;
        public EffectorSelection2D forceSource;
        public EffectorSelection2D forceTarget;
        public EffectorForceMode2D forceMode;
        public static int Size = sizeof(PointEffectorData);
        public static int ElementSize = Size / 4;
    }
    public class PointEffectorLoader:DataLoader
    {
        public unsafe override void LoadToComponent(FakeStruct fake, Component game, FakeStruct main)
        {
            PointEffectorData* data = (PointEffectorData*)fake.ip;
            var obj = game.GetComponent<PointEffector2D>();
            if (obj == null)
                return;
            obj.useColliderMask = data->useColliderMask;
            obj.colliderMask = data->colliderMask;
            obj.forceMagnitude = data->forceMagnitude;
            obj.forceVariation = data->forceVariation;
            obj.distanceScale = data->distanceScale;
            obj.drag = data->drag;
            obj.angularDrag = data->angularDrag;
            obj.forceSource = data->forceSource;
            obj.forceTarget = data->forceTarget;
            obj.forceMode = data->forceMode;
        }
        public override unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var ae = com as PointEffector2D;
            if (ae == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, PointEffectorData.ElementSize);
            PointEffectorData* data = (PointEffectorData*)fake.ip;
            data->useColliderMask = ae.useColliderMask;
            data->colliderMask = ae.colliderMask;
            data->forceMagnitude = ae.forceMagnitude;
            data->forceVariation = ae.forceVariation;
            data->distanceScale = ae.distanceScale;
            data->drag = ae.drag;
            data->angularDrag = ae.angularDrag;
            data->forceSource = ae.forceSource;
            data->forceTarget = ae.forceTarget;
            data->forceMode = ae.forceMode;
            return fake;
        }
    }
}
