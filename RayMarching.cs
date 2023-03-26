using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RayMarching : VolumeComponent, IPostProcessComponent
{
    public ColorParameter baseColor = new(Color.white);

    public bool IsActive() => baseColor.overrideState;
    public bool IsTileCompatible() => false;
}