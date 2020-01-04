#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("rlcvUgjkXJPG0YbwM2NmuMjTFDcQjM78QFSS7gV3KjUCut8+B8O8NhDDxevONf/yl30Qw4hX92lnB3zCyzxze+Rh3qnNH0Pe9ibay8MRW936eXd4SPp5cnr6eXl4wYIFdGo9Z7lToYVzLbDwpGsXcBIHxW5O9ehbNnNaRbzOqPetf1b/BWKNJAbtC9zc6nBl2oH0H8F2Atuv68xMhBjZuf4jtdkvFr3SshM1eODjCDfj4tfnyf/FBAE3AkccDdMxwenkmLPX3QwUPQLZrzuWQNYkRi0dzTFViCIm5Ej6eVpIdX5xUv4w/o91eXl5fXh7OJu0LzQjC9qDKoOof8kh5b3xQihr2H5jXLu0W24IdDkxqfZFbU6TZTUpSXqelmRgA3p7eXh5");
        private static int[] order = new int[] { 10,7,5,5,7,6,13,9,12,11,12,12,13,13,14 };
        private static int key = 120;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
