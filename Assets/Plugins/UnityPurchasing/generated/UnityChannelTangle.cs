#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class UnityChannelTangle
    {
        private static byte[] data = System.Convert.FromBase64String("I/fj4zPUhpfdbrGbI5AbqWCi4aJHd4Rgg37vS5jFWU/vhst2coKpCtEPaqGNcgeL6ruuBsF7QiEYzTCAJ5UWNScaER49kV+R4BoWFhYSFxQS/Wnp35b1PCexXFhFcga5u++vH75vL5t1EL9wF6YoQ4aVv2RzMXcx0T8ABM6vTuXiVRF3/zu7mRJm6IlRqeoV1YRYFBI66ZHxB0bOFuuy4iYECWYsJD3e+E6NViCUsH8rBDMRxyAsuc3y/Y3nH6sbsty+u4K3Nf2VFhgXJ5UWHRWVFhYXjIUPaY40eZ0elpQDBN+moijcQhnIgsKsUOLOmI2Zx/3IZGeL6aX9hEIGcUohuflV48TKrQW+YxXNQI7cOdvtGborz3xOgHkH2GNVEBUUFhcW");
        private static int[] order = new int[] { 2,5,12,12,10,7,11,10,13,9,10,11,12,13,14 };
        private static int key = 23;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
