using System;

namespace AutoExecuteNode
{
    public class Tools
    {
        static public string print_vector(Vector3d vec)
        {
            return $"{vec.x:n2} {vec.y:n2} {vec.z:n2}";
        }

        static public string print_duration(double secs)
        {

            TimeSpan t = TimeSpan.FromSeconds(secs);

            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
        }
    }


}