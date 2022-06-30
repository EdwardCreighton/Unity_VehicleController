using UnityEngine;

namespace Extensions
{
    public static class AdvMath
    {
        /// <summary>
        /// Radians-to-RevolutionsPerMinute conversion constant.
        /// </summary>
        public const float Rad2Rpm = 9.54929659f;
        
        /// <summary>
        /// RevolutionsPerMinute-Radians conversion constant.
        /// </summary>
        public const float Rpm2Rad = 0.10471976f;

        /// <summary>
        /// MetersPerSecond-KilometersPerHour conversion constant.
        /// </summary>
        public const float Mps2Kph = 3.6f;

        /// <summary>
        /// KilometersPerHour-MetersPerSecond conversion constant.
        /// </summary>
        public const float Kph2Mps = 0.27777778f;
        
        /// <summary>
        /// Function for safe division in case of zero denominator.
        /// </summary>
        /// <param name="numerator">numerator</param>
        /// <param name="denominator">denominator</param>
        /// <returns>
        /// If denominator is close to zero, returns 0.
        /// </returns>
        public static float SafeDivide0(float numerator, float denominator)
        {
            if (Mathf.Approximately(denominator, 0f))
            {
                return 0;
            }
            
            return numerator / denominator;
        }
        
        /// <summary>
        /// Function for safe division in case of zero denominator.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns>
        /// Returns numerator / denominator.
        /// </returns>
        public static float SafeDivide(float numerator, float denominator)
        {
            if (Mathf.Approximately(denominator, 0f))
            {
                return float.MaxValue * Mathf.Sign(numerator);
            }
            
            return numerator / denominator;
        }

        /// <summary>
        /// Get value sign
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// Returns -1 if value is lesser than 0,
        /// 0 if value is equal to 0,
        /// 1 if value is greater than 0
        /// </returns>
        public static float Sign(float value)
        {
            if (value > 0)
            {
                return 1f;
            }
            if (value < 0)
            {
                return -1f;
            }
            
            return 0f;
        }
    }
}
