using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public class ProceduralMapManagerGame : ProceduralMapManager
{
    [Header("Player")]
    [SerializeField] Player playerPrefab = default;

    public override IEnumerator EndGeneration()
    {
        yield return base.EndGeneration();

        //instantiate player
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }
}
