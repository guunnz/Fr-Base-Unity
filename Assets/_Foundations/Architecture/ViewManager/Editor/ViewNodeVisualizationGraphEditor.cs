using System.Collections.Generic;
using Graph._3rdparty.xNode.Scripts.Editor;
using UnityEngine;

namespace Architecture.ViewManager.Editor
{
    [CustomNodeGraphEditor(typeof(ViewNodesNavigation), "Bones.Settings")]
    public class ViewNodeVisualizationGraphEditor : NodeGraphEditor
    {
        public override NodeEditorPreferences.Settings GetDefaultPreferences()
        {
            var color = new Color(0.47f, 1f, 0.67f);
            return new NodeEditorPreferences.Settings
            {
                gridBgColor = new Color(0.4f, 0.54f, 1f),
                gridLineColor = new Color(0.37f, 0.88f, 1f),
                typeColors = new Dictionary<string, Color>
                {
                    {typeof(string).PrettyName(), color},
                    {typeof(int).PrettyName(), color}
                }
            };
        }
    }
}