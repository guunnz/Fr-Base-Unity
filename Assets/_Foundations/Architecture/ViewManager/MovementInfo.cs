using System;

namespace Architecture.ViewManager
{
    public struct MovementInfo
    {
        public Type goToNode;
        public string getOut;
        public bool modal;
        public object[] parameters;


        public void SetGoToNode(Type node, bool isModal = false, params object[] instanceParameters)
        {
            Consume();
            parameters = instanceParameters;
            goToNode = node;
            modal = isModal;
        }

        public void SetGetOut(string outputKey)
        {
            Consume();
            getOut = outputKey;
        }

        public void Consume()
        {
            goToNode = null;
            getOut = string.Empty;
            parameters = Array.Empty<object>();
            modal = false;
        }

        public bool UseGoToNode(out Type nodeType, out object[] instanceParameters)
        {
            nodeType = goToNode;
            instanceParameters = this.parameters;
            return nodeType != null;
        }

        public bool IsModal()
        {
            return modal && UseGoToNode(out _, out _);
        }

        public bool UseGetOut(out string outPort)
        {
            outPort = getOut;
            return !string.IsNullOrEmpty(getOut);
        }


        public void Clear()
        {
            Consume();
        }
    }
}