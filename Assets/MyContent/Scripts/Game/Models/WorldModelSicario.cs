﻿using System;
using System.Collections.Generic;

public class WorldModelSicario: WorldModel{
    public float calm;
    public float money;
    public string weapon;
    public int bullets;
    public int stealCount;
    public float precisionFire;
    public float energy;
    public bool hasWeapon;
    public bool targetKilled;

    public override bool IsEqual(WorldModel a, WorldModel b) {
        return false;
    }

    public override WorldModel Clone(WorldModel baseModel) {
        var clone = new WorldModelDetective() { };

        return clone;
    }

    public override WorldModel UpdateValues(WorldModel wm, List<Func<WorldModel, WorldModel>> effects) {
        var newValue = Clone(wm);
        foreach (var e in effects) {
            newValue = e(newValue);
        }

        return newValue;
    }
}