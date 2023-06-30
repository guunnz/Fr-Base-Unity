namespace Tools
{
    public static class Chaining
    {


        public static T As<T>(this object obj) where T : class
        {
            return obj as T;
        }


        public static bool HasAndIs(this bool? boolean)
        {
            return boolean.HasValue && boolean.Value;
        }
        
        
        
    }
}