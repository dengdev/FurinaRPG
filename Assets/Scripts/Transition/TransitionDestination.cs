using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour {
    public enum DestinationTag { AToB, BToA, CToD, DToC,Enter }
    public DestinationTag destinationtag;
}
