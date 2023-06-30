using Graph._3rdparty.xNode.Scripts;

namespace Architecture.ViewManager
{
    [CreateNodeMenu("New")]
    public class ViewNodeVisualization : Node
    {
        [Input(ShowBackingValue.Never, connectionType = ConnectionType.Multiple)]
        public string input = "";

        [Output(ShowBackingValue.Always, dynamicPortList = true, connectionType = ConnectionType.Override)]
        public string[] outputs = {"back"};


        public ViewNode prefab;


        private void OnValidate()
        {
            (graph as ViewNodesNavigation)?.ValidateNode(this);
            if (prefab)
                name = prefab.GetType().Name;
        }


        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}