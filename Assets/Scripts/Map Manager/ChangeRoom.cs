using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRoom : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] RoomGame roomToDeactivate = default;
    [SerializeField] RoomGame roomToActivate = default;
}
