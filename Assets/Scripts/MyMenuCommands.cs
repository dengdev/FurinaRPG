using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyMenuCommands
{
    [MenuItem("Tools/Spawn Orc")]
    private static void SpawnOrc() {
        FactoryManager.Instance.SpawnEnemiesOverTime("兽人", 5, new Vector3(5, 0.01f, 4), 5.0f);
    }

    [MenuItem("Tools/Spawn Golem")]
    private static void SpawnGiant() {
        FactoryManager.Instance.SpawnEnemiesOverTime("石头人", 1, new Vector3(8, 0.01f, 4), 5.0f);
    }
}
