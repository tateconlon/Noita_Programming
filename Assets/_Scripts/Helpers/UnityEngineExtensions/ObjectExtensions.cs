namespace UnityEngine
{
    public static class ObjectExtensions
    {
        public static bool TryGetObjectFromInstanceID(int instanceID, out Object outObject)
        {
            if (Object.DoesObjectWithInstanceIDExist(instanceID))
            {
                outObject = Object.FindObjectFromInstanceID(instanceID);
                return true;
            }

            outObject = null;
            return false;
        }
    }
}