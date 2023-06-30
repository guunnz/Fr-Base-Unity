namespace UI.Utils
{
    public class StringFieldComposite : UIWidget<string>
    {
        private StringWidget[] children;
        private StringWidget[] Children => /*children ??=*/ GetComponentsInChildren<StringWidget>();


        private void Start() => InvalidateCache();

        private void InvalidateCache() => children = null;

        private void OnTransformChildrenChanged() => InvalidateCache();

        public override string Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                foreach (var widget in Children)
                {
                    widget.Value = value;
                }
            }
        }
    }
}