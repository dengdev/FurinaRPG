using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyMenuCommands
{
    [MenuItem("Tools/Spawn Orc")]
    private static void SpawnOrc() {
        FactoryManager.Instance.SpawnEnemiesOverTime(" ﬁ»À", 5, new Vector3(5, 0.1f, 4), 5.0f);
    }
}
