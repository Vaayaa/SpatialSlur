﻿using SpatialSlur.SlurCore;

/*
 * Notes
 * 
 * References
 * http://webstaff.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
 */

namespace SpatialSlur.SlurField
{
    /// <summary>
    /// 
    /// </summary>
    public static class PerlinNoise
    {
        // permutation table
        private static readonly int[] _perm = new int[256];

        // 2d gradient table
        private static readonly double[] _grad2 =
        {
          1, 1,
          -1, 1,
          1, -1,
          -1, -1,
          1, 0,
          -1, 0,
          0, 1,
          0, -1,
        };

        // 3d gradient table
        private static readonly double[] _grad3 =
        {
          1, 1, 0,
          -1, 1, 0,
          1, -1, 0,
          -1, -1, 0,
          1, 0, 1,
          -1, 0, 1,
          1, 0, -1,
          -1, 0, -1,
          0, 1, 1,
          0, -1, 1,
          0, 1, -1,
          0, -1, -1
        };


        /// <summary>
        /// 
        /// </summary>
        static PerlinNoise()
        {
            SetPermutation(0);
        }


        /// <summary>
        /// Sets the permutation table.
        /// </summary>
        /// <param name="seed"></param>
        public static void SetPermutation(int seed)
        {
            // assign permutation table and shuffle
            for (int i = 0; i < 256; i++)
                _perm[i] = i;

            _perm.Shuffle(seed);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double ValueAt(Vec2d point)
        {
            return ValueAt(point.X, point.Y);
        }


        /// <summary>
        /// Returns the noise value at the given coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double ValueAt(double x, double y)
        {
            // get whole and franctional componenets
            x = SlurMath.Fract(x, out int i);
            y = SlurMath.Fract(y, out int j);

            // calculate noise contributions from each corner
            double n00 = GradDot(ToIndex(i, j), x, y);
            double n10 = GradDot(ToIndex(i + 1, j), x - 1.0, y);
            double n01 = GradDot(ToIndex(i, j + 1), x, y - 1.0);
            double n11 = GradDot(ToIndex(i + 1, j + 1), x - 1.0, y - 1.0);

            // eased values for x and y
            x = SlurMath.HermiteC2(x);
            y = SlurMath.HermiteC2(y);

            // bilinear interpolation
            return SlurMath.Lerp(
              SlurMath.Lerp(n00, n10, x),
              SlurMath.Lerp(n01, n11, x),
              y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static double ValueAt(Vec3d point)
        {
            return ValueAt(point.X, point.Y, point.Z);
        }


        /// <summary>
        /// Returns the noise value at the given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double ValueAt(double x, double y, double z)
        {
            // get whole and franctional componenets
            x = SlurMath.Fract(x, out int i);
            y = SlurMath.Fract(y, out int j);
            z = SlurMath.Fract(z, out int k);

            // calculate noise contributions from each corner
            double n000 = GradDot(ToIndex(i, j, k), x, y, z);
            double n100 = GradDot(ToIndex(i + 1, j, k), x - 1.0, y, z);
            double n010 = GradDot(ToIndex(i, j + 1, k), x, y - 1.0, z);
            double n110 = GradDot(ToIndex(i + 1, j + 1, k), x - 1.0, y - 1.0, z);

            double n001 = GradDot(ToIndex(i, j, k + 1), x, y, z - 1.0);
            double n101 = GradDot(ToIndex(i + 1, j, k + 1), x - 1.0, y, z - 1.0);
            double n011 = GradDot(ToIndex(i, j + 1, k + 1), x, y - 1.0, z - 1.0);
            double n111 = GradDot(ToIndex(i + 1, j + 1, k + 1), x - 1.0, y - 1.0, z - 1.0);

            // eased values for each dimension
            x = SlurMath.HermiteC2(x);
            y = SlurMath.HermiteC2(y);
            z = SlurMath.HermiteC2(z);

            // trilinear interpolation
            return SlurMath.Lerp(
              SlurMath.Lerp(
              SlurMath.Lerp(n000, n100, x),
              SlurMath.Lerp(n010, n110, x),
              y),
              SlurMath.Lerp(
              SlurMath.Lerp(n001, n101, x),
              SlurMath.Lerp(n011, n111, x),
              y),
              z);
        }


        /// <summary>
        /// Returns the gradient table index for the given coordinates.
        /// </summary>
        private static int ToIndex(int i, int j)
        {
            return (_perm[(i + _perm[j & 255]) & 255] & 7) << 1;
        }


        /// <summary>
        /// Returns the gradient table index for the given coordinates.
        /// </summary>
        private static int ToIndex(int i, int j, int k)
        {
            return _perm[(i + _perm[(j + _perm[k & 255]) & 255]) & 255] % 12 * 3;
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GradDot(int index, double x, double y)
        {
            return _grad2[index] * x + _grad2[index + 1] * y;
        }


        /// <summary>
        /// 
        /// </summary>
        private static double GradDot(int index, double x, double y, double z)
        {
            return _grad3[index] * x + _grad3[index + 1] * y + _grad3[index + 2] * z;
        }


        /// <summary>
        /// Returns the gradient of noise values.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec2d GradientAt(Vec2d point, double delta)
        {
            return GradientAt(point.X, point.Y, delta);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec2d GradientAt(double x, double y, double delta)
        {
            var gx = (ValueAt(x + delta, y) - ValueAt(x - delta, y));
            var gy = (ValueAt(x, y + delta) - ValueAt(x, y - delta));
            return new Vec2d(gx, gy) / (delta * 2.0);
        }


        /// <summary>
        /// Returns the gradient of noise values.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec3d GradientAt(Vec3d point, double delta)
        {
            return GradientAt(point.X, point.Y, delta);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec3d GradientAt(double x, double y, double z, double delta)
        {
            var gx = (ValueAt(x + delta, y, z) - ValueAt(x - delta, y, z));
            var gy = (ValueAt(x, y + delta, z) - ValueAt(x, y - delta, z));
            var gz = (ValueAt(x, y, z + delta) - ValueAt(x, y, z - delta));
            return new Vec3d(gx, gy, gz) / (delta * 2.0);
        }


        /// <summary>
        /// Returns the curl of noise vectors i.e. vectors whose components equal the noise value at the given location.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static double CurlAt(Vec2d point, double delta)
        {
            return CurlAt(point.X, point.Y, delta);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static double CurlAt(double x, double y, double delta)
        {
            var dx = (ValueAt(x + delta, y) - ValueAt(x - delta, y));
            var dy = (ValueAt(x, y + delta) - ValueAt(x, y - delta));
            return (dx - dy) / (delta * 2.0);
        }


        /// <summary>
        /// Returns the curl of noise vectors i.e. vectors whose components equal the noise value at the given location.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Vec3d CurlAt(Vec3d point, double delta)
        {
            return GradientAt(point.X, point.Y, delta);
        }


       /// <summary>
       /// 
       /// </summary>
       /// <param name="x"></param>
       /// <param name="y"></param>
       /// <param name="z"></param>
       /// <param name="delta"></param>
       /// <returns></returns>
        public static Vec3d CurlAt(double x, double y, double z, double delta)
        {
            var dx = ValueAt(x + delta, y, z) - ValueAt(x - delta, y, z);
            var dy = ValueAt(x, y + delta, z) - ValueAt(x, y - delta, z);
            var dz = ValueAt(x, y, z + delta) - ValueAt(x, y, z - delta);
            return new Vec3d(dy - dz, dz - dx, dx - dy) / (2.0 * delta);
        }
    }
}
