﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfirePanel : UIPanel
{
    public void Continue()
    {
		AssetManager.Instance.GetManager<DungeonManager>().FinishCampRest();
    }
}
