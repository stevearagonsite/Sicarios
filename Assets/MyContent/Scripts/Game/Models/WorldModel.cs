﻿using System;
using System.Collections.Generic;

public abstract class WorldModel{
    public abstract bool IsEqual(WorldModel a, WorldModel b);
    public abstract WorldModel Clone(WorldModel baseModel);
    public abstract WorldModel UpdateValues(WorldModel wm, List<Func<WorldModel, WorldModel>> effects);
}