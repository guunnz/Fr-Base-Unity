﻿using UnityEngine;

namespace Graph._3rdparty.xNode.Scripts.Attributes
{
    /// <summary> Draw enums correctly within nodes. Without it, enums show up at the wrong positions. </summary>
    /// <remarks> Enums with this attribute are not detected by EditorGui.ChangeCheck due to waiting before executing </remarks>
    public class NodeEnumAttribute : PropertyAttribute
    {
    }
}