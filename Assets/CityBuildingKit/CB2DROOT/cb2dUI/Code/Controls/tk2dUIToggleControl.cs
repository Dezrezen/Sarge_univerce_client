using UnityEngine;
using System.Collections;

/// <summary>
/// Toggle control that have both a toggle button and a description text
/// </summary>
[AddComponentMenu("CB 2D/UI/tk2dUIToggleControl")]
public class tk2dUIToggleControl : tk2dUIToggleButton
{
    /// <summary>
    /// Description of the toggle button
    /// </summary>
    public tk2dTextMesh descriptionTextMesh;
}
